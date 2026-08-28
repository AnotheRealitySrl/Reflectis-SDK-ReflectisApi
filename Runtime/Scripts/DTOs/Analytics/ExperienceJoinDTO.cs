using System;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    public class ExperienceJoinDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;

    }
}
