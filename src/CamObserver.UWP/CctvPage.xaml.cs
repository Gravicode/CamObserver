using CamObserver.Models;
using CamObserver.Tools;
using CamObserver.UWP.Helpers;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AI.Skills.Vision.ObjectDetector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Rectangle = Windows.UI.Xaml.Shapes.Rectangle;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CamObserver.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CctvPage : Page, IDisposable
    {
        public enum StreamSourceTypes { WebCam, RTSP, HttpImage }
        StreamSourceTypes Mode = StreamSourceTypes.WebCam;

        PointF StartLocation;
        bool IsSelect = false;
        private MediaCapture _media_capture;
        private Model5 _model;
        private DispatcherTimer _timer;
        private DispatcherTimer PushTimer;
        private DispatcherTimer StatTimer;
        private readonly SolidColorBrush _fill_brush = new SolidColorBrush(Colors.Transparent);
        private readonly SolidColorBrush _line_brush = new SolidColorBrush(Colors.DarkGreen);
        private readonly double _line_thickness = 2.0;
        ImageHttpHelper Grabber;
        int DelayTime = 30;
        List<System.Drawing.PointF> SelectionArea = new List<PointF>();
        VideoFrame frame;
        bool IsTracking = false;
        //RtspHelper rtsp;
        private bool processing;
        private Stopwatch watch;
        private int count;
        double ImgWidth, ImgHeight;
        DataCounterService dataCounterService;
        List<System.Drawing.PointF> selectPoly = new List<PointF>();
        List<System.Drawing.PointF> SelectionAreaPoly= new List<PointF> ();
        List<System.Drawing.PointF> TempSelectionAreaPoly= new List<PointF>();
        public CctvPage()
        {
            this.InitializeComponent();

            LoadConfig();
            SetupGrpc();
            SetupComponents();
        }

        void SetupGrpc()
        {
            var channel = GrpcChannel.ForAddress(
              AppConstants.GrpcUrl, new GrpcChannelOptions
              {
                  MaxReceiveMessageSize = 8 * 1024 * 1024, // 5 MB
                  MaxSendMessageSize = 8 * 1024 * 1024, // 2 MB                
                  HttpHandler = new GrpcWebHandler(new HttpClientHandler())
              });
            ObjectContainer.Register<GrpcChannel>(channel);
            ObjectContainer.Register<DataCounterService>(new DataCounterService(channel));
            ObjectContainer.Register<CCTVService>(new CCTVService(channel));
            ObjectContainer.Register<GatewayService>(new GatewayService(channel));
        }
        void LoadConfig()
        {
            AppConfig.Load();
            if (!string.IsNullOrEmpty(AppConstants.SelectionArea))
            {
                SelectionArea = JsonSerializer.Deserialize<List< System.Drawing.PointF>>(AppConstants.SelectionArea);
                RefreshSelection();
            }

        }
        void SetupComponents()
        {
            button_go.IsEnabled = false;
            this.Loaded += OnPageLoaded;
            this.Unloaded += (a, b) => { AppConfig.Save(); };
            BtnClearArea.Click += (a, b) => {
                TempSelectionAreaPoly.Clear();
                SelectionAreaPoly.Clear();
                selectPoly.Clear();
                RefreshSelection();
            };
            OverlayCanvas.PointerReleased += (s, e) =>
            {
                
                PointerPoint ptrPt = e.GetCurrentPoint(this.OverlayCanvas);
               
                if (ChkSelectArea.IsChecked.HasValue && ChkSelectArea.IsChecked.Value)
                {
                    if (ptrPt.Properties.PointerUpdateKind==PointerUpdateKind.LeftButtonReleased)
                    {
                        var pos = e.GetCurrentPoint(this.OverlayCanvas).Position;
                        TempSelectionAreaPoly.Add(new System.Drawing.PointF((float)pos.X, (float)pos.Y));
                        
                            RefreshSelection();
                        
                    }else if (ptrPt.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                    {
                        if (TempSelectionAreaPoly.Count > 2)
                        {
                            SelectionAreaPoly = TempSelectionAreaPoly.Select(x=>x).ToList();
                            TempSelectionAreaPoly.Clear();
                            ChkSelectArea.IsChecked = false;
                            RefreshSelection();
                            //create selection area
                            selectPoly.Clear();

                            foreach (var pointArea in SelectionAreaPoly)
                            {
                                //cropping sesuai selection area
                                var ratioX = (double)pointArea.X / ImgWidth;
                                var ratioY = (double)pointArea.Y / ImgHeight;
                                //var ratioWidth = (double)SelectionArea.Width / ImgWidth;
                                //var ratioHeight = (double)SelectionArea.Height / ImgHeight;
                                selectPoly.Add(new PointF((float)(ratioX * ImgWidth), (float)(ratioY * ImgHeight)));

                            }

                            
                        }
                    }
                }
                
                
            };
            dataCounterService = ObjectContainer.Get<DataCounterService>();
            if (PushTimer == null)
            {
                // set per menit
                PushTimer = new DispatcherTimer()
                {

                    Interval = TimeSpan.FromMilliseconds(60 * 1000)
                };
                PushTimer.Tick += PushTimerTick;
                PushTimer.Start();

            }
            if (StatTimer == null)
            {
                // set per menit
                StatTimer = new DispatcherTimer()
                {

                    Interval = TimeSpan.FromMilliseconds(2000)
                };
                StatTimer.Tick += StatTimerTick;
                StatTimer.Start();

            }
            ChkAutoStart.IsChecked = AppConstants.AutoStart;
        }
        private void StatTimerTick(object sender, object e)
        {
            if (_model != null)
            {
                var tracker = _model.GetTracker();
                var stat = tracker.GetStatTable();
                Grid1.ItemsSource = stat;
            }
        }
        public System.Collections.ObjectModel.ObservableCollection<dynamic> ToDynamic(DataTable dt)
        {
            var dynamicDt = new System.Collections.ObjectModel.ObservableCollection<dynamic>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                dynamic dyn = new System.Dynamic.ExpandoObject();
                dynamicDt.Add(dyn);
                //Converting the DataTable collcetion in Dynamic collection    
                foreach (System.Data.DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }


        private async void PushTimerTick(object sender, object e)
        {
            if (ChkAutoPush.IsChecked.Value)
                await SyncToCloud();
        }

        void RefreshSelection()
        {
            OverlayCanvas2.Children.Clear();
            if (TempSelectionAreaPoly.Count > 1) 
            {
                for (int i = 0; i < TempSelectionAreaPoly.Count - 1; i++)
                {
                    var p1 = TempSelectionAreaPoly[i];
                    var p2 = TempSelectionAreaPoly[i + 1];
                    var line = new Line()
                    {
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 2
                    };
                    OverlayCanvas2.Children.Add(line);
                }
            }else if (TempSelectionAreaPoly.Count==1)
            {
                var ellipse1 = new Ellipse();
                ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                ellipse1.Width = 5;
                ellipse1.Height = 5;
                Canvas.SetLeft(ellipse1, TempSelectionAreaPoly.First().X);
                Canvas.SetTop(ellipse1, TempSelectionAreaPoly.First().Y);
                OverlayCanvas2.Children.Add(ellipse1);
            }
            if (SelectionAreaPoly.Count > 1)
            {
                var poly = new Polygon();
                poly.Points = new PointCollection();
                SelectionAreaPoly.ForEach(x => poly.Points.Add(new Windows.Foundation.Point(x.X, x.Y)));

                poly.Fill = new SolidColorBrush(Windows.UI.Colors.LightSkyBlue);
                poly.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkBlue);
                poly.Opacity = 0.3;
                poly.StrokeThickness = this._line_thickness;
                //Canvas.SetLeft(selection, SelectionArea.X);
                //Canvas.SetTop(selection, SelectionArea.Y);
                this.OverlayCanvas2.Children.Add(poly);
            }
            /*
            if (selection == null)
            {
                OverlayCanvas2.Children.Clear();
                selection = new Rectangle();
                selection.Width = SelectionArea.Width;
                selection.Height = SelectionArea.Height;
                selection.Fill = new SolidColorBrush(Windows.UI.Colors.LightSkyBlue);
                selection.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkBlue);
                selection.Opacity = 0.3;
                selection.StrokeThickness = this._line_thickness;
                Canvas.SetLeft(selection, SelectionArea.X);
                Canvas.SetTop(selection, SelectionArea.Y);
                this.OverlayCanvas2.Children.Add(selection);
            }
            else
            if (SelectionArea.Width > 0 && SelectionArea.Height > 0)
            {
                selection.Width = SelectionArea.Width;
                selection.Height = SelectionArea.Height;
                Canvas.SetLeft(selection, SelectionArea.X);
                Canvas.SetTop(selection, SelectionArea.Y);

            }*/
            AppConstants.SelectionArea = JsonSerializer.Serialize(SelectionArea);
        }

        async Task<VideoFrame> Capture(UIElement element)
        {
            // Render to an image at the current system scale and retrieve pixel contents
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            SoftwareBitmap outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(pixelBuffer, BitmapPixelFormat.Bgra8, renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight, BitmapAlphaMode.Ignore);
            var current = VideoFrame.CreateWithSoftwareBitmap(outputBitmap);
            return current;
        }
        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {

            await InitModelAsync(DeviceComboBox.SelectedIndex);
            VlcPlayer.Visibility = Visibility.Collapsed;
            WebCam.Visibility = Visibility.Collapsed;
            switch (Mode)
            {
                case StreamSourceTypes.RTSP:
                    VlcPlayer.Visibility = Visibility.Visible;
                    //var rtsp = new RtspHelper();
                    //await rtsp.StartStream();
                    //
                    //rtsp.FrameReceived += (a, ev) => {
                    //var bmp = SoftwareBitmap.CreateCopyFromBuffer(ev.BitmapFrame.PixelBuffer, BitmapPixelFormat.Bgra8, ev.BitmapFrame.PixelWidth, ev.BitmapFrame.PixelHeight);
                    //frame = VideoFrame.CreateWithSoftwareBitmap(bmp);
                    //};
                    //VlcPlayer.Source = AppConstants.Cctv1;
                    //VlcPlayer.Play();
                    
                    string FILE_TOKEN = "{1BBC4B94-BE33-4D79-A0CB-E5C6CDB9D107}";
                    var fileOpenPicker = new FileOpenPicker();
                    fileOpenPicker.FileTypeFilter.Add("*");
                    var file = await fileOpenPicker.PickSingleFileAsync();
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(FILE_TOKEN, file);
                    VlcPlayer.Source = $"winrt://{FILE_TOKEN}";
                    VlcPlayer.Play();
                    break;
                case StreamSourceTypes.WebCam:
                    WebCam.Visibility = Visibility.Visible;
                    _ = InitCameraAsync();
                    break;
                case StreamSourceTypes.HttpImage:

                    //for cctv image grabber through http
                    this.Grabber = new ImageHttpHelper(AppConstants.Username, AppConstants.Password);
                    break;
            }

            if (AppConstants.AutoStart)
                await StartTracking();
        }
        private async Task InitCameraAsync()
        {
            if (_media_capture == null || _media_capture.CameraStreamState == Windows.Media.Devices.CameraStreamState.Shutdown || _media_capture.CameraStreamState == Windows.Media.Devices.CameraStreamState.NotStreaming)
            {
                if (_media_capture != null)
                {
                    _media_capture.Dispose();
                }

                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
                var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                var camera = cameras.FirstOrDefault();
                settings.VideoDeviceId = camera.Id;

                _media_capture = new MediaCapture();
                await _media_capture.InitializeAsync(settings);
                WebCam.Source = _media_capture;
            }

            if (_media_capture.CameraStreamState == Windows.Media.Devices.CameraStreamState.NotStreaming)
            {
                await _media_capture.StartPreviewAsync();
                WebCam.Visibility = Visibility.Visible;
            }
        }


        private async Task InitModelAsync(int DevId = 0)
        {
            ShowStatus("Loading yolo.onnx model...");
            try
            {
                #region model1
                if (_model == null)
                {

                    _model = new Model5();

                    ImgWidth = OverlayCanvas.Width;
                    ImgHeight = OverlayCanvas.Height;
                }
                await _model.InitModelAsync(ImgWidth, ImgHeight, DevId);

                //set filter objects
                _model.SetFilter( ObjectKind.Person, ObjectKind.Bicycle);

                #endregion
                ShowStatus("ready");
                Log("Model is loaded..");
                button_go.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message);
            }
        }
        private async Task ProcessFrame()
        {
            if (processing)
            {
                // if we can't keep up to 30 fps, then ignore this tick.
                return;
            }
            try
            {
                if (watch == null)
                {
                    watch = new Stopwatch();
                    watch.Start();
                }

                processing = true;

                
                switch (Mode)
                {
                    case StreamSourceTypes.RTSP:
                        //VlcPlayer.Play();
                        frame = await Capture(VlcPlayer);
                        break;
                    case StreamSourceTypes.WebCam:
                        frame = new VideoFrame(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, (int)ImgWidth, (int)ImgHeight);
                        await _media_capture.GetPreviewFrameAsync(frame);
                        break;
                    case StreamSourceTypes.HttpImage:
                        frame = await Grabber.GetFrameFromHttp(AppConstants.CctvHttp);
                        break;
                }

                if (frame != null)
                {
                    var results = await _model.EvaluateFrame(frame, selectPoly);
                    Log($"detected objects : {results.Count}");
                    await DrawBoxes5(results, frame, _model.GetTracker());
                    count++;
                    if (watch.ElapsedMilliseconds > 1000)
                    {
                        ShowStatus(string.Format("{0} fps", count));
                        count = 0;
                        watch.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                processing = false;
            }
        }

        private async Task DrawBoxes5(List<ObjectDetectorResult> detections, VideoFrame frame, Helpers.Tracker tracker)
        {
            this.OverlayCanvas.Children.Clear();
            var brush = new ImageBrush();
            var bitmap_source = new SoftwareBitmapSource();
            await bitmap_source.SetBitmapAsync(frame.SoftwareBitmap);

            brush.ImageSource = bitmap_source;
            brush.Stretch = Stretch.Fill;
            this.OverlayCanvas.Background = brush;

            for (int i = 0; i < detections.Count; ++i)
            {
                int top = (int)(detections[i].Rect.Top * ImgHeight);
                int left = (int)(detections[i].Rect.Left * ImgWidth);
                int bottom = (int)(detections[i].Rect.Bottom * ImgHeight);
                int right = (int)(detections[i].Rect.Right * ImgWidth);



                var r = new Rectangle();
                r.Tag = i;
                r.Width = right - left;
                r.Height = bottom - top;
                r.Fill = this._fill_brush;
                r.Stroke = this._line_brush;
                r.StrokeThickness = this._line_thickness;
                r.Margin = new Thickness(left, top, 0, 0);

                this.OverlayCanvas.Children.Add(r);
                // Default configuration for border
                // Render text label


                var border = new Border();
                var backgroundColorBrush = new SolidColorBrush(Colors.Black);
                var foregroundColorBrush = new SolidColorBrush(Colors.SpringGreen);
                var textBlock = new TextBlock();
                textBlock.Foreground = foregroundColorBrush;
                textBlock.FontSize = 18;

                textBlock.Text = detections[i].Kind.ToString();
                // Hide
                textBlock.Visibility = Visibility.Collapsed;
                border.Background = backgroundColorBrush;
                border.Child = textBlock;

                Canvas.SetLeft(border, detections[i].Rect.Left * ImgWidth);
                Canvas.SetTop(border, detections[i].Rect.Top * ImgHeight);
                textBlock.Visibility = Visibility.Visible;
                // Add to canvas
                this.OverlayCanvas.Children.Add(border);
            }

            if (tracker.TrackedList != null)
            {
                foreach (var target in tracker.TrackedList)
                {
                    var p1 = new PointF(target.Location.X, target.Location.Y);
                    for (int i = target.Trails.Count - 1; i > 0; i--)
                    {
                        var p2f = target.Trails[i];
                        var p2 = new PointF(p2f.X, p2f.Y);
                        var line = new Line();
                        line.X1 = p1.X;
                        line.Y1 = p1.Y;
                        line.X2 = p2.X;
                        line.Y2 = p2.Y;
                        var fill = new SolidColorBrush(Windows.UI.Color.FromArgb(180, target.Col.R, target.Col.G, target.Col.B));
                        line.Fill = fill;
                        line.Stroke = fill;
                        line.StrokeThickness = this._line_thickness;
                        OverlayCanvas.Children.Add(line);
                        p1 = p2;
                    }
                }
            }
        }
        private void ChkAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            AppConstants.AutoStart = true;
        }

        private void ChkAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            AppConstants.AutoStart = false;
        }
        private async void button_go_Click(object sender, RoutedEventArgs e)
        {
            await StartTracking();
        }

        async Task StartTracking()
        {
            if (!IsTracking)
            {
                var deviceIndex = DeviceComboBox.SelectedIndex;
                if (_model.DeviceIndex != deviceIndex)
                {
                    await InitModelAsync(deviceIndex);
                }


                if (_timer == null)
                {
                    // now start processing frames, no need to do more than 30 per second!
                    _timer = new DispatcherTimer()
                    {

                        Interval = TimeSpan.FromMilliseconds(DelayTime)
                    };
                    _timer.Tick += OnTimerTick;

                }
                _timer.Start();
                IsTracking = true;
                Log("Tracking is enabled..");
                /*
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
                //picker.FileTypeFilter.Add(".mp4");
                //picker.FileTypeFilter.Add(".avi");
                //picker.FileTypeFilter.Add(".mov");
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");

                currentFile = await picker.PickSingleFileAsync();
                if (currentFile != null)
                {
                    // Application now has read/write access to the picked file
                    //this.textBlock.Text = "Picked photo: " + file.Name;
                    AppConstants.Cctv1 = currentFile.Path;
                    var stream = await currentFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    var image = new BitmapImage();
                    image.SetSource(stream);

                    ImageEncodingProperties properties = ImageEncodingProperties.CreateJpeg();

                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var outputBitmap = await decoder.GetSoftwareBitmapAsync();

                    if (outputBitmap != null)
                    {

                        //SoftwareBitmap outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(data.AsBuffer(), BitmapPixelFormat.Bgra8, bmp.PixelWidth, bmp.PixelHeight, BitmapAlphaMode.Premultiplied);
                        var currentImage = SoftwareBitmap.Convert(outputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                        frame = VideoFrame.CreateWithSoftwareBitmap(currentImage);


                    }
                }
                else
                {
                    //this.textBlock.Text = "Operation cancelled.";
                }
                */

            }
            else
            {
                if (_timer != null)
                {
                    _timer.Stop();
                }
                IsTracking = false;
                Log("Tracking is disabled..");

            }
        }

        void ShowStatus(string text)
        {
            textblock_status.Text = text;
        }

        private async void OnTimerTick(object sender, object e)
        {
            // don't wait for this async task to finish

            _ = ProcessFrame();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Save();
            Log("Config is saved.");
        }

        void Log(string Message)
        {
            TxtStatus.Text = $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} => {Message}";
        }

        async Task SyncToCloud()
        {
            try
            {
                var tracker = _model.GetTracker();
                var table = tracker.GetLogTable();
                if (table != null)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        var newItem = new DataCounter();
                        newItem.Objek = dr["Objek"].ToString();
                        newItem.Tanggal = Convert.ToDateTime(dr["Waktu"]);
                        newItem.Deskripsi = "-";
                        newItem.CCTV = AppConstants.CCTV;
                        newItem.Lokasi = AppConstants.Lokasi;
                        var res = await dataCounterService.InsertData(newItem);
                    }
                }
                tracker.ClearLogTable();
                Console.WriteLine("Sync succeed");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Sync failed:{ex.ToString()}");
            }

        }
    

        public void Dispose()
        {
            _media_capture?.Dispose();
            _timer.Stop();
            PushTimer.Stop();
            StatTimer.Stop();
            frame?.Dispose();
            if (watch.IsRunning)
            {
                watch.Stop();
            }
            VlcPlayer.Stop();
        }
    }
}
