using CWBOpenData.Attributes;

namespace CWBOpenData.Models
{
    public class LocationModel
    {
        [AutoKey]
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
