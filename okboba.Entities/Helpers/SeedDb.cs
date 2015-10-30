using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace okboba.Entities.Helpers
{
    public class SeedDb
    {
        private string connString;

        public void SeedTranslateQuestions(List<TranslateQuestion> quesList)
        {
            var db = new OkbDbContext();

            foreach (var q in quesList)
            {
                db.TranslateQuestions.Add(q);                
            }
            var ret = db.SaveChanges();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="connString"></param>
        public SeedDb(string connString)
        {
            this.connString = connString;
        }

        /// <summary>
        /// Seed the questions
        /// </summary>
        /// <param name="numOfQues"></param>
        public void SeedQuestions(int numOfQues)
        {
            using (OkbDbContext context = new OkbDbContext())
            { 
                if (context.Questions.Count() > 0)
                {
                    //Skip if questions exist
                    return;
                }

                var choices = "This is Choice A;This is Choice B;This is Choice C;This is Choice D;This is Choice E";

                for (int i = 1; i <= numOfQues; i++)
                {
                    Question q = CreateQuestion("What's your favorite color?", choices, i);
                    context.Questions.Add(q);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed the Number of users
        /// </summary>
        /// <param name="numOfUsers"></param>
        /// <param name="connString"></param>
        public void SeedUsers(int numOfUsers, List<Location> provinces)
        {
            //Skip if users exist
            OkbDbContext db = new OkbDbContext();
            if(db.Profiles.Count() > 0)
            {
                db.Dispose();
                return;
            }

            UserProfileBulkDataReader profileReader = new UserProfileBulkDataReader(numOfUsers, "", "Profiles", provinces);

            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.BatchSize = 1000;
                sbc.DestinationTableName = "Profiles";

                foreach (var col in profileReader.ColumnMappings)
                {
                    sbc.ColumnMappings.Add(col);
                }

                sbc.WriteToServer(profileReader);
            }
        }

        /// <summary>
        /// Seed the Answers
        /// </summary>
        /// <param name="numOfUsers"></param>
        /// <param name="numOfAnswersPerUser"></param>
        public void SeedAnswers(int numOfUsers, int numOfAnswersPerUser)
        {
            //Skip if answers exist
            OkbDbContext context = new OkbDbContext();
            if (context.Answers.Count() > 0)
            {
                context.Dispose();
                return;
            }
            context.Dispose();

            UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(numOfUsers, numOfAnswersPerUser, "", "Answers");

            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.BatchSize = 4000;
                sbc.DestinationTableName = "Answers";

                foreach (var col in bulkReader.ColumnMappings)
                {
                    sbc.ColumnMappings.Add(col);
                }

                sbc.WriteToServer(bulkReader);
            }
        }        

        public void SeedLocations(string filename)
        {
            OkbDbContext db = new OkbDbContext();
            if(db.Locations.Count() > 0)
            {
                db.Dispose();
                return;
            }

            //Read from file
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                Int16 provinceCount = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    var cities = line.Split(' ');

                    //First entry is the province
                    string province = cities[0];

                    for(Int16 districtCount = 1; districtCount < cities.Length; districtCount++)
                    {
                        string district = cities[districtCount];

                        var loc = new Location { LocationId1 = provinceCount,
                                                 LocationId2 = districtCount,
                                                 LocationName1 = province,
                                                 LocationName2 = district };
                        //Update the database
                        db.Locations.Add(loc);
                        db.SaveChanges();
                    }
                    provinceCount++;
                }
            }            
        }

        public void SeedOkcQuestions(string filename)
        {
            var db = new OkbDbContext();
            if(db.Questions.Count() > 0)
            {
                return;
            }            

            //////////////// Insert Okcupid questions ////////////////////
            var sr = new StreamReader(filename);
            int count = 0;            

            while (!sr.EndOfStream)
            {
                var ques = sr.ReadLine();
                string ans, ansInternal = "";
                while ((ans = sr.ReadLine()) != "" && !sr.EndOfStream)
                {
                    ansInternal += ans + ";";
                }
                //remove trailing semicolon
                ansInternal = ansInternal.TrimEnd(';');

                var addQues = new Question
                {
                    Text = ques,
                    ChoicesInternal = ansInternal,
                    Rank = ++count                    
                };

                db.Questions.Add(addQues);
            }

            db.SaveChanges();
        }

        private Dictionary<int,Answer> GetUserAnswers(int profileId)
        {
            var db = new OkbDbContext();
            var result = from ans in db.Answers
                         where ans.ProfileId == profileId
                         select ans;
            var answers = new Dictionary<int, Answer>();

            foreach (var ans in result)
            {
                answers.Add(ans.QuestionId, ans);                
            }
            return answers;
        }

        public Dictionary<int,Profile> SimulateMatchSearch(int profileId, string gender)
        {
            var db = new OkbDbContext();
            var matches = new Dictionary<int, Profile>();

            var result = (from p in db.Profiles
                         join a in db.Answers on p.Id equals a.ProfileId
                         where p.Gender == gender
                         select new { Profile = p, Answer = a }).Take(20000);

            var myAnswers = GetUserAnswers(profileId);

            foreach (var ans in result)
            {
                //calculate match between two users
                if(!matches.ContainsKey(ans.Profile.Id))
                {
                    matches.Add(ans.Profile.Id, ans.Profile);
                }

                // do some calculation     
                int x = ans.Answer.ChoiceIndex & myAnswers[ans.Answer.QuestionId].ChoiceAcceptable;       
            }
            return matches;
        }

        private Profile CreateUser(string name, string gender, DateTime dob, string location)
        {
            return new Profile
            {
                Name = name,
                Gender = gender,
                Birthdate = dob
            };
        }

        private Question CreateQuestion(string text, string choices, int rank)
        {
            return new Question
            {
                Text = text,
                Rank = rank,
                ChoicesInternal = choices
            };
        }
    }
}