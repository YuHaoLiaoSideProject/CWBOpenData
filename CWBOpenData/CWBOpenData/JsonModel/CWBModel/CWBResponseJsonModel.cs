using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CWBOpenData.JsonModel.CWBModel
{
    public class CWBResponseJsonModel<T>
    {
        public string Success { get; set; }
        
        public CWBResponseResultJsonModel Result { get; set; }
        public T Records { get; set; }
    }
}
