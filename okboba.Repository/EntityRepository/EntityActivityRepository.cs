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

        public void AnsweredQuestionActivity(int who, string quesText, string choiceText)
        {
            var db = new OkbDbContext();
            db.ActivityFeed.Add(new Activity
            {
                Who = who,
                CategoryId = (int)OkbConstants.ActivityCategories.AnsweredQuestion,
                Field1 = Truncate(quesText, OkbConstants.FEED_BLURB_SIZE),
                Field2 = Truncate(choiceText, OkbConstants.FEED_BLURB_SIZE),
                Timestamp = DateTime.Now
            });
            db.SaveChanges();
        }

        public void EditProfileTextActivity(int who, string what)
        {
            var db = new OkbDbContext();
            var act = new Activity
            {
                Who = who,
                CategoryId = (int)OkbConstants.ActivityCategories.EditedProfileText,
                Field1 = Truncate(what, OkbConstants.FEED_BLURB_SIZE),
                Timestamp = DateTime.Now
            };

            db.ActivityFeed.Add(act);
            db.SaveChanges();
        }

        public IEnumerable<ActivityModel> GetActivities(int numOfActivities)
        {
            //get the top N activities
            var db = new OkbDbContext();

            var result = from activity in db.ActivityFeed.AsNoTracking()
                         join profile in db.Profiles.AsNoTracking()
                         on activity.Who equals profile.Id
                         orderby activity.Timestamp descending
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
                CategoryId = (int)OkbConstants.ActivityCategories.Joined,
                Field1 = "",
                Timestamp = DateTime.Now
            });
            db.SaveChanges();
        }

        public void UploadPhotoActivity(int who, string what)
        {
            var db = new OkbDbContext();
            db.ActivityFeed.Add(new Activity
            {
                Who = who,
                CategoryId = (int)OkbConstants.ActivityCategories.UploadedPhoto,
                Field1 = what,
                Timestamp = DateTime.Now
            });
            db.SaveChanges();
        }

        public IList<Profile> GetActiveUsers()
        {
            var db = new OkbDbContext();
            var loginThreshold = DateTime.Now.AddHours(-OkbConstants.ACTIVE_USER_INTERVAL);
            var query = from user in db.Users
                        where user.LastLoginDate > loginThreshold
                        join profile in db.Profiles.AsNoTracking() on user.ProfileId equals profile.Id
                        orderby user.LastLoginDate descending
                        select profile;

            return query.ToList();
        }
    }
}
