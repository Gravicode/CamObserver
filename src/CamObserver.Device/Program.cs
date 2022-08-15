﻿using CamObserver.Device.Properties;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using Json.NETMF;
namespace CamObserver.Device
{
    internal class Program
    {
        private static ST7735Controller st7735;
        private const int SCREEN_WIDTH = 160;
        private const int SCREEN_HEIGHT = 128;
        const int MATRIX_WIDTH = rows;
        const int MATRIX_HEIGHT = cols * 1;
        const int cols = 32;
        const int rows = 8;
        static int pct = 0;
        static CounterData CurrentCounter;
        private static void Main()
        {
            var spi = SpiController.FromName(SC13048.SpiBus.Spi1);
            var gpio = GpioController.GetDefault();

            st7735 = new ST7735Controller(
                spi.GetDevice(ST7735Controller.GetConnectionSettings
                (SpiChipSelectType.Gpio, gpio.OpenPin(SC13048.GpioPin.PB14))), //CS pin.
                gpio.OpenPin(SC13048.GpioPin.PA14), //RS pin.
                gpio.OpenPin(SC20100.GpioPin.PA13) //RESET pin.
            );

            var backlight = gpio.OpenPin(SC13048.GpioPin.PB13);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);

            st7735.SetDataAccessControl(true, true, false, false); //Rotate the screen.
            st7735.SetDrawWindow(0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1);
            st7735.Enable();

            var screen = new BasicGraphicsImp(SCREEN_WIDTH,SCREEN_HEIGHT);
            screen.SetDisplay(st7735);

            screen.Clear();

            screen.DrawCircle((uint)System.Drawing.Color.FromArgb
                (255, 255, 0, 0).ToArgb(), 0, 0, 64);
            
            screen.DrawCircle((uint)System.Drawing.Color.FromArgb
                (255, 0, 0, 255).ToArgb(), 80, 0, 80);

            screen.DrawCircle((uint)System.Drawing.Color.FromArgb
                (128, 0, 255, 0).ToArgb(), 40, 0, 80);

            screen.DrawRectangle((uint)Color.Yellow.ToArgb(), 10, 80, 40, 25);
            screen.DrawCircle((uint)Color.Purple.ToArgb(), 60, 80, 40);
            screen.DrawRectangle((uint)Color.Teal.ToArgb(), 110, 80, 40, 25);

            screen.DrawLine((uint)Color.White.ToArgb(), 10, 127, 150, 127);
            screen.SetPixel(80, 92, (uint)Color.White.ToArgb());

            screen.DrawString("Cam Observer 1.0", (uint) Color.Blue.ToArgb(), 50, 110);

            screen.Flush();

            var sensorCuaca = new WeatherSensor();
            sensorCuaca.StartSensing();

