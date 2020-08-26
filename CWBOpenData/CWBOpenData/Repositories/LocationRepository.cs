using CWBOpenData.ConfigModels;
using CWBOpenData.IRepositories;
using CWBOpenData.Models;
using CWBOpenData.Repositories.BaseRepositories;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace CWBOpenData.Repositories
{    
    public class LocationRepository : BaseCWBRepository<LocationModel> , ILocationRepository
    {
        public LocationRepository(IOptions<ConnectionStringConfig> config) : base(config)
        {

        }
    }    
}
