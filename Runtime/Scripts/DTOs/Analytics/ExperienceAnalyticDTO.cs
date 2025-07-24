namespace Reflectis.CreatorKit.Worlds.Analytics
{
    public abstract class ExperienceAnalyticDTO : AnalyticDTO
    {
        [SettableField(isRequired = true)]
        protected string key;

        public string uniqueId;

        public string GetKey()
        {
            return key;
        }
    }
}
