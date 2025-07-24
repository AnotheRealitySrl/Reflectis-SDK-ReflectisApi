namespace Reflectis.CreatorKit.Worlds.Analytics
{
    public class ExperienceCompleteDTO : ExperienceAnalyticDTO
    {
        [SettableField]
        public int score;
        [SettableField]
        public int? maxScore;
        [SettableField]
        public int? passingScore;
        [SettableField]
        public EExperienceOutcome outcome;
        [SettableField]
        public EExperienceScoringType scoringType;
        [SettableField]
        public string notes;
    }
}
