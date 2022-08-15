using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CamObserver.Device
{
    public class Xbee
    {
        public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
        public class DataReceivedEventArgs : EventArgs
        {
            public string Data;
        }
        public event DataReceivedEventHandler DataReceived;

        bool IsInit = false;
        
        UartController uart;
        public Xbee(string ComPort = SC13048.UartPort.Uart2)
        {
            try
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
                IsInit = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                IsInit = false;
            }
          



        }
        
        public void SendMessage(string Message)
        {
            if (IsInit)
            {
                uart.Write(Encoding.UTF8.GetBytes(Message+Environment.NewLine));
            }
        }
        
        void Uart_DataReceived(UartController sender, GHIElectronics.TinyCLR.Devices.Uart.DataReceivedEventArgs e)
        {
            var readbuffer = new byte[e.Count];
            var bytesReceived = uart.Read(readbuffer, 0, e.Count);
            var dataStr = Encoding.UTF8.GetString(readbuffer, 0, bytesReceived);
            Debug.WriteLine(dataStr);
            DataReceived?.Invoke(this, new DataReceivedEventArgs() { Data = dataStr });

        }
    }
}
