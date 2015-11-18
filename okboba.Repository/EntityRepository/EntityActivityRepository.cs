using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.EntityRepository
{
    public class EntityActivityRepository : IActivityRepository
    {
        public void AnsweredQuestionActivity(int who, string what)
        {
            throw new NotImplementedException();
        }

        public void EditProfileTextActivity(int who, string what)
        {
            //var db = new OkbDbContext();
            //var act = new Activity
            //{
            //    ProfileId = who,
            //    CategoryId = (int)ActivityCategories.EditedProfileText,
            //    Metadata = what,
            //    When = DateTime.Now
            //};

            //db.ActivityFeed.Add(act);
            //db.SaveChanges();
        }

        public void JoinedActivity(int who)
        {
            throw new NotImplementedException();
        }

        public void UploadPhotoActivity(int who, string what)
        {
            throw new NotImplementedException();
        }
    }
}
