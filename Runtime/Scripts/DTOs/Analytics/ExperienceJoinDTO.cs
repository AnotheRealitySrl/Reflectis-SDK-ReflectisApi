using System;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    public class ExperienceJoinDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;

    }
}
