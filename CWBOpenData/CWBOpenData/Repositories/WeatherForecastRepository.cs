using CWBOpenData.ConfigModels;
using CWBOpenData.IRepositories;
using CWBOpenData.Models;
using CWBOpenData.Repositories.BaseRepositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CWBOpenData.Repositories
{    
    public class WeatherForecastRepository : BaseCWBRepository<WeatherForecastModel> , IWeatherForecastRepository
    {
        public WeatherForecastRepository(IOptions<ConnectionStringConfig> config) : base(config)
        {

        }

        public WeatherForecastModel GetByModel(WeatherForecastModel model)
        {
            string whereSQL = "WHERE LocationId = @locationId AND StartTime = @startTime AND EndTime = @endTime";
            return GetByWhereSQL(whereSQL, model);
        }
    }    
}
