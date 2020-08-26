using CWBOpenData.Attributes;
using CWBOpenData.Enums;

namespace CWBOpenData.Models
{
    public class WeatherForecastDetailModel
    {
        [AutoKey]
        public int Id { get; set; }
        public int WeatherForecastId { get; set; }
        public ElementTypeEnum ElementType { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterUnit { get; set; }
    }
}
