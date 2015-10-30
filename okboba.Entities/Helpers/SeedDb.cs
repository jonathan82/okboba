﻿using okboba.Entities;
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
        const int LITTLE_IMPORTANT = 1;
        const int SOMEWHAT_IMPORTANT = 5;
        const int VERY_IMPORTANT = 25;
        int[] weights = { 0, LITTLE_IMPORTANT, SOMEWHAT_IMPORTANT, VERY_IMPORTANT };

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

        public class Match
        {
            public int ProfileId { get; set; }
            public int PossibleScoreMe { get; set; }
            public int ScoreMe { get; set; }
            public int PossibleScoreThem { get; set; }
            public int ScoreThem { get; set; }
            public string Name { get; set; }
            public int PercentageMatch { get; set; }
        }

        public struct CacheAnswer
        {
            public byte ChoiceIndex;
            public byte ChoiceAccept;
            public byte ChoiceWeight;
            public short QuestionId;
        }

        public Dictionary<int,List<CacheAnswer>> CacheAnswers()
        {
            var db = new OkbDbContext();
            var cache = new Dictionary<int, List<CacheAnswer>>();

            foreach (var ans in db.Answers.AsNoTracking())
            {
                if (!cache.ContainsKey(ans.ProfileId))
                {
                    cache.Add(ans.ProfileId, new List<CacheAnswer>());
                }

                cache[ans.ProfileId].Add(new CacheAnswer
                {
                    QuestionId = ans.QuestionId,
                    ChoiceIndex = ans.ChoiceIndex,
                    ChoiceAccept = ans.ChoiceAcceptable,
                    ChoiceWeight = ans.ChoiceWeight
                });
            }
            return cache;
        }

        private int CalculateMatchPercent(Profile p, Dictionary<int,Answer> myAnswers, Dictionary<int, List<CacheAnswer>> cache)
        {
            int scoreMe = 0,
                scoreThem = 0,
                possibleScoreMe = 0,
                possibleScoreThem = 0;

            if(!cache.ContainsKey(p.Id))
            {
                return 0;
            }

            foreach (var them in cache[p.Id])
            {
                if (!myAnswers.ContainsKey(them.QuestionId)) continue;
                var me = myAnswers[them.QuestionId];
                var meAccept = me.ChoiceAcceptable & them.ChoiceIndex;
                var themAccept = me.ChoiceIndex & them.ChoiceAccept;
                scoreMe += meAccept != 0 ? weights[me.ChoiceWeight] : 0;
                scoreThem += themAccept != 0 ? weights[them.ChoiceWeight] : 0;
                possibleScoreMe += weights[me.ChoiceWeight];
                possibleScoreThem += weights[them.ChoiceWeight];
            }

            float pctMe, pctThem;
            pctMe = (float)scoreMe / (float)possibleScoreMe;
            pctThem = (float)scoreThem / (float)possibleScoreThem;

            return (int)(Math.Sqrt(pctMe * pctThem) * 100);
        }

        public List<Match> SimulateMatchSearchCache(int profileId, string gender, int loc1, Dictionary<int, List<CacheAnswer>> cache)
        {
            var db = new OkbDbContext();
            var matches = new List<Match>();
            var query = from p in db.Profiles.AsNoTracking()
                        where p.Gender == gender && p.LocationId1 == loc1
                        select p;
            var myAnswers = GetUserAnswers(profileId);

            foreach (var p in query)
            {
                var pctMatch = CalculateMatchPercent(p, myAnswers, cache);
                matches.Add(new Match
                {
                    PercentageMatch = pctMatch,
                    Name = p.Name
                });
            }

            return matches;
        }

        public Dictionary<int, Match> SimulateMatchSearch(int profileId, string gender, int loc1)
        {
            int count = 0;
            var db = new OkbDbContext();
            var matches = new Dictionary<int, Match>();
            int[] weights = { 0, LITTLE_IMPORTANT, SOMEWHAT_IMPORTANT, VERY_IMPORTANT };

            var result = (from a in db.Answers
                         join p in db.Profiles on a.ProfileId equals p.Id
                         where p.Gender == gender && p.LocationId1 == loc1
                         select new { Profile = p, Answer = a });

            var myAnswers = GetUserAnswers(profileId);

            foreach (var ans in result)
            {
                count++;
                //calculate match between two users
                if(!matches.ContainsKey(ans.Profile.Id))
                {
                    matches.Add(ans.Profile.Id, new Match());
                }

                if (!myAnswers.ContainsKey(ans.Answer.QuestionId))
                {
                    continue;
                }

                //cache objects
                var me = myAnswers[ans.Answer.QuestionId];
                var them = matches[ans.Profile.Id];

                // do some calculation  
                var meAccept = me.ChoiceAcceptable & ans.Answer.ChoiceIndex;
                var themAccept = me.ChoiceIndex & ans.Answer.ChoiceAcceptable;

                them.ScoreMe += meAccept != 0 ? weights[me.ChoiceWeight] : 0;
                them.ScoreThem += themAccept != 0 ? weights[ans.Answer.ChoiceWeight] : 0;
                them.PossibleScoreMe += weights[me.ChoiceWeight];
                them.PossibleScoreThem += weights[ans.Answer.ChoiceWeight];

            }

            //Calculate the percentages
            foreach (Match m in matches.Values)
            {
                float pctMe, pctThem;
                pctMe = m.ScoreMe / m.PossibleScoreMe;
                pctThem = m.ScoreThem / m.PossibleScoreThem;
                m.PercentageMatch = (int)(Math.Sqrt(pctMe * pctThem) * 100);
            }

            Console.WriteLine("went thru {0} answers", count);

            return matches;
        }

        public List<Profile> SimulateMatchSearchNoAnswer(int profileId, string gender, int loc1)
        {
            var db = new OkbDbContext();
            var matches = new List<Profile>();

            var result = (from p in db.Profiles
                          where p.Gender == gender && p.LocationId1 == loc1
                          select p).Take(15000);

            var myAnswers = GetUserAnswers(profileId);

            foreach (var p in result)
            {
                matches.Add(p);
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