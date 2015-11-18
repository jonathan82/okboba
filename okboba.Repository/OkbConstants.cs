namespace okboba.Repository
{
    //Activities
    public enum ActivityCategories { Joined, UploadedPhoto, EditedProfileText, AnsweredQuestion }

    public static class OkbConstants
    {
        //Gender
        public const byte UNKNOWN_GENDER = 0;
        public const byte MALE = 1;
        public const byte FEMALE = 2;

        //Avatar
        public const int AVATAR_WIDTH = 200;
        public const int AVATAR_HEIGHT = 200;
        public const int AVATAR_WIDTH_SMALL = 90;
        public const int AVATAR_HEIGHT_SMALL = 90;

        //Profile
        public const int MAX_PROFILE_TEXT_SIZE = 4000;

        //Profile Details
        public const string DETAIL_EDUCATION = "Education";
        public const string DETAIL_RELATIONSHIPSTATUS = "RelationshipStatus";
        public const string DETAIL_HAVECHILDREN = "HaveChildren";
        public const string DETAIL_WANTCHILDREN = "WantChildren";
        public const string DETAIL_NATIONALITY = "Nationality";
        public const string DETAIL_MONTHLYINCOME = "MonthlyIncome";
        public const string DETAIL_LIVINGSITUATION = "LivingSituation";
        public const string DETAIL_CARSITUATION = "CarSituation";
        public const string DETAIL_SMOKE = "Smoke";
        public const string DETAIL_DRINK = "Drink";
        public const string DETAIL_EXERCISE = "Exercise";
        public const string DETAIL_EATING = "Eating";
        public const string DETAIL_SHOPPING = "Shopping";
        public const string DETAIL_RELIGION = "Religion";
        public const string DETAIL_SLEEPSCHEDULE = "SleepSchedule";
        public const string DETAIL_SOCIALCIRCLE = "SocialCircle";
        public const string DETAIL_MOSTMONEY = "MostMoney";
        public const string DETAIL_HOUSEWORK = "Housework";
        public const string DETAIL_LOVEPETS = "LovePets";
        public const string DETAIL_HAVEPETS = "HavePets";
        public const string DETAIL_JOB = "Job";
        public const string DETAIL_INDUSTRY = "Industry";
        public const string DETAIL_WORKHOURS = "WorkHours";
        public const string DETAIL_CAREERANDFAMILY = "CareerAndFamily";
        public const string DETAIL_BODYTYPE = "BodyType";
        public const string DETAIL_FACETYPE = "FaceType";
        public const string DETAIL_EYECOLOR = "EyeColor";
        public const string DETAIL_EYESHAPE = "EyeShape";
        public const string DETAIL_HAIRCOLOR = "HairColor";
        public const string DETAIL_HAIRTYPE = "HairType";
        public const string DETAIL_HAIRLENGTH = "HairLength";
        public const string DETAIL_SKINTYPE = "SkinType";
        public const string DETAIL_MUSCLE = "Muscle";
        public const string DETAIL_HEALTHCONDITION = "HealthCondition";
        public const string DETAIL_DRESSSTYLE = "DressStyle";
        public const string DETAIL_SOCIABILITY = "Sociability";
        public const string DETAIL_HUMOUR = "Humour";
        public const string DETAIL_TEMPER = "Temper";
        public const string DETAIL_FEELINGS = "Feelings";
        public const string DETAIL_LOOKINGFOR = "LookingFor";
        public const string DETAIL_ECONOMICCONCEPT = "EconomicConcept";
    }
}
