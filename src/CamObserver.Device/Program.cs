using CamObserver.Device.Properties;
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
using GHIElectronics.TinyCLR.Devices.Display;

namespace CamObserver.Device
{
    internal class Program
    {
        private static ST7735Controller st7735;
        private const int SCREEN_WIDTH = 160;
        private const int SCREEN_HEIGHT = 128;
        const int MATRIX_WIDTH = rows;
        const int MATRIX_HEIGHT = cols * 2;
        const int cols = 32;
        const int rows = 8;
        static int pct = 0;
        static CounterData CurrentCounter;
        public enum Chips { SC20100, SC20260, SC13040 };
        static Chips MyChip = Chips.SC20260;
        private static void Main()
        {

            if (MyChip == Chips.SC13040)
            {
                var spi = SpiController.FromName(SC13048.SpiBus.Spi1);
                var gpio = GpioController.GetDefault();

                st7735 = new ST7735Controller(
                    spi.GetDevice(ST7735Controller.GetConnectionSettings
                    (SpiChipSelectType.Gpio, gpio.OpenPin(SC13048.GpioPin.PB14))), //CS pin.
                    gpio.OpenPin(SC13048.GpioPin.PA14), //RS pin.
                    gpio.OpenPin(SC13048.GpioPin.PA13) //RESET pin.
                );

                var backlight = gpio.OpenPin(SC13048.GpioPin.PB13);
                backlight.SetDriveMode(GpioPinDriveMode.Output);
                backlight.Write(GpioPinValue.High);

                st7735.SetDataAccessControl(true, true, false, false); //Rotate the screen.
                st7735.SetDrawWindow(0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1);
                st7735.Enable();

                var screen = new BasicGraphicsImp(SCREEN_WIDTH, SCREEN_HEIGHT);
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

                screen.DrawString("Cam Observer 1.0", (uint)Color.Blue.ToArgb(), 50, 110);

                screen.Flush();

                var sensorCuaca = new WeatherSensor(SC13048.UartPort.Uart2);
                sensorCuaca.StartSensing();

                //var BarPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
                var InfoPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
                //var BarMatrix = new LedMatrix(BarPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                var InfoMatrix = new LedMatrix(InfoPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                CurrentCounter = new CounterData() { Bicycle = 0, Person = 0 };
                var xbee = new Xbee(SC13048.UartPort.Uart1);
                xbee.DataReceived += (s, o) =>
                {
                    if (!string.IsNullOrEmpty(o.Data))
                    {
                        var dict = JsonSerializer.DeserializeString(o.Data) as Hashtable;
                        if (dict != null)
                            foreach (var item in dict.Keys)
                            {
                                if (item == "Person")
                                {
                                    CurrentCounter.Person = (long)dict[item];
                                }
                                else
                                if (item == "Bicycle")
                                {
                                    CurrentCounter.Bicycle = (long)dict[item];
                                }
                                else
                                    if (item == "Message")
                                {
                                    infoBox.Pesan = dict[item].ToString();
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
                const int MaxInfo = 9;
                Random rnd = new Random();
                int InfoCounter = 0;

                while (true)
                {
                    var current = sensorCuaca.GetCurrentData();
                    if (current != null)
                    {
                        //screen.Clear();
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
                                InfoMatrix.DrawString($"Temp: {current.Temperature.ToString("n0")}", (uint)Color.Blue.ToArgb(), 0, 0);
                                break;
                            case 1:
                                InfoMatrix.DrawString($"ARAH ANGIN: {current.WindDirection.ToString("n0")}", (uint)Color.Red.ToArgb(), 0, 0);
                                break;
                            case 2:
                                InfoMatrix.DrawString($"KEC ANGIN: {current.WindSpeedAverage.ToString("n0")}", (uint)Color.Yellow.ToArgb(), 0, 0);
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
            }
            else if (MyChip == Chips.SC20260)
            {
                GpioPin backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);
                backlight.SetDriveMode(GpioPinDriveMode.Output);
                backlight.Write(GpioPinValue.High);

                var displayController = DisplayController.GetDefault();

                // Enter the proper display configurations
                displayController.SetConfiguration(new ParallelDisplayControllerSettings
                {
                    Width = 480,
                    Height = 272,
                    DataFormat = DisplayDataFormat.Rgb565,
                    Orientation = DisplayOrientation.Degrees0, //Rotate display.
                    PixelClockRate = 10000000,
                    PixelPolarity = false,
                    DataEnablePolarity = false,
                    DataEnableIsFixed = false,
                    HorizontalFrontPorch = 2,
                    HorizontalBackPorch = 2,
                    HorizontalSyncPulseWidth = 41,
                    HorizontalSyncPolarity = false,
                    VerticalFrontPorch = 2,
                    VerticalBackPorch = 2,
                    VerticalSyncPulseWidth = 10,
                    VerticalSyncPolarity = false,
                });

                displayController.Enable(); //This line turns on the display I/O and starts
                                            //  refreshing the display. Native displays are
                                            //  continually refreshed automatically after this
                                            //  command is executed.

                var screen = Graphics.FromHdc(displayController.Hdc);


                screen.Clear();

                var font = Resources.GetFont(Resources.FontResources.small);

                screen.Clear();

                screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb
                    (255, 255, 0, 0)), 0, 0, 240, 136);

                screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb
                    (255, 0, 0, 255)), 240, 0, 240, 136);

                screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb
                    (128, 0, 255, 0)), 120, 0, 240, 136);


                screen.DrawRectangle(new Pen(Color.Yellow), 10, 150, 140, 100);
                screen.DrawEllipse(new Pen(Color.Purple), 170, 150, 140, 100);
                screen.FillRectangle(new SolidBrush(Color.Teal), 330, 150, 140, 100);

                screen.DrawLine(new Pen(Color.White), 10, 271, 470, 271);
                screen.SetPixel(240, 200, Color.White);

                screen.DrawString("Cam Observer 1.0", font, new SolidBrush(Color.Blue), 210, 255);

                screen.Flush();

                var sensorCuaca = new WeatherSensor(SC20260.UartPort.Uart1);
                sensorCuaca.StartSensing();

                //var BarPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
                var InfoPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA0);
                var InfoPin2 = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PB0);
                //var BarMatrix = new LedMatrix(BarPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                var InfoMatrix = new LedMatrix(InfoPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                var InfoMatrix2 = new LedMatrix(InfoPin2, MATRIX_HEIGHT, MATRIX_WIDTH);
                var infoBox = new InfoBox(InfoMatrix2);
                infoBox.StartAnimation();
                CurrentCounter = new CounterData() { Bicycle = 0, Person = 0 };
                var xbee = new Xbee(SC20260.UartPort.Uart2);
                xbee.DataReceived += (s, o) =>
                {
                    if (!string.IsNullOrEmpty(o.Data))
                    {
                        try
                        {
                            var dict = JsonSerializer.DeserializeString(o.Data) as Hashtable;
                            if (dict != null)
                                foreach (var item in dict.Keys)
                                {
                                    if (item == "Person")
                                    {
                                        CurrentCounter.Person = (long)dict[item];
                                    }
                                    else
                                    if (item == "Bicycle")
                                    {
                                        CurrentCounter.Bicycle = (long)dict[item];
                                    }
                                    else
                                    if (item == "Message")
                                    {
                                        infoBox.Pesan = dict[item].ToString();
                                    }
                                }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
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
                const int MaxInfo = 9;
                Random rnd = new Random();
                int InfoCounter = 0;
                var solid = new SolidBrush(Color.Blue);
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

                        screen.DrawString($"Temp: {current.Temperature}", font, solid, 10, 10);
                        screen.DrawString($"Wind Dir: {current.WindDirection}", font, solid, 10, 20);
                        screen.DrawString($"Wind Speed: {current.WindSpeedAverage}", font, solid, 10, 30);
                        screen.DrawString($"Rain 1d: {current.RainfallOneDay}", font, solid, 10, 40);
                        screen.DrawString($"Rain 1h: {current.RainfallOneHour}", font, solid, 10, 50);
                        screen.DrawString($"Barometer: {current.BarPressure}", font, solid, 10, 60);
                        screen.DrawString($"Humid: {current.Humidity}", font, solid, 10, 70);
                        screen.DrawString($"Persen: {pct} %", font, solid, 10, 80);
                        switch (InfoCounter)
                        {
                            case 0:
                                InfoMatrix.DrawString($"Temp: {current.Temperature.ToString("n0")}", (uint)Color.Blue.ToArgb(), 0, 0);
                                break;
                            case 1:
                                InfoMatrix.DrawString($"ARAH ANGIN: {current.WindDirection.ToString("n0")}", (uint)Color.Red.ToArgb(), 0, 0);
                                break;
                            case 2:
                                InfoMatrix.DrawString($"KEC ANGIN: {current.WindSpeedAverage.ToString("n0")}", (uint)Color.Yellow.ToArgb(), 0, 0);
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
            }
            else if (MyChip == Chips.SC20100)
            {

                var gpio = GpioController.GetDefault();

                var sensorCuaca = new WeatherSensor(SC20100.UartPort.Uart5); //pb12,pb13
                sensorCuaca.StartSensing();

                //var BarPin = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
                var InfoPin = GpioController.GetDefault().OpenPin(SC20100.GpioPin.PC8);
                var InfoPin2 = GpioController.GetDefault().OpenPin(SC20100.GpioPin.PC9);
                //var BarMatrix = new LedMatrix(BarPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                var InfoMatrix = new LedMatrix(InfoPin, MATRIX_HEIGHT, MATRIX_WIDTH);
                var InfoMatrix2 = new LedMatrix(InfoPin2, MATRIX_HEIGHT, MATRIX_WIDTH);
                var infoBox = new InfoBox(InfoMatrix2);
                infoBox.StartAnimation();
                CurrentCounter = new CounterData() { Bicycle = 0, Person = 0 };
                var xbee = new Xbee(SC20100.UartPort.Uart1); //pa 10, pa 9
                xbee.DataReceived += (s, o) =>
                {
                    if (!string.IsNullOrEmpty(o.Data))
                    {
                        try
                        {
                            var dict = JsonSerializer.DeserializeString(o.Data) as Hashtable;
                            if (dict != null)
                                foreach (var item in dict.Keys)
                                {
                                    if (item == "Person")
                                    {
                                        CurrentCounter.Person = (long)dict[item];
                                    }
                                    else
                                    if (item == "Bicycle")
                                    {
                                        CurrentCounter.Bicycle = (long)dict[item];
                                    }
                                    else
                                    if (item == "Message")
                                    {
                                        infoBox.Pesan = dict[item].ToString();
                                    }
                                }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
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
                const int MaxInfo = 9;
                Random rnd = new Random();
                int InfoCounter = 0;
                var solid = new SolidBrush(Color.Blue);
                while (true)
                {
                    var current = sensorCuaca.GetCurrentData();
                    if (current != null)
                    {
                        InfoMatrix.Clear();
                        CurrentCounter.Person = rnd.Next(1000);
                        CurrentCounter.Bicycle = rnd.Next(1000);
                        var jsonToken = JsonSerializer.SerializeObject(current);
                        var json = jsonToken.ToString();
                        xbee.SendMessage(json);

                        Debug.WriteLine($"Temp: {current.Temperature}");
                        Debug.WriteLine($"Wind Dir: {current.WindDirection}");
                        Debug.WriteLine($"Wind Speed: {current.WindSpeedAverage}");
                        Debug.WriteLine($"Rain 1d: {current.RainfallOneDay}");
                        Debug.WriteLine($"Rain 1h: {current.RainfallOneHour}");
                        Debug.WriteLine($"Barometer: {current.BarPressure}");
                        Debug.WriteLine($"Humid: {current.Humidity}");
                        Debug.WriteLine($"Persen: {pct} %");
                        switch (InfoCounter)
                        {
                            case 0:
                                InfoMatrix.DrawString($"Temp: {current.Temperature.ToString("n0")}", (uint)Color.Blue.ToArgb(), 0, 0);
                                break;
                            case 1:
                                InfoMatrix.DrawString($"ARAH ANGIN: {current.WindDirection.ToString("n0")}", (uint)Color.Red.ToArgb(), 0, 0);
                                break;
                            case 2:
                                InfoMatrix.DrawString($"KEC ANGIN: {current.WindSpeedAverage.ToString("n0")}", (uint)Color.Yellow.ToArgb(), 0, 0);
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
                        
                    }


                    Thread.Sleep(2000);
                }
            }
        }

    }

    public class InfoBox
    {
        int cols;
        int rows;
        Random rnd;
        LedMatrix screen;
        public string Pesan { set; get; } = "";
        public InfoBox(LedMatrix screen)
        {

            this.cols = (int)screen.column;
            this.rows = (int)screen.row;
            this.rnd = new Random();
            this.screen = screen;
        }

        Thread th1;

        public void StartAnimation()
        {
            if (th1 == null)
            {
                th1 = new Thread(new ThreadStart(Animation));
                th1.Start();
            }
        }
        void Animation()
        {
            string[] words = { "WILUJENG", "SUMPING" };
            string[] words2 = { "BOGOR", "TEGAR", "BERIMAN" };
            string[] words3 = { "SELAMAT", "BEROLAHRAGA", "SEMANGAT !!", };
            string[] words4 = { "HIDUP", "SEHAT", "SELALU" };
            while (true)
            {
                CountDownAnimation(0, 100, 1);
                BrickAnimation();
                CharAnimation(words);
                LineAnimation();
                CharAnimation(words2);
                LineAnimation2();
                CharAnimation(words3);
                LineAnimation();
                CharAnimation(words4);
                LineAnimation2();
                BallAnimation(200);
                if (!string.IsNullOrEmpty(Pesan))
                {
                    var pesanArr = Pesan.Split(';');
                    foreach (var msg in pesanArr)
                    {
                        var splitWord = msg.Split(' ');
                        CharAnimation(splitWord);
                        LineAnimation();
                    }
                }
            }
        }
        void BrickAnimation(int Moves = 16 * 4, int Delay = 100)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
            var MaxX = cols / 2;
            var MaxY = rows / 2;
            int x, y;
            for (int i = 0; i < Moves; i++)
            {
                x = rnd.Next(MaxX);
                y = rnd.Next(MaxY);
                screen.DrawRectangle(col, x * 2, y * 2, 2, 2);
                screen.Flush();
                Thread.Sleep(Delay);
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));

            }
        }
        void BallAnimation(int Moves = 1000, int Delay = 50)
        {
            var x = rnd.Next(cols);
            var y = rnd.Next(rows);
            var ax = 1 + rnd.Next(2);
            var ay = 1 + rnd.Next(2);
            screen.Clear();
            var col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
            var current = 0;
            while (current < Moves)
            {

                screen.Clear();
                screen.DrawCircle(col, x, y, 1);
                screen.Flush();
                Thread.Sleep(Delay);
                x += ax;
                y += ay;
                if (x + ax > cols || x < 0)
                {
                    ax = -ax;
                }
                if (ay + y > rows || y < 0)
                {
                    ay = -ay;
                }
                current++;
            }
        }
        void LineAnimation2(int Delay = 10)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);
            var rnd = new Random();
            for (int x = 0; x < cols; x++)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                if (x % 2 == 0)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
                else
                {
                    for (int y = rows - 1; y >= 0; y--)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
            }
        }
        void LineAnimation(int Delay = 10)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);
            var rnd = new Random();
            for (int y = 0; y < rows; y++)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                if (y % 2 == 0)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
                else
                {
                    for (int x = cols - 1; x >= 0; x--)
                    {
                        screen.SetPixel(x, y, col);
                        screen.Flush();
                        Thread.Sleep(Delay);
                    }
                }
            }
        }
        void CharAnimation(string[] Words, int Delay = 500)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);


            foreach (var word in Words)
            {
                col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                var statement = string.Empty;
                for (int i = 0; i < word.Length; i++)
                {
                    statement += word[i];
                    screen.Clear();
                    screen.DrawString(statement.ToString(), col, 0, 0);
                    screen.Flush();
                    Thread.Sleep(Delay);
                }
            }
        }
        void CountDownAnimation(int From, int To, int Incr)
        {
            screen.Clear();
            var col = LedMatrix.ColorFromRgb(0, 20, 50);


            int current = From;
            while (true)
            {
                screen.Clear();
                screen.DrawString(current.ToString(), col, 0, 0);
                screen.Flush();
                Thread.Sleep(10);
                if (current % 10 == 0)
                {
                    col = LedMatrix.ColorFromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                }
                if (current >= Int32.MaxValue) current = 0;
                if (current == To) break;
                current += Incr;
            }
        }
    }

    public class BasicGraphicsImp : BasicGraphics
    {
        ST7735Controller lcd;
        public BasicGraphicsImp(uint Width, uint Height) : base(Width, Height, ColorFormat.Rgb565)
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

    public class BasicGraphicsDisplay : BasicGraphics
    {
        DisplayController lcd;
        public BasicGraphicsDisplay(uint Width, uint Height) : base(Width, Height, ColorFormat.Rgb565)
        {

        }
        public void SetDisplay(DisplayController controller)
        {
            lcd = controller;
        }

        // You may need to add this to send an optional buffer...
        public void Flush()
        {
            // ... for example
            lcd.DrawBuffer(0, 0, 0, 0, this.Width, this.Height, this.Width, this.Buffer, 0);
        }
    }


}
