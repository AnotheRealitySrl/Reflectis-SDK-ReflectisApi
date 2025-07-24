namespace Reflectis.CreatorKit.Worlds.Analytics
{
    public abstract class ExperienceStepDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string stepId;
    }
}
