﻿using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using CamObserver.Display.Data;
using CamObserver.Display.Helpers;
using CamObserver.Models;
using CamObserver.Tools;
using DepthAI.Core;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
namespace CamObserver.Display
{
    public partial class MainDisplay : Form
    {
        //0 for false, 1 for true.
        private static int usingResource = 0;
        
        private System.Timers.Timer PushTimer;
        private System.Timers.Timer PushImageTimer;
        int ImgHeight = 416, ImgWidth = 416;
        Point StartLocation;
        bool IsSelect = false;
        Rectangle SelectionArea = new Rectangle(0, 0, 0, 0);
        DataCounterService dataCounterService;
        bool IsCapturing = false;
        string SelectedFile;
        CancellationTokenSource source;
        AppConfig appConfig;
        Tracker tracker;
        List<OakDeviceItem> ListDevice;
        int DelayTime = 1;
        public MainDisplay()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            //services.AddSingleton<WeatherForecastService>();
            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<Main>("#app");
            Setup();
            if (AppConstants.AutoStart) Start();
        }



        //DaiFaceDetector face;
        //DaiStreams stream;
        DaiObjectDetector objdet;
        //DaiBodyPose pose;


        record analysistype(string name, string value);



        void UpdateStatLoop(CancellationToken token)
        {
            while (true)
            {
                var row = 0;
                var dt = tracker.GetLogTable();
                AppConstants.InfoStat.Clear();

                foreach (var fil in filter)
                {
                    var count = dt.Where(x => x.Jenis == fil).Count();
                    AppConstants.InfoStat.Add(new ObjekStatistik() { Jumlah = count, No = row++, Objek = fil });
                }
                if (token.IsCancellationRequested) break;
                Thread.Sleep(1000);
            }
        }

