namespace Reflectis.SDK.ReflectisApi
{
    public abstract class ExperienceStepDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string stepId;
    }
}
