using CWBOpenData.Attributes;
using System;

namespace CWBOpenData.Models
{
    public class WeatherForecastModel
    {
        [AutoKey]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
