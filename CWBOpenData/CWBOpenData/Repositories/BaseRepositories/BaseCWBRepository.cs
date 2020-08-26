using CWBOpenData.ConfigModels;
using Microsoft.Extensions.Options;

namespace CWBOpenData.Repositories.BaseRepositories
{
    public class BaseCWBRepository<T> : BaseRepository<T> where T : class
    {
        public static new string _tableName = typeof(T).Name.Replace("Model", string.Empty);
        public BaseCWBRepository(IOptions<ConnectionStringConfig> config) : base(config.Value.CWBDBStr, _tableName)
        {

        }
    }
}
