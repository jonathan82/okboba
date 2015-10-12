using okboba.Entities.Helpers;
using System.Linq;
using okboba.MatchLoader.Models;
using System.Collections.Generic;
using System;
using okboba.Entities;

namespace okboba.MatchLoader
{
    public class MatchLoader
    {
        public List<UserInMem> UsersInMem { get; set; }

        public void LoadInitial()
        {
            // scan the database and load all the records into memory 
            OkbDbContext db = new OkbDbContext();
            UsersInMem = new List<UserInMem>();

            var results =
                (from user in db.Profiles
                 join ans in db.Answers
                 on user.Id equals ans.ProfileId into uGroup
                 from ans in uGroup.DefaultIfEmpty()
                 orderby user.Id
                 select new
                 {
                     Id = user.Id,
                     Name = user.Name,
                     Gender = user.Gender,
                     QuestionId = ans.QuestionId,
                     ChoiceIndex = ans.ChoiceIndex

                 }).Take(50000000);

            int currentUserId = 0;
            List<AnswerInMem> currentAnswers = null;

            foreach (var u in results)
            {
                if(u.Id != currentUserId)
                {
                    //Create a new question set
                    currentAnswers = new List<AnswerInMem>();

                    //Add new user
                    UsersInMem.Add(new UserInMem
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Gender = u.Gender[0],
                        Answers = currentAnswers
                    });
                }

                //Add the answer
                currentAnswers.Add(new AnswerInMem
                {
                    QuestionId = u.QuestionId,
                    ChoiceIndex = u.ChoiceIndex
                });

                //Update the current user
                currentUserId = u.Id;
            }

        }

        public void ScanAll()
        {
            int count = 0;

            //Loop thru all the users
            for(int i=0; i < UsersInMem.Count; i++)
            {
                //peform some dummy filter
                if (UsersInMem[i].Gender == 'M')
                    continue;

                //foreach(var a in u.Answers)
                //{
                //    //perform some dummy calculation
                //    count = a.ChoiceIndex & a.ChoiceAcceptable;
                //}
            }
        }

    }
}
