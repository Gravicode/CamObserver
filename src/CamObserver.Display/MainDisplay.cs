using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamObserver.Display.Data;
using CamObserver.Display.Helpers;
using CamObserver.Display.Pages;
using CamObserver.Models;
using CamObserver.Tools;
using DepthAI.Core;
using GemBox.Document;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
namespace CamObserver.Display
{
    public partial class MainDisplay : Form
    {
        int ImgHeight = 416, ImgWidth = 416;
        Point StartLocation;
        bool IsSelect = false;
        Rectangle SelectionArea = new Rectangle(0, 0, 0, 0);
        DataCounterService dataCounterService;
        bool IsCapturing = false;
        string SelectedFile;
        CancellationTokenSource source;

        Tracker tracker;

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
        }



        //DaiFaceDetector face;
        //DaiStreams stream;
        DaiObjectDetector objdet;
        //DaiBodyPose pose;


        record analysistype(string name, string value);

        private void BtnStart_Click(object sender, EventArgs e)
        {

        }

        void UpdateStatLoop(CancellationToken token)
        {
            while (true)
            {
                AppConstants.InfoStat.Clear();
                var row = 0;
                var dt = tracker.GetLogTable();
                foreach (var fil in filter)
                {
                    var count = dt.AsEnumerable().Where(x => x.Field<string>("Jenis") == "person").Count();
                    AppConstants.InfoStat.Add(new ObjekStatistik() { Jumlah = count, No = row++, Objek = fil });
                }
                if (token.IsCancellationRequested) break;
                Thread.Sleep(1000);
            }
        }

        void Clear()
        {
            PicBox1.Image = null;

            TxtInfo.Clear();
        }
        void Setup()
        {

            var manager = new OAKDeviceManager();
            var list = manager.GetAvailableDevices();
            source = new();
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
                BtnStart.Enabled = false;
                BtnStop.Enabled = true;
                Clear();
                var device = list.First();
                objdet = new();

                objdet.ObjectDetected += (_, o) =>
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
                            DoTracking(o.DetectedObjects, o.NewImage, source.Token);
                        });
                    }
                    else
                    {
                        PicBox1.Image = o.NewImage;
                        TxtInfo.Clear();
                        foreach (var obj in o.DetectedObjects)
                        {
                            TxtInfo.Text += $"label: {obj.Label}, score: {obj.Score}, pos: ({obj.P1.X},{obj.P1.Y}) - ({obj.P2.X},{obj.P2.Y})\n";
                        }
                    }
                };

                objdet.device = new();
                objdet.device.deviceId = device.deviceId;
                objdet.ConnectDevice();

                objdet.StartAnalysis();
                var task = Task.Run(() => UpdateStatLoop(source.Token));
                /*
                face = new DaiFaceDetector();

                face.FaceDetected += (_, o) => {
                    if (InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            PicBox1.Image = o.NewImage;
                            TxtInfo.Clear();
                            TxtInfo.Text = $"center X: {o.valueCenterX}\ncenter Y: {o.valueCenterY}";
                        });
                    }
                    else
                    {
                        PicBox1.Image = o.NewImage;
                        TxtInfo.Clear();
                        TxtInfo.Text = $"center X: {o.valueCenterX}\ncenter Y: {o.valueCenterY}";
                    }
                };

                face.device = new();
                face.device.deviceId = device.deviceId;
                face.ConnectDevice();

                face.StartAnalysis();
               */

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



            };
        }
        async Task SyncToCloud()
        {
            try
            {
                var table = tracker.GetLogTable();
                if (table != null)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        var newItem = new DataCounter();
                        newItem.Objek = dr["Jenis"].ToString();
                        newItem.Tanggal = Convert.ToDateTime(dr["Waktu"]);
                        newItem.Aktivitas = "-";
                        newItem.Lokasi = AppConstants.Lokasi;
                        newItem.CCTV = AppConstants.CCTVName ;
                        var res = await dataCounterService.InsertData(newItem);
                    }
                }
                Console.WriteLine("Sync succeed");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Sync failed:{ex.ToString()}");
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

        private static readonly string[] filter = new string[] {
            "bicycle", "person"
        };

        async void DoTracking(List<ObjectInfo> results, Image nextFrame, CancellationToken token)
        {
            Rectangle selectRect = new Rectangle();
            if (IsCapturing) return;
            IsCapturing = true;

            if (SelectionArea.Width > 0 && SelectionArea.Height > 0)
            {
                // cropping sesuai selection area
                var ratioX = (double)SelectionArea.X / ImgWidth;
                var ratioY = (double)SelectionArea.Y / ImgHeight;
                var ratioWidth = (double)SelectionArea.Width / ImgWidth;
                var ratioHeight = (double)SelectionArea.Height / ImgHeight;

                selectRect = new Rectangle((int)(ratioX * nextFrame.Width), (int)(ratioY * nextFrame.Height), (int)(ratioWidth * nextFrame.Width), (int)(ratioHeight * nextFrame.Height));
            }

            //var bmp = await yolo.Detect(resize.ToBitmap(), selectRect);

            var watch = new Stopwatch();
            watch.Start();
            var filtered = results.Where(x => filter.Contains(x.Label)).ToList();
            // draw tracker
            tracker.Process(filtered, selectRect);
            var bmp = DrawResults.Draw(filtered, (Bitmap)nextFrame, tracker);

            this.PicBox1?.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                PicBox1.Image = bmp;
            });

            //this.PicBox1?.Invoke((MethodInvoker)delegate
            //{
            //    // Running on the UI thread
            //    PicBox1.Image = resize.ToBitmap();
            //});

            watch.Stop();

            this.TxtStatus?.Invoke((MethodInvoker)delegate
            {
                TxtStatus.Text = $"FPS: {(1000f / watch.ElapsedMilliseconds).ToString("n0")}";
            });


            if (token.IsCancellationRequested)
            {
                return;
            }

            IsCapturing = false;
        }
    }
}
