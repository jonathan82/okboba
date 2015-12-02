using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Repository.Models;
using okboba.Resources;

namespace okboba.Repository.EntityRepository
{
    public class EntityActivityRepository : IActivityRepository
    {
        #region Singelton
        private static EntityActivityRepository instance;
        private EntityActivityRepository() { }
        public static EntityActivityRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityActivityRepository();
                }
                return instance;
            }
        }
        #endregion

        private string Truncate(string str, int max)
        {
            return str.Length > max ? str.Substring(0, max) : str;
        }

        public void AnsweredQuestionActivity(int who, string what)
        {
            var db = new OkbDbContext();
            db.ActivityFeed.Add(new Activity
            {
                Who = who,
                CategoryId = (int)ActivityCategories.AnsweredQuestion,
                What = Truncate(what, OkbConstants.FEED_BLURB_SIZE),
                When = DateTime.Now
            });
            db.SaveChanges();
        }

        public void EditProfileTextActivity(int who, string what)
        {
            var db = new OkbDbContext();
            var act = new Activity
            {
                Who = who,
                CategoryId = (int)ActivityCategories.EditedProfileText,
                What = Truncate(what, OkbConstants.FEED_BLURB_SIZE),
                When = DateTime.Now
            };

            db.ActivityFeed.Add(act);
            db.SaveChanges();
        }

        public IEnumerable<ActivityModel> GetActivities(byte gender, int numOfActivities)
        {
            //get the top N activities
            var db = new OkbDbContext();

            var result = from activity in db.ActivityFeed.AsNoTracking()
                         join profile in db.Profiles.AsNoTracking()
                         on activity.Who equals profile.Id
                         where profile.Gender == gender
                         orderby activity.When descending
                         select new ActivityModel
                         {
                             Activity = activity,
                             Profile = profile
                         };

            var feed = new List<ActivityModel>();

            foreach (var item in result.Take(numOfActivities))
            {
                feed.Add(item);
            }

            return feed;
        }

        public void JoinedActivity(int who)
        {
            var db = new OkbDbContext();
            db.ActivityFeed.Add(new Activity
            {
                Who = who,
                CategoryId = (int)ActivityCategories.Joined,
                What = "",
                When = DateTime.Now
            });
            db.SaveChanges();
        }

        public void UploadPhotoActivity(int who, string what)
        {
            var db = new OkbDbContext();
            db.ActivityFeed.Add(new Activity
            {
                Who = who,
                CategoryId = (int)ActivityCategories.UploadedPhoto,
                What = what,
                When = DateTime.Now
            });
        }
    }
}
