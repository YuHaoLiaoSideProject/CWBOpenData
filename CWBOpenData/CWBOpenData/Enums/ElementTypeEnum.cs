using System.ComponentModel;

namespace CWBOpenData.Enums
{
    public enum ElementTypeEnum
    {
        Unknown = 0,
        [Description("天氣現象")]
        Wx = 1,
        [Description("最高溫度")]
        MaxT = 2,
        [Description("最低溫度")]
        MinT = 3,
        [Description("舒適度")]
        CI = 4,
        [Description("降雨機率")]
        PoP = 5,
    }
}
