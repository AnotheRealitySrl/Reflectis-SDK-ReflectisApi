namespace Reflectis.SDK.ReflectisApi
{
    public class ExperienceStartDTO : ExperienceAnalyticDTO
    {
        [SettableField(isRequired = true)]
        public string context;
    }
}
