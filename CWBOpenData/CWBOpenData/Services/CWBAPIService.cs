using CWBOpenData.Attributes;
using CWBOpenData.ConfigModels;
using CWBOpenData.Enums;
using CWBOpenData.Extensions;
using CWBOpenData.Helpers.APIRequest;
using CWBOpenData.IRepositories;
using CWBOpenData.JsonModel.CWBModel;
using CWBOpenData.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CWBOpenData.Services
{
    public class CWBAPIService : ICWBAPIService
    {
        ILocationRepository _LocationRepositorie { get; set; }
        IWeatherForecastRepository _WeatherForecastRepository { get; set; }
        IWeatherForecastDetailRepository _WeatherForecastDetailRepository { get; set; }
        private string _Authorization;
        private string _BaseUrl;
        public CWBAPIService(
            ILocationRepository locationRepositorie
            , IWeatherForecastRepository weatherForecastRepository
            , IWeatherForecastDetailRepository weatherForecastDetailRepository
            , IOptions<CWBSettingConfig>  config )
        {
            _LocationRepositorie = locationRepositorie;
            _WeatherForecastRepository = weatherForecastRepository;
            _WeatherForecastDetailRepository = weatherForecastDetailRepository;
            _Authorization = config.Value.Authorization;
            _BaseUrl = config.Value.DomainUrl;
        }
        public CWBAPIService(ILocationRepository locationRepositorie, IOptions<CWBSettingConfig> config)
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

            return result;
        }
        public void CreateWeatherForecastTo36Hour()
        {
            var result =
                WebRequestHelper.Request(_BaseUrl, "api/v1/rest/datastore/F-C0032-001")
                .Get(e => e.AddParameter("Authorization", _Authorization))
                .Response<CWBResponseJsonModel<WeatherForecastTo36HourJsonModel>>();

            //新增地區資料
            CreateLocations(result.Records);

            //從資料庫取得地區資料
            Dictionary<string,int> locations = 
                _LocationRepositorie.GetList()
                .ToDictionary(e => e.Name, e => e.Id);

            //key = $"{item.LocationName}_{t.StartTime}_{t.EndTime}"
            var weatherForecasts = new Dictionary<string, WeatherForecastModel>();
            //key = $"{key}_{weather.ElementName}"
            var weatherForecastDetails = new Dictionary<string, WeatherForecastDetailModel>();

            foreach (var item in result.Records.Location)
            {
                foreach (var weather in item.WeatherElement)
                {
                    foreach (var t in weather.Time)
                    {
                        //同一個地區 同時間只會有一筆
                        string key = $"{item.LocationName}_{t.StartTime}_{t.EndTime}";
                        int weatherId = 0;
                        if (weatherForecasts.ContainsKey(key) == false)
                        {
                            DateTime? startTime = t.StartTime.TryToDateTime();
                            DateTime? entTime = t.EndTime.TryToDateTime();

                            if (startTime.HasValue == false || entTime.HasValue == false)
                            {
                                //時間轉型失敗處裡?
                                throw new NotImplementedException();
                            }

                            var weatherEntity = new WeatherForecastModel
                            {
                                LocationId = locations.GetValueOrDefault(item.LocationName),
                                StartTime = startTime.Value,
                                EndTime = entTime.Value,
                            };
                            //新增 WeatherForecast 資料
                            var entity = _WeatherForecastRepository.GetByModel(weatherEntity);
                            //資料不存在則新增
                            if (entity == null)
                            {
                                weatherForecasts.Add(key, weatherEntity);
                                weatherId = _WeatherForecastRepository.CreateAndResultIdentity<int>(weatherEntity);
                                weatherEntity.Id = weatherId;
                            }
                            else
                            {
                                weatherForecasts.Add(key, entity);
                                weatherId = entity.Id;
                            }
                        }
                        else
                        {
                            weatherId = weatherForecasts[key].Id;
                        }

                        var detailKey = $"{key}_{weather.ElementName}";

                        if (weatherForecastDetails.ContainsKey(detailKey) == false)
                        {
                            weatherForecastDetails.Add(detailKey, new WeatherForecastDetailModel
                            {
                                ElementType = weather.ElementName.TryToEnum<ElementTypeEnum>().GetValueOrDefault(),
                                ParameterName = t.Parameter.ParameterName,
                                ParameterUnit = t.Parameter.ParameterUnit,
                                ParameterValue = t.Parameter.ParameterValue,
                                WeatherForecastId = weatherId
                            });
                        }
                        else
                        {
                            //有已經存在的可能性?
                            throw new NotImplementedException();
                        }
                    }
                }                
            }

            //新增 WeatherForecastDetail 資料
            var details = weatherForecastDetails.Values.ToList();

            _WeatherForecastDetailRepository.MergeInsert(details);

        }
        
        private void CreateLocations(WeatherForecastTo36HourJsonModel records)
        {
            List<string> locationNames = records.Location.Select(e => e.LocationName).ToList();
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
