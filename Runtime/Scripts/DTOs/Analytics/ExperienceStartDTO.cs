namespace Reflectis.CreatorKit.Worlds.Analytics
{
    public class ExperienceStartDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;
    }
}
