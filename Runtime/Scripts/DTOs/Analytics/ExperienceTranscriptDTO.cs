namespace Reflectis.SDK.ReflectisApi
{
    public class ExperienceTranscriptDTO : ExperienceStepDTO
    {
        [SettableField(isRequired = true)]
        public string description;
    }
}
