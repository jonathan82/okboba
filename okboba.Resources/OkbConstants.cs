namespace okboba.Resources
{
   
    public static class OkbConstants
    {
        //Questions
        public const int NUM_QUES_PER_PAGE = 30;

        //Matches
        public const int MAX_MATCH_RESULTS = 4000;
        public const int MATCHES_PER_PAGE = 28;
        public const int EXPIRE_MATCHES = 5; //minutes that matches will exist in cache

        //Messages
        public const int MAX_MESSAGE_LENGTH = 1000;
        public const int MESSAGES_PER_PAGE = 20; //number of messages to show per "load" request
        public const int INITIAL_NUM_MESSAGES = 5; //number of messages show initially

        //Activity Feed
        public enum ActivityCategories { Joined = 1, UploadedPhoto = 2, EditedProfileText = 3, AnsweredQuestion = 4 }
        public const int FEED_BLURB_SIZE = 100;
        public const int ACTIVITY_UPLOADEDPHOTO_INTERVAL = 5; // interval between photo uploads in minutes
        public const int ACTIVITY_EDITEDPROFILE_INTERVAL = 5; // interval between profile edits in minutes
        public const int ACTIVITY_ANSWEREDQUESTION_INTERVAL = 5; // interval between answering questions in minutes
        public const int NUM_ACTIVITIES_TO_SHOW = 20; //# of activities to show on the Home page

        //Photo
        public const int MAX_PHOTO_SIZE = 4000000; //MB
        public const int MAX_NUM_PHOTOS = 10; //max photos per user
        public const string HEADSHOT_SUFFIX = "_t";
        public const string HEADSHOT_SMALL_SUFFIX = "_s";
        public const string THUMBNAIL_SUFFIX = "_u";
        public const string PHOTO_CONTAINER = "photos";

        //Gender
        public const byte UNKNOWN_GENDER = 0;
        public const byte MALE = 1;
        public const byte FEMALE = 2;

        //Avatar
        public const int AVATAR_WIDTH = 200;
        public const int AVATAR_HEIGHT = 200;
        public const int AVATAR_WIDTH_SMALL = 90;
        public const int AVATAR_HEIGHT_SMALL = 90;

        //Profile Text
        public const int MAX_PROFILE_TEXT_SIZE = 4000;

        //Profile Details
        public enum ProfileDetailSections
        {
            Basic, Lifestyle, Job, Appearance, Personality
        }
        public const int MIN_HEIGHT = 120; //CM
        public const int MAX_HEIGHT = 220; //CM
        public const string DETAIL_HEIGHT = "Height";
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
