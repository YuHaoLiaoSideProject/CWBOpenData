using CWBOpenData.ConfigModels;
using CWBOpenData.Helpers.APIRequest;
using CWBOpenData.JsonModel.CWBModel;
using Microsoft.Extensions.Options;

namespace CWBOpenData.Services
{
    public class CWBAPIService : ICWBAPIService
    {
        private string _Authorization;
        private string _BaseUrl;
        public CWBAPIService(IOptions<CWBSettingConfig>  config)
        {
            _Authorization = config.Value.Authorization;
            _BaseUrl = config.Value.DomainUrl;
        }
        public CWBResponseJsonModel<WeatherForecastTo36HourJsonModel> GetWeatherForecastTo36Hour()
        {
            var result =
                WebRequestHelper.Request(_BaseUrl, "api/v1/rest/datastore/F-C0032-001")
                .Get(e => e.AddParameter("Authorization", _Authorization))
                .Response<CWBResponseJsonModel<WeatherForecastTo36HourJsonModel>>();

            return result;
        }
    }
}
