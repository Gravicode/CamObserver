using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CamObserver.Device
{
    public class CounterData
    {
        public long Person { get; set; }
        public long Bicycle { get; set; }

        public string Message { get; set; }
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
        UartController uart;
        public WeatherSensor(string ComPort = SC13048.UartPort.Uart2)
        {
            uart = UartController.FromName(ComPort);

            var uartSetting = new UartSetting()
            {
                BaudRate = 9600,
                DataBits = 8,
                Parity = UartParity.None,
                StopBits = UartStopBitCount.One,
                Handshaking = UartHandshake.None,
            };

            uart.SetActiveSettings(uartSetting);

            uart.Enable();
            
            uart.DataReceived += Uart_DataReceived;

            // if dealing with massive data input, increase the buffer size
            this.uart.ReadBufferSize = 2048;



        }
        
        void Uart_DataReceived(UartController sender, DataReceivedEventArgs e)
        {
            if (ques.Count > 100) ques.Clear();
            var bytesReceived = uart.Read(readbuffer, 0, e.Count);
            Debug.WriteLine(Encoding.UTF8.GetString(readbuffer, 0, bytesReceived));
            for(var i=0;i< bytesReceived;i++)
            {
                ques.Enqueue(readbuffer[i]);
            }
            
        }

        public SensorData GetCurrentData()
        {
            return current;
        }

        public void StartSensing()
        {
            if (th1 == null)
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
        //byte[] readbuff = new byte[1];
        Queue ques = new Queue();
        double temp;
        byte[] databuffer = new byte[35];
        byte[] readbuffer = new byte[100];
        void getBuffer()                                                                    //Get weather status data
        {
            
            int index;
            for (index = 0; index < 35; index++)
            {
                
                if (ques.Count > 0)
                {
                    databuffer[index] = (byte)ques.Dequeue();
                    //var bytesReceived = uart.Read(readbuff, 0, uart.BytesToRead);
                    //Debug.WriteLine(Encoding.UTF8.GetString(readbuff, 0, bytesReceived));

                    //databuffer[index] = (byte)readbuff[0];
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