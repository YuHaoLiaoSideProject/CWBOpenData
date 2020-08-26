using System.Collections.Generic;

namespace CWBOpenData.JsonModel.CWBModel
{
    public class LocationJsonModel
    {
        public string LocationName { get; set; }
        public List<WeatherelementJsonModel> WeatherElement { get; set; }
    }
}
