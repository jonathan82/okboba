using okboba.MatchApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.MatchLoader
{
    class MatchLoader
    {
        public void LoadInitial()
        {
            // scan the database and load all the records into memory 
            OkbDbContext db = new OkbDbContext();

            var query =
                from user in db.UserProfiles
                join ans in db.UserAnswers
                on user.Id equals ans.UserProfileId into uGroup
                from ans in uGroup.DefaultIfEmpty()
                orderby user.Id
                select new
                {
                    Id = user.Id,
                    Name = user.Name,
                    QuestionId = ans.QuestionId
                };

            foreach (var item in query)
            {

            }

        }

    }
}
