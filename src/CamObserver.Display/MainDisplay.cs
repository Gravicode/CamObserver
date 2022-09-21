using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamObserver.Display.Pages;
using DepthAI.Core;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
namespace CamObserver.Display
{
    public partial class MainDisplay : Form
    {
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

        void Clear()
        {
            PicBox1.Image = null;

            TxtInfo.Clear();
        }
        void Setup()
        {

            var manager = new OAKDeviceManager();
            var list = manager.GetAvailableDevices();

            //stream = new();
            BtnStart.Click += (a, b) =>
            {
                Clear();
                var device = list.First();
                objdet = new();

                objdet.ObjectDetected += (_, o) =>
                {
                    if (InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            PicBox1.Image = o.NewImage;
                            TxtInfo.Clear();
                            foreach (var obj in o.DetectedObjects)
                            {
                                TxtInfo.Text += $"label: {obj.Label}, score: {obj.Score}, pos: ({obj.P1.X},{obj.P1.Y}) - ({obj.P2.X},{obj.P2.Y})\n";
                            }

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
                /*
                    face.FinishDevice();
                    face.Dispose();
                  */

                objdet.FinishDevice();
                objdet.Dispose();



            };
        }
    }
}
