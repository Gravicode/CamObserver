using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamObserver.RadioTransceiver.Models
{
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
}
