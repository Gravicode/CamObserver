using BMC.Drivers.BasicGraphics;
using Iot.Device.Ws28xx.Esp32;
using nanoFramework.Hardware.Esp32;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Collections;
using System.Device.Wifi;
using System.Diagnostics;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace LedApp
{
    public class Program
    {  // Set the SSID & Password to your local Wifi network
        const string MYSSID = "WholeOffice";
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
            var sensorCuaca = new WeatherSensor();
            sensorCuaca.StartSensing();
            /*
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
            }*/
            Thread.Sleep(Timeout.Infinite);
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

        public float Target { get; set; }
        public float Current { get; set; }
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
    public class SensorData
    {
        public double WindSpeedAverage { get; set; }
        public double WindDirection { get; set; }
        public double WindSpeedMax { get; set; }
        public double RainfallOneHour { get; set; }
        public double RainfallOneDay { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double BarPressure { get; set; }
    }

    public class WeatherSensor
    {
        SensorData current;
        Thread th1;
        SerialPort uart;
        public WeatherSensor(string ComPort="COM2", int PinRx = 32, int PinTx = 33)
        { 
            // COM2 in ESP32-WROVER-KIT mapped to free GPIO pins
            // mind to NOT USE pins shared with other devices, like serial flash and PSRAM
            // also it's MANDATORY to set pin function to the appropriate COM before instantiating it

            Configuration.SetPinFunction(PinRx, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(PinTx, DeviceFunction.COM2_TX);

            // open COM2
            uart = new SerialPort(ComPort);
            // set parameters
            uart.BaudRate = 9600;
            uart.Parity = Parity.None;
            uart.StopBits = StopBits.One;
            uart.Handshake = Handshake.None;
            uart.DataBits = 8;

            // if dealing with massive data input, increase the buffer size
            uart.ReadBufferSize = 2048;

            // open the serial port with the above settings
            uart.Open();
          
        }

        public SensorData GetCurrentData()
        {
            return current;
        }

        public void StartSensing()
        {
            if (th1 != null)
            {
                th1 = new Thread(new ThreadStart(Loop));
                th1.Start();
            }
        }

        void Loop()
        {
            while (true)
            {
                getBuffer();
                //lora
                current = new SensorData()
                {
                    WindDirection = WindDirection(),
                    WindSpeedMax = WindSpeedMax(),
                    BarPressure = BarPressure(),
                    Humidity = Humidity(),
                    RainfallOneDay = RainfallOneDay(),
                    RainfallOneHour = RainfallOneHour(),
                    Temperature = Temperature()
                    ,
                    WindSpeedAverage = WindSpeedAverage()


                };//Begin!
                Debug.WriteLine("Wind Direction: " + current.WindDirection);
                Debug.WriteLine("Average Wind Speed (One Minute): " + current.WindSpeedAverage + "m/s  ");
                Debug.WriteLine("Max Wind Speed (Five Minutes): " + current.WindSpeedMax + "m/s");
                Debug.WriteLine("Rain Fall (One Hour): " + current.RainfallOneHour + "mm  ");
                Debug.WriteLine("Rain Fall (24 Hour): " + current.RainfallOneDay + "mm");
                Debug.WriteLine("Temperature: " + current.Temperature + "C  ");
                Debug.WriteLine("Humidity: " + current.Humidity + "%  ");
                Debug.WriteLine("Barometric Pressure: " + current.BarPressure + "hPa");
                Debug.WriteLine("----------------------");
                Thread.Sleep(1000);
            }

            //var jsonStr = Json.NETMF.JsonSerializer.SerializeObject(data);
            //Debug.WriteLine("kirim :" + jsonStr);
            //sendData(jsonStr);
        }

        double temp;
        byte[] databuffer = new byte[35];
            void getBuffer()                                                                    //Get weather status data
            {
                int index;
                for (index = 0; index < 35; index++)
                {
                    if (uart.BytesToRead > 0)
                    {
                        databuffer[index] = (byte)uart.ReadByte();
                        if (databuffer[0] != 'c')
                        {
                            index = -1;
                        }
                    }
                    else
                    {
                        index--;
                    }
                }
            }
            int transCharToInt(byte[] _buffer, int _start, int _stop)                               //char to int）
        {
            int _index;
            int result = 0;
            int num = _stop - _start + 1;
            var _temp = new int[num];
            for (_index = _start; _index <= _stop; _index++)
            {
                _temp[_index - _start] = _buffer[_index] - '0';
                result = 10 * result + _temp[_index - _start];
            }
            return result;
        }

        int WindDirection()                                                                  //Wind Direction
        {
            return transCharToInt(databuffer, 1, 3);
        }

        double WindSpeedAverage()                                                             //air Speed (1 minute)
        {
            temp = 0.44704 * transCharToInt(databuffer, 5, 7);
            return temp;
        }

        double WindSpeedMax()                                                                 //Max air speed (5 minutes)
        {
            temp = 0.44704 * transCharToInt(databuffer, 9, 11);
            return temp;
        }

        double Temperature()                                                                  //Temperature ("C")
        {
            temp = (transCharToInt(databuffer, 13, 15) - 32.00) * 5.00 / 9.00;
            return temp;
        }

        double RainfallOneHour()                                                              //Rainfall (1 hour)
        {
            temp = transCharToInt(databuffer, 17, 19) * 25.40 * 0.01;
            return temp;
        }

        double RainfallOneDay()                                                               //Rainfall (24 hours)
        {
            temp = transCharToInt(databuffer, 21, 23) * 25.40 * 0.01;
            return temp;
        }

        int Humidity()                                                                       //Humidity
        {
            return transCharToInt(databuffer, 25, 26);
        }

        double BarPressure()                                                                  //Barometric Pressure
        {
            temp = transCharToInt(databuffer, 28, 32);
            return temp / 10.00;
        }

    }


}
