using CWBOpenData.ConfigModels;
using CWBOpenData.Helpers.APIRequest;
using CWBOpenData.IRepositories;
using CWBOpenData.JsonModel.CWBModel;
using CWBOpenData.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CWBOpenData.Services
{
    public class CWBAPIService : ICWBAPIService
    {
        ILocationRepository _LocationRepositorie { get; set; }
        private string _Authorization;
        private string _BaseUrl;
        public CWBAPIService(ILocationRepository locationRepositorie, IOptions<CWBSettingConfig>  config )
        {
            _LocationRepositorie = locationRepositorie;
            _Authorization = config.Value.Authorization;
            _BaseUrl = config.Value.DomainUrl;
        }
        public CWBResponseJsonModel<WeatherForecastTo36HourJsonModel> GetWeatherForecastTo36Hour()
        {
            var result =
                WebRequestHelper.Request(_BaseUrl, "api/v1/rest/datastore/F-C0032-001")
                .Get(e => e.AddParameter("Authorization", _Authorization))
                .Response<CWBResponseJsonModel<WeatherForecastTo36HourJsonModel>>();

            List<string> locationNames = result.Records.Location.Select(e => e.LocationName).ToList();
            //新增地區資料
            CreateLocations(locationNames);

            return result;
        }

        private void CreateLocations(List<string> locationNames)
        {
            //從資料庫取得地區資料
            List<LocationModel> locations = _LocationRepositorie.GetList();

            //篩選出資料庫沒有的地區資料
            var insertLocation =
                locationNames.Where(e => locations.Select(f => f.Name).Contains(e) == false)
                .Select(locationName => new LocationModel
                {
                    Name = locationName
                }).ToList();

            _LocationRepositorie.Create(insertLocation);
        }
    }
}
