using BMC.Drivers.BasicGraphics;
using Iot.Device.Ws28xx.Esp32;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Collections;
using System.Device.Wifi;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace LedApp
{
    public class Program
    {  // Set the SSID & Password to your local Wifi network
        const string MYSSID = "bmc makerspace";
        const string MYPASSWORD = "123qweasd";
        const string MqttHost = "test.mosquitto.org";
        const string MqttClientId = "led-app";
        const string Topic = "/led/data";
        
        const int MATRIX_WIDTH = rows;
        const int MATRIX_HEIGHT = cols * 4;
        const int cols = 32;
        const int rows = 8;
        static LedMatrix matrix;
        static bool IsConnected = false;
        static MqttClient mqtt;
        public static void Main()
        {
            try
            {
                // Get the first WiFI Adapter
                WifiAdapter wifi = WifiAdapter.FindAllAdapters()[0];

                // Set up the AvailableNetworksChanged event to pick up when scan has completed
                wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;

                // Loop forever scanning every 30 seconds
                //while (true)
                //{
                    Debug.WriteLine("starting Wifi scan");
                    wifi.ScanAsync();

                    //Thread.Sleep(30000);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine("message:" + ex.Message);
                Debug.WriteLine("stack:" + ex.StackTrace);
            }
            if(IsConnected)
            {
                mqtt = new MqttClient(MqttHost);//, 8883, true, new X509Certificate(CertMosquitto), null, MqttSslProtocols.TLSv1_2);

                // register to message received
                mqtt.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                var ret = mqtt.Connect(MqttClientId, true);
                
                // subscribe to the topic "/home/temperature" with QoS 2
                mqtt.Subscribe(new string[] { Topic }, new MqttQoSLevel[] {  MqttQoSLevel.ExactlyOnce });

                // You can add some code here

               
                if (ret != MqttReasonCode.Success)
                {
                    Debug.WriteLine($"ERROR connecting: {ret}");
                    mqtt.Disconnect();
                    return;
                }

            }
            var PinLed = nanoFramework.Hardware.Esp32.Gpio.IO13;
            Debug.WriteLine("Matrix Testing !!");
            matrix = new LedMatrix(PinLed, MATRIX_HEIGHT, MATRIX_WIDTH);
            var pct = 0;
            while (true)
            {
                pct++;
                SetLevel(pct);
                if (pct >= 100) pct = 0;
                Thread.Sleep(500);
            }
            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == Topic)
            {
                nanoFramework.Json.JsonSerializer serial = new nanoFramework.Json.JsonSerializer();
                string json = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                var item = (DeviceData)nanoFramework.Json.JsonConvert.DeserializeObject(json,typeof(DeviceData));
                if(item!=null)
                    SetLevel(item.Progress);
            }
            // handle message received 
        }
        /// <summary>
        /// Event handler for when Wifi scan completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Wifi_AvailableNetworksChanged(WifiAdapter sender, object e)
        {
            Debug.WriteLine("Wifi_AvailableNetworksChanged - get report");

            // Get Report of all scanned Wifi networks
            WifiNetworkReport report = sender.NetworkReport;

            // Enumerate though networks looking for our network
            foreach (WifiAvailableNetwork net in report.AvailableNetworks)
            {
                // Show all networks found
                Debug.WriteLine($"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {net.SignalBars.ToString()}");

                // If its our Network then try to connect
                if (net.Ssid == MYSSID)
                {
                    // Disconnect in case we are already connected
                    sender.Disconnect();

                    // Connect to network
                    WifiConnectionResult result = sender.Connect(net, WifiReconnectionKind.Automatic, MYPASSWORD);

                    // Display status
                    if (result.ConnectionStatus == WifiConnectionStatus.Success)
                    {
                        Debug.WriteLine("Connected to Wifi network");
                        IsConnected = true;
                    }
                    else
                    {
                        IsConnected = false;
                        Debug.WriteLine($"Error {result.ConnectionStatus.ToString()} connecting o Wifi network");
                    }
                }
            }
        }
        public static uint ColorToUint(System.Drawing.Color c)
        {
            uint u = (UInt32)c.A << 24;
            u += (UInt32)c.R << 16;
            u += (UInt32)c.G << 8;
            u += c.B;
            return u;

            // (UInt32)((UInt32)c.A << 24 + (UInt32)c.R << 16 + (UInt32)c.G << 8 + (UInt32)c.B);
        }

        static void SetLevel(float Percent)
        {
            matrix.Clear();
            var color = ColorToUint( System.Drawing.Color.Blue);
            //if (Percent > 1) Percent = Percent / 100;
            Percent = Percent / 100;
            var onePercent = MATRIX_HEIGHT / 100;
            var TargetHeight = (int)(Percent * onePercent);
            for (int x = 0; x < TargetHeight; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    matrix.SetPixel(x, y, color);
                }
                //Thread.Sleep(10);
            }
            matrix.Flush();
        }
    }
    public class DeviceData
    {
        public DateTime Tanggal { get; set; }
        public float Progress { get; set; }
    }
    public class LedMatrix : BasicGraphics
    {
        private int row, column;
        Ws28xx leds;
        BitmapImage img;

        public LedMatrix(int pin, int column, int row)
        {
            this.row = row;
            this.column = column;
            leds = new Ws2808(pin, column, row);
            img = leds.Image;
            //this.leds = new WS2812Controller(pin, this.row * this.column, WS2812Controller.DataFormat.rgb565);

            Clear();
        }

        public override void Clear()
        {
            img.Clear();
            //leds.Clear();
        }

        public override void SetPixel(int x, int y, uint color)
        {
            if (x < 0 || x >= this.column) return;
            if (y < 0 || y >= this.row) return;

            // even columns are inverted
            //if ((x & 0x01) != 0)
            //{
            //    y = (int)(this.row - 1 - y);
            //}

            //var index = x * this.row + y;
            var col = System.Drawing.Color.FromArgb(255, (byte)(color >> 16), (byte)(color >> 8), (byte)(color >> 0));
            img.SetPixel(x,y, col);
            //leds.Image.SetPixel(x, y, );
        }
        public void Flush()
        {
            leds.Update();
        }
    }
}
