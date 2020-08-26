using System.Collections.Generic;

namespace CWBOpenData.JsonModel.CWBModel
{
    public class WeatherelementJsonModel
    {
        public string ElementName { get; set; }
        public List<TimeJsonModel> Time { get; set; }
    }
}
