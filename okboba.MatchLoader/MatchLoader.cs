using okboba.Entities.Helpers;
using System.Linq;
using okboba.MatchLoader.Models;
using System.Collections.Generic;

namespace okboba.MatchLoader
{
    class MatchLoader
    {
        public List<UserInMem> UsersInMem { get; set; }

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
                select new UserInMem
                {
                    Id = user.Id,
                    Name = user.Name,
                };

            foreach (var user in query)
            {
                UsersInMem.Add(user);
            }
        }

    }
}
