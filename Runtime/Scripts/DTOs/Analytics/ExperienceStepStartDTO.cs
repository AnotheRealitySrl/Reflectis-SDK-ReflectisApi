using System;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    public class ExperienceStepStartDTO : ExperienceStepDTO
    {
        [SettableField(isRequired = true)]
        public string description;
    }
}
