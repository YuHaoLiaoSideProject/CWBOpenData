using CWBOpenData.Helpers.APIRequest;
using CWBOpenData.JsonModel.CWBModel;

namespace CWBOpenData.Services
{
    public class CWBAPIService : ICWBAPIService
    {
        private static readonly string _Authorization = "CWB-147426DF-BB6E-41BF-B05E-0E613245F77E";
        private static readonly string _BaseUrl = "https://opendata.cwb.gov.tw/api/";
        public CWBResponseJsonModel<WeatherForecastTo36HourJsonModel> GetWeatherForecastTo36Hour()
        {
            var result =
                WebRequestHelper.Request(_BaseUrl, "v1/rest/datastore/F-C0032-001")
                .Get(e => e.AddParameter("Authorization", _Authorization))
                .Response<CWBResponseJsonModel<WeatherForecastTo36HourJsonModel>>();

            return result;
        }
    }
}
