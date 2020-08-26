using CWBOpenData.IRepositories.IBaseRepositories;
using CWBOpenData.Models;
using System.Collections.Generic;

namespace CWBOpenData.IRepositories
{
    public interface IWeatherForecastDetailRepository : IBaseRepository<WeatherForecastDetailModel>
    {
        void MergeInsert(List<WeatherForecastDetailModel> models);
    }
}
