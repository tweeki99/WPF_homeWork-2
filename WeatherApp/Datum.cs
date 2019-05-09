using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class Datum
    {
        public int Time { get; set; }
        public string Icon { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
    }
}
