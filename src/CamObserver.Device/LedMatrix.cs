using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.Worldsemi.WS2812;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace CamObserver.Device
{
    public class LedMatrix : BasicGraphics
    {
        private uint row, column;
        WS2812Controller leds;

        public LedMatrix(GpioPin pin, uint column, uint row)
        {
            this.row = row;
            this.column = column;
            this.leds = new WS2812Controller(pin, this.row * this.column, WS2812Controller.DataFormat.rgb565);

            Clear();
        }

        public override void Clear()
        {
            leds.Clear();
        }

        public void SetLevel(float Percent)
        {
            leds.Clear();
            var color = ColorFromRgb(0,0,200);
            var onePercent = (float)column / 100;
            var TargetHeight = (int)(Percent * onePercent);
            for (int x = 0; x < TargetHeight; x++)
            {
                for (int y = 0; y < row; y++)
                {
                    SetPixel(x, y, color);
                }
                //Thread.Sleep(10);
            }
            leds.Flush();
        }

        public override void SetPixel(int x, int y, uint color)
        {
            if (x < 0 || x >= this.column) return;
            if (y < 0 || y >= this.row) return;

            // even columns are inverted
            if ((x & 0x01) != 0)
            {
                y = (int)(this.row - 1 - y);
            }

            var index = x * this.row + y;

            leds.SetColor((int)index, (byte)(color >> 16), (byte)(color >> 8), (byte)(color >> 0));
        }
        public void Flush()
        {
            leds.Flush();
        }
    }

}