        void Clear()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)async delegate ()
                {
                    PicBox1.Image = null;

                    TxtInfo.Clear();

                });
            }
            else
            {
                PicBox1.Image = null;

                TxtInfo.Clear();

            }
        }
        HttpClient client = new();
        const string PushImageApiUrl = "https://camobserver.my.id/api/cctv/sendimage";
        static byte[] CloudImage;
        bool AddCloudImage(byte[] ImgData)
        {
            //0 indicates that the method is not in use.
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                Console.WriteLine("{0} acquired the lock", Thread.CurrentThread.Name);

                CloudImage = ImgData;

                Console.WriteLine("{0} exiting lock", Thread.CurrentThread.Name);

                //Release the lock
                Interlocked.Exchange(ref usingResource, 0);
                return true;
            }
            else
            {
                Console.WriteLine("{0} was denied the lock", Thread.CurrentThread.Name);
                return false;
            }
        }
        
        async Task<bool> PushImageToCloud(string CctvName, byte[] ImageData)
        {
            try
            {
                var info = new CCTVImage() { CctvName = CctvName, ImageBytes = ImageData, CreatedDate = DateTime.Now };
                var json = System.Text.Json.JsonSerializer.Serialize(info);
                var hasil = await client.PostAsync(PushImageApiUrl, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                if (hasil.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{DateTime.Now} => push image dari {CctvName} sukses");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now} => push image dari {CctvName} gagal, {await hasil.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        void Setup()
        {

            var manager = new OAKDeviceManager();
            ListDevice = manager.GetAvailableDevices();
            if (!ListDevice.Any())
            {
                MessageBox.Show("Camera OAK tidak ditemukan, silakan periksa kembali kameranya.");
                Application.Exit();
                return;
            }
            dataCounterService = ObjectContainer.Get<DataCounterService>();

            tracker = new Tracker();

            BtnSave.Click += (a, b) =>
            {
                //yolo.SaveLog();
                tracker.SaveToLog();
            };

            BtnSync.Click += async (a, b) =>
            {
                await SyncToCloud();
            };

            this.FormClosing += Form1_FormClosing;

            BtnOpen.Click += async (a, b) =>
            {
                var fname = OpenFileDialogForm();
                if (!string.IsNullOrEmpty(fname))
                {
                    SelectedFile = fname;
                    this.Text = $"Cam Observer v1.0 ({SelectedFile})";
                }
            };

            PicBox1.Resize += (object? sender, EventArgs e) =>
            {
                ImgHeight = PicBox1.Height;
                ImgWidth = PicBox1.Width;

            };
            PicBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            PicBox1.Paint += (object? sender, PaintEventArgs e) =>
            {
                if (SelectionArea.Width > 0)
                {
                    var pen = new Pen(System.Drawing.Color.LightGreen, 2);
                    e.Graphics.DrawRectangle(pen, SelectionArea);
                }
            };
            PicBox1.MouseDown += (object? sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    IsSelect = true;
                    StartLocation = e.Location;
                }
            };
            PicBox1.MouseMove += (object? sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && IsSelect)
                {
                    SelectionArea.X = Math.Min(StartLocation.X, e.X);
                    SelectionArea.Y = Math.Min(StartLocation.Y, e.Y);
                    SelectionArea.Width = Math.Abs(StartLocation.X - e.X);
                    SelectionArea.Height = Math.Abs(StartLocation.Y - e.Y);
                    PicBox1.Invalidate();
                }
            };
            PicBox1.MouseUp += (object? sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && IsSelect)
                {
                    IsSelect = false;
                    //detector.SetSelectionArea(SelectionArea);

                }
                else if (e.Button == MouseButtons.Right)
                {
                    SelectionArea.X = 0;
                    SelectionArea.Y = 0;
                    SelectionArea.Width = 0;
                    SelectionArea.Height = 0;
                }
            };
            //stream = new();
            BtnStart.Click += (a, b) =>
            {
                Start();
            };
            BtnStop.Click += (a, b) =>
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
                /*
                    face.FinishDevice();
                    face.Dispose();
                  */
                source.Cancel();
                objdet.FinishDevice();
                objdet.Dispose();
                IsCapturing = false;

            };
            appConfig = new();
            BtnConfig.Click += (a, b) =>
            {
                if (SelectionArea != null)
                {
                    var cord = $"{SelectionArea.X},{SelectionArea.Y},{SelectionArea.Width},{SelectionArea.Height}";
                    appConfig.MyConfig["Coords"]["SelectionArea"] = cord;
                    appConfig.Save();
                    TxtStatus.Text = "Save config ok";
                }
            };
            var selStr = appConfig.MyConfig["Coords"]["SelectionArea"].ToString();
            if (selStr != null && !string.IsNullOrEmpty(selStr))
            {
                var cord = selStr.Split(",");
                SelectionArea = new Rectangle(int.Parse(cord[0]), int.Parse(cord[1]), int.Parse(cord[2]), int.Parse(cord[3]));
                PicBox1.Invalidate();
            }
            PushTimer = new();// System.Windows.Forms.Timer();
            PushTimer.Interval = AppConstants.SyncDelay;
            TxtInfo.Text += $"Sync Time: {AppConstants.SyncDelay / 1000} seconds";
            PushTimer.Elapsed += async (a, b) =>
            {
                try
                {
                    PushTimer.Stop();
                    if (!CheckInternetConnection())
                    {
                        if (InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)async delegate ()
                            {
                                TxtStatus.Text = ("Internet connection not available, cancel sync data...");
                            });
                        }
                        else
                        {
                            TxtStatus.Text = ("Internet connection not available, cancel sync data...");
                        }

                    }
                    else
                    {
                        await SyncToCloud();
                    }
                }
                finally
                {
                    PushTimer.Start();
                }
            };
            PushTimer.Start();
            TxtInfo.AppendText("\nAuto sync is enabled.");
            ChkPushToCloud.Click += async(a, b) => {
                if (PushImageTimer == null)
                {
                    PushImageTimer = new();
                    PushImageTimer.Interval = AppConstants.ImageSyncDelay;
                    TxtInfo.Text += $"Image Sync Time: {(float)AppConstants.ImageSyncDelay / 1000} seconds";
                    PushImageTimer.Elapsed += async (a, b) =>
                    {
                        try
                        {
                            PushImageTimer.Stop();
                            if (!CheckInternetConnection())
                            {
                                if (InvokeRequired)
                                {
                                    this.BeginInvoke((MethodInvoker)async delegate ()
                                    {
                                        TxtStatus.Text = ("Internet connection not available, cancel sync image data...");
                                    });
                                }
                                else
                                {
                                    TxtStatus.Text = ("Internet connection not available, cancel sync image data...");
                                }

                            }
                            else
                            {
                                if (ChkPushToCloud.Checked && CloudImage != null)
                                {
                                    await PushImageToCloud("cctv-1", CloudImage);
                                    AddCloudImage(null);
                                }
                            }
                        }
                        finally
                        {
                            PushImageTimer.Start();
                        }
                    };
                }
                if (ChkPushToCloud.Checked)
                    PushImageTimer.Start();
                else
                    PushImageTimer.Stop();
            };
        }
        //ObjectDetectedArgs TempArgs;
        //Bitmap CaptureImage;
        Image RawImage;
        void Start()
        {
            if (!ListDevice.Any())
            {
                var ErrMsg = "Camera OAK tidak ditemukan, tidak bisa mulai.";
                TxtInfo.Text = ErrMsg;
                Console.WriteLine(ErrMsg);
                return;
            }
            source = new();
            BtnStart.Enabled = false;
            BtnStop.Enabled = true;
            Clear();
            
            var device = ListDevice.First();
            objdet = new();

            objdet.ObjectDetected += (_, o) =>
            {
                //TempArgs = o;
                RawImage = (Image)o.NewImage.Clone();
                o.NewImage.Dispose();
                //if(!ProcessWorker.IsBusy)
                //ProcessWorker.RunWorkerAsync();
                //if (TempArgs == null) return;
                //var o = TempArgs;
                //var tempImg = (Image)o.NewImage?.Clone();
                if (RawImage != null)
                {
                    if (InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)async delegate ()
                        {
                            //PicBox1.Image = o.NewImage;
                            TxtInfo.Clear();
                            foreach (var obj in o.DetectedObjects)
                            {
                                TxtInfo.Text += $"label: {obj.Label}, score: {obj.Score}, pos: ({obj.P1.X},{obj.P1.Y}) - ({obj.P2.X},{obj.P2.Y})\n";
                            }
                            DoTracking(o.DetectedObjects, RawImage, source.Token);
                        });
                    }
                    else
                    {
                        //PicBox1.Image = o.NewImage;
                        TxtInfo.Clear();
                        foreach (var obj in o.DetectedObjects)
                        {
                            TxtInfo.Text += $"label: {obj.Label}, score: {obj.Score}, pos: ({obj.P1.X},{obj.P1.Y}) - ({obj.P2.X},{obj.P2.Y})\n";
                        }
                        DoTracking(o.DetectedObjects, RawImage, source.Token);
                    }
                    //tempImg.Dispose();
                }
            };

            objdet.device = new();
            objdet.device.deviceId = device.deviceId;
            objdet.ConnectDevice();

            objdet.StartAnalysis();
            var task = Task.Run(() => UpdateStatLoop(source.Token));


        }
        bool CheckInternetConnection()
        {
            return (new Ping().Send("www.google.com.mx").Status == IPStatus.Success);

        }
        async Task SyncToCloud()
        {
            var resultStr = string.Empty;
            try
            {
                var table = tracker.GetLogTable();
                /*
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        var newItem = new DataCounter();
                        newItem.Objek = dr["Jenis"].ToString();
                        newItem.Tanggal = Convert.ToDateTime(dr["Waktu"]);
                        newItem.Aktivitas = "-";
                        newItem.Tags = "-";
                        newItem.FileUrl = "-";
                        newItem.Deskripsi = "-";
                        newItem.Lokasi = AppConstants.Lokasi;
                        newItem.CCTV = AppConstants.CCTVName;
                        var res = await dataCounterService.InsertData(newItem);
                    }
                    tracker.ClearLogTable();
                    resultStr = $"Sync succeed at {DateTime.Now}";
                }*/
                if (table != null && table.Count > 0)
                {
                    foreach (var dr in table)
                    {
                        var newItem = new DataCounter();
                        newItem.Objek = dr.Jenis;
                        newItem.Tanggal = dr.Waktu;
                        newItem.Aktivitas = "-";
                        newItem.Tags = "-";
                        newItem.FileUrl = "-";
                        newItem.Deskripsi = "-";
                        newItem.Lokasi = AppConstants.Lokasi;
                        newItem.CCTV = AppConstants.CCTVName;
                        var res = await dataCounterService.InsertData(newItem);
                    }
                    tracker.ClearLogTable();
                    resultStr = $"Sync succeed at {DateTime.Now}";
                }
                else
                {
                    resultStr = $"No data to sync at {DateTime.Now}";
                }
            }
            catch (Exception ex)
            {

                resultStr = $"Sync failed at {DateTime.Now}:{ex.ToString()}";
            }
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)async delegate ()
                {
                    TxtStatus.Text = resultStr;
                });
            }
            else
            {
                TxtStatus.Text = resultStr;
            }
        }

        public string? OpenFileDialogForm()
        {
            var openFileDialog1 = new OpenFileDialog()
            {
                FileName = "",
                Filter = "Video files (*.mp4)|*.mp4",
                Title = "Open video file"
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(openFileDialog1.FileName))
                    {
                        return openFileDialog1.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");

                }
            }
            return null;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (source != null)
                source.Cancel();
        }

        private void ProcessWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
          
        }

        private void ProcessWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //this.Invoke((MethodInvoker)delegate
            //{

            //    PicBox1.Image = CaptureImage;
            //});

        }

        private static readonly string[] filter = new string[] {
            "bicycle", "person"
        };

        void DoTracking(List<ObjectInfo> results, Image nextFrame, CancellationToken token)
        {
            Rectangle selectRect = new Rectangle();
            if (IsCapturing) return;
            IsCapturing = true;
            var frameHeight = nextFrame.Height;
            var frameWidth = nextFrame.Width;
            if (SelectionArea.Width > 0 && SelectionArea.Height > 0)
            {
                // cropping sesuai selection area
                var ratioX = (double)SelectionArea.X / ImgWidth;
                var ratioY = (double)SelectionArea.Y / ImgHeight;
                var ratioWidth = (double)SelectionArea.Width / ImgWidth;
                var ratioHeight = (double)SelectionArea.Height / ImgHeight;

                selectRect = new Rectangle((int)(ratioX * frameWidth), (int)(ratioY * frameHeight), (int)(ratioWidth * frameWidth), (int)(ratioHeight * frameHeight));
            }

            //var bmp = await yolo.Detect(resize.ToBitmap(), selectRect);

            var watch = new Stopwatch();
            watch.Start();
            var filtered = results.Where(x => filter.Contains(x.Label)).ToList();
            // draw tracker
            tracker.Process(filtered, selectRect);
            var bmp = DrawResults.Draw(filtered, (Bitmap)nextFrame, tracker);
            //CaptureImage = bmp;
            
            this.PicBox1?.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                PicBox1.Image = bmp;
            });
            if (ChkPushToCloud.Checked)
            {
                AddCloudImage(ImageToByte2(bmp));
            }
            watch.Stop();
            /*
            this.TxtStatus?.Invoke((MethodInvoker)delegate
            {
                TxtStatus.Text = $"FPS: {(1000f / watch.ElapsedMilliseconds).ToString("n0")}";
            });
            */

            if (token.IsCancellationRequested)
            {
                IsCapturing = false;
                return;
            }

            IsCapturing = false;
        }
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
