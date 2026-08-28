using System;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    public class ExperienceStartDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;
    }
}
