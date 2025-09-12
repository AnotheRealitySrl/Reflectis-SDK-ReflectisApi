using System;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    public class ExperienceTranscriptDTO : ExperienceStepDTO
    {
        [SettableField(isRequired = true)]
        public string description;
    }
}
