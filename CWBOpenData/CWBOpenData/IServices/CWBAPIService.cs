using CWBOpenData.JsonModel.CWBModel;

namespace CWBOpenData.Services
{
    public interface ICWBAPIService
    {
        CWBResponseJsonModel<WeatherForecastTo36HourJsonModel> GetWeatherForecastTo36Hour();
    }
}