            //var BarPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
            var InfoPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
            //var BarMatrix = new LedMatrix(BarPin, MATRIX_HEIGHT, MATRIX_WIDTH);
            var InfoMatrix = new LedMatrix(InfoPin, MATRIX_HEIGHT, MATRIX_WIDTH);
            CurrentCounter = new CounterData() { Bicycle=0, Person=0 };
            var xbee = new Xbee(SC13048.UartPort.Uart1);
            xbee.DataReceived += (s, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data))
                {
                    var dict = JsonSerializer.DeserializeString(o.Data) as Hashtable;
                    if(dict!=null)
                    foreach(var item in dict.Keys)
                    {
                        if (item == "Person")
                        {
                            CurrentCounter.Person = (long)dict[item] ;
                        }else 
                        if (item == "Bicycle")
                        {
                            CurrentCounter.Bicycle = (long)dict[item];
                        }
                    }
                }
            };
            //BarMatrix.Clear();
          
            /*
            Thread threadMatrix = new Thread(new ThreadStart(() => {
                
                while (true)
                {
                    pct++;
                    BarMatrix.SetLevel(pct);
                    if (pct >= 100)
                    {
                        pct = 0;
                        BarMatrix.Clear();
                    }
                    Thread.Sleep(100);
                }
            }));
            threadMatrix.Start();
            */
            const int MaxInfo= 9;
            Random rnd = new Random();
            int InfoCounter = 0;

            while (true)
            {
                var current = sensorCuaca.GetCurrentData();
                if (current != null)
                {
                    screen.Clear();
                    InfoMatrix.Clear();
                    CurrentCounter.Person = rnd.Next(1000);
                    CurrentCounter.Bicycle = rnd.Next(1000);
                    var jsonToken = JsonSerializer.SerializeObject(current);
                    var json = jsonToken.ToString();
                    xbee.SendMessage(json);                    
                    screen.DrawString($"Temp: {current.Temperature}", (uint)Color.Blue.ToArgb(), 10, 10);
                    screen.DrawString($"Wind Dir: {current.WindDirection}", (uint)Color.Blue.ToArgb(), 10, 20);
                    screen.DrawString($"Wind Speed: {current.WindSpeedAverage}", (uint)Color.Blue.ToArgb(), 10, 30);
                    screen.DrawString($"Rain 1d: {current.RainfallOneDay}", (uint)Color.Blue.ToArgb(), 10, 40);
                    screen.DrawString($"Rain 1h: {current.RainfallOneHour}", (uint)Color.Blue.ToArgb(), 10, 50);
                    screen.DrawString($"Barometer: {current.BarPressure}", (uint)Color.Blue.ToArgb(), 10, 60);
                    screen.DrawString($"Humid: {current.Humidity}", (uint)Color.Blue.ToArgb(), 10, 70);
                    screen.DrawString($"Persen: {pct} %", (uint)Color.Blue.ToArgb(), 10, 80);
                    switch (InfoCounter)
                    {
                        case 0:
                            InfoMatrix.DrawString($"Temp: {current.Temperature.ToString("n0")}", (uint)Color.Blue.ToArgb(),0,0);
                            break; 
                        case 1:
                            InfoMatrix.DrawString($"ARAH ANGIN: {current.WindDirection.ToString("n0")}", (uint)Color.Red.ToArgb(),0,0);
                            break;  
                        case 2:
                            InfoMatrix.DrawString($"KEC ANGIN: {current.WindSpeedAverage.ToString("n0")}", (uint)Color.Yellow.ToArgb(),0,0);
                            break;
                        case 3:
                            InfoMatrix.DrawString($"HUJAN 1HARI: {current.RainfallOneDay}", (uint)Color.Green.ToArgb(), 0, 0);
                            break;
                        case 4:
                            InfoMatrix.DrawString($"HUJAN 1JAM: {current.RainfallOneHour.ToString("n0")}", (uint)Color.White.ToArgb(), 0, 0);
                            break;
                        case 5:
                            InfoMatrix.DrawString($"BAROMETER: {current.BarPressure.ToString("n0")}", (uint)Color.Purple.ToArgb(), 0, 0);
                            break;
                        case 6:
                            InfoMatrix.DrawString($"HUMID: {current.Humidity.ToString("n0")}", (uint)Color.Teal.ToArgb(), 0, 0);
                            break; 
                        case 7:
                            InfoMatrix.DrawString($"ORANG: {CurrentCounter.Person.ToString("n0")}", (uint)Color.Green.ToArgb(), 0, 0);
                            break;    
                        case 8:
                            InfoMatrix.DrawString($"SEPEDA: {CurrentCounter.Bicycle.ToString("n0")}", (uint)Color.Yellow.ToArgb(), 0, 0);
                            break;
                    }
                    InfoMatrix.Flush();
                    InfoCounter++;
                    if (InfoCounter >= MaxInfo) InfoCounter = 0;
                    screen.Flush();
                }


                Thread.Sleep(2000);
            }
            
            Thread.Sleep(Timeout.Infinite);
        }

    }

    public class BasicGraphicsImp : BasicGraphics
    {
        ST7735Controller lcd;
        public BasicGraphicsImp(uint Width, uint Height):base(Width,Height, ColorFormat.Rgb565)
        {
            
        }
        public void SetDisplay(ST7735Controller controller)
        {
            lcd = controller;
        }
      
        // You may need to add this to send an optional buffer...
        public void Flush()
        {
            // ... for example
            lcd.DrawBufferNative(this.Buffer);
        }
    }
    

}
