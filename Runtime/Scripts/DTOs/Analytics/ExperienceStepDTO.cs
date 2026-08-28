namespace Virtuademy.SDK.PlatformApi
{
    public abstract class ExperienceStepDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string stepId;
    }
}
