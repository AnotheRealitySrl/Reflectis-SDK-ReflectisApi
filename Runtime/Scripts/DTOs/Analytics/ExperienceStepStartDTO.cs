using System;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    public class ExperienceStepStartDTO : ExperienceStepDTO
    {
        [SettableField(isRequired = true)]
        public string description;
    }
}
