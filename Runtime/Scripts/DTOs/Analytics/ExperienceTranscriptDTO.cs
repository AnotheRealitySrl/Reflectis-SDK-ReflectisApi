using System;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    public class ExperienceTranscriptDTO : ExperienceStepDTO
    {
        [SettableField(isRequired = true)]
        public string description;
    }
}
