using CWBOpenData.IRepositories.IBaseRepositories;
using CWBOpenData.Models;
using System;

namespace CWBOpenData.IRepositories
{
    public interface IWeatherForecastRepository : IBaseRepository<WeatherForecastModel>
    {
        WeatherForecastModel GetByModel(WeatherForecastModel model);
    }
}
