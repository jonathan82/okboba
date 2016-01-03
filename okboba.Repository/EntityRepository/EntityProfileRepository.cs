using okboba.Entities;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.EntityRepository
{
    public class EntityProfileRepository : IProfileRepository
    {
        #region Singleton
        private static EntityProfileRepository instance;
        private EntityProfileRepository() { }

        public static EntityProfileRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityProfileRepository();
                }
                return instance;
            }
        }
        #endregion

        //////////////////// Member variables ///////////////////////
        Dictionary<string, IList<ProfileDetailOption>> _detailOptions; //cache the options in memory


        ////////////////////// Methods //////////////////
        public Profile GetProfile(int profileId)
        {
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);
            return profile;
        }

        public ProfileDetail GetProfileDetail(int profileId)
        {
            var db = new OkbDbContext();
            var detail = db.ProfileDetails.Find(profileId);
            if (detail==null)
            {
                detail = new ProfileDetail();
            }
            return detail;
        }

        public ProfileText GetProfileText(int profileId)
        {
            var db = new OkbDbContext();
            var text = db.ProfileTexts.Find(profileId);
            if (text == null)
            {
                text = new ProfileText();
            }
            return text;
        }

        public IDictionary<string,IList<ProfileDetailOption>> GetDetailOptions()
        {
            if (_detailOptions==null)
            {
                LoadDetailOptions();
            }
            return _detailOptions;
        }
       
        private void LoadDetailOptions()
        {
            var db = new OkbDbContext();

            _detailOptions = new Dictionary<string, IList<ProfileDetailOption>>();

            var query = from o in db.ProfileDetailOptions.AsNoTracking()
                        orderby o.ColName, o.Id ascending
                        select o;

            foreach (var option in query)
            {               
                if (!_detailOptions.ContainsKey(option.ColName))
                {
                    _detailOptions.Add(option.ColName, new List<ProfileDetailOption>());
                }

                _detailOptions[option.ColName].Add(new ProfileDetailOption
                {
                    Id = option.Id,
                    Value = option.Value
                });
            }
        }

        public void EditProfileText(int profileId, string text, string whichQuestion)
        {            
            var db = new OkbDbContext();

            //Check if we are adding or updating
            var currProfileText = db.ProfileTexts.Find(profileId);

            if (currProfileText == null)
            {
                currProfileText = new ProfileText { ProfileId = profileId };
                db.ProfileTexts.Add(currProfileText);
            }

            switch (whichQuestion)
            {
                case "q1":
                    currProfileText.Question1 = text;
                    break;
                case "q2":
                    currProfileText.Question2 = text;
                    break;
                case "q3":
                    currProfileText.Question3 = text;
                    break;
                case "q4":
                    currProfileText.Question4 = text;
                    break;
                case "q5":
                    currProfileText.Question5 = text;
                    break;
                default:
                    throw new Exception("Invalid profile text id");
            }

            db.SaveChanges();
        }

        public int GetProfileId(string userId)
        {
            var db = new OkbDbContext();
            var user = db.Users.Find(userId);
            return user == null ? -1 : user.Profile.Id;
        }

        public void EditDetails(ProfileDetail details, OkbConstants.ProfileDetailSections section, int profileId)
        {
            var db = new OkbDbContext();

            //check if we're updating or adding
            var currDetails = db.ProfileDetails.Find(profileId);
            if (currDetails==null)
            {
                //adding new Profile Detail row
                details.ProfileId = profileId;
                db.ProfileDetails.Add(details);
                db.SaveChanges();
                return;
            }

            //otherwise we're updating
            //db.ProfileDetails.Attach(details);
            //db.Entry(details).State = System.Data.Entity.EntityState.Modified;
            
            switch (section)
            {
                case OkbConstants.ProfileDetailSections.Basic:
                    currDetails.LookingFor = details.LookingFor;
                    currDetails.Height = details.Height;
                    currDetails.Education = details.Education;
                    currDetails.RelationshipStatus = details.RelationshipStatus;
                    currDetails.HaveChildren = details.HaveChildren;
                    currDetails.WantChildren = details.WantChildren;
                    currDetails.Nationality = details.Nationality;
                    currDetails.MonthlyIncome = details.MonthlyIncome;
                    currDetails.LivingSituation = details.LivingSituation;
                    currDetails.CarSituation = details.CarSituation;
                    currDetails.EconomicConcept = details.EconomicConcept;
                    //db.Entry(details).Property(col => col.LookingFor).IsModified = true;
                    //db.Entry(details).Property(col => col.Height).IsModified = true;
                    //db.Entry(details).Property(col => col.Education).IsModified = true;
                    //db.Entry(details).Property(col => col.RelationshipStatus).IsModified = true;
                    //db.Entry(details).Property(col => col.HaveChildren).IsModified = true;
                    //db.Entry(details).Property(col => col.WantChildren).IsModified = true;
                    //db.Entry(details).Property(col => col.Nationality).IsModified = true;
                    //db.Entry(details).Property(col => col.MonthlyIncome).IsModified = true;
                    //db.Entry(details).Property(col => col.LivingSituation).IsModified = true;
                    //db.Entry(details).Property(col => col.CarSituation).IsModified = true;
                    //db.Entry(details).Property(col => col.EconomicConcept).IsModified = true;
                    break;

                case OkbConstants.ProfileDetailSections.Lifestyle:
                    currDetails.Smoke = details.Smoke;
                    currDetails.Drink = details.Drink;
                    currDetails.Exercise = details.Exercise;
                    currDetails.Eating = details.Eating;
                    currDetails.Shopping = details.Shopping;
                    currDetails.Religion = details.Religion;
                    currDetails.SleepSchedule = details.SleepSchedule;
                    currDetails.SocialCircle = details.SocialCircle;
                    currDetails.MostMoney = details.MostMoney;
                    currDetails.Housework = details.Housework;
                    currDetails.LovePets = details.LovePets;
                    currDetails.HavePets = details.HavePets;
                    break;

                case OkbConstants.ProfileDetailSections.Job:
                    currDetails.Job = details.Job;
                    currDetails.Industry = details.Industry;
                    currDetails.WorkHours = details.WorkHours;
                    currDetails.CareerAndFamily = details.CareerAndFamily;
                    break;

                case OkbConstants.ProfileDetailSections.Appearance:
                    currDetails.BodyType = details.BodyType;
                    currDetails.FaceType = details.FaceType;
                    currDetails.EyeColor = details.EyeColor;
                    currDetails.EyeShape = details.EyeShape;
                    currDetails.HairColor = details.HairColor;
                    currDetails.HairLength = details.HairLength;
                    currDetails.HairType = details.HairType;
                    currDetails.SkinType = details.SkinType;
                    currDetails.Muscle = details.Muscle;
                    currDetails.HealthCondition = details.HealthCondition;
                    currDetails.DressStyle = details.DressStyle;
                    break;

                case OkbConstants.ProfileDetailSections.Personality:
                    currDetails.Sociability = details.Sociability;
                    currDetails.Humour = details.Humour;
                    currDetails.Temper = details.Temper;
                    currDetails.Feelings = details.Feelings;
                    break;

                default:
                    //invalid section. fail silently?
                    break;
            }

            db.SaveChanges();
        }

        public Profile GetProfile(string userId)
        {
            var db = new OkbDbContext();
            var user = db.Users.Find(userId);
            return db.Profiles.Find(user.ProfileId);
        }
    }
}
