using System;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    public class ExperienceStartDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;
    }
}
