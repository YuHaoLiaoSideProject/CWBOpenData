using CWBOpenData.ConfigModels;
using CWBOpenData.IRepositories;
using CWBOpenData.Models;
using CWBOpenData.Repositories.BaseRepositories;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace CWBOpenData.Repositories
{    
    public class WeatherForecastDetailRepository : BaseCWBRepository<WeatherForecastDetailModel> , IWeatherForecastDetailRepository
    {
        public WeatherForecastDetailRepository(IOptions<ConnectionStringConfig> config) : base(config)
        {

        }

        public void MergeInsert(List<WeatherForecastDetailModel> models)
        {
            string sql = @"
MERGE INTO WeatherForecastDetail as T
USING (
	SELECT 
		@ElementType AS ElementType
		,@Id AS Id,@ParameterName AS ParameterName
		,@ParameterUnit AS ParameterUnit
		,@ParameterValue AS ParameterValue
		,@WeatherForecastId AS WeatherForecastId
		) AS S ON T.ElementType = S.ElementType AND T.WeatherForecastId = S.WeatherForecastId
WHEN MATCHED THEN
    UPDATE 
	SET 
		T.ElementType = S.ElementType,T.ParameterName = S.ParameterName,T.ParameterUnit = S.ParameterUnit,T.ParameterValue = S.ParameterValue,T.WeatherForecastId = S.WeatherForecastId
WHEN NOT MATCHED THEN
    INSERT(ElementType,ParameterName,ParameterUnit,ParameterValue,WeatherForecastId) 
    VALUES(S.ElementType,S.ParameterName,S.ParameterUnit,S.ParameterValue,S.WeatherForecastId);";
            ExecuteSQL(sql, models);
        }
    }    
}
