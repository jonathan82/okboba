//using okboba.Entities;
//using okboba.Resources;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Web;

//namespace okboba.Entities.Helpers
//{
//    public class SeedDb
//    {
//        private string connString;
//        const int LITTLE_IMPORTANT = 1;
//        const int SOMEWHAT_IMPORTANT = 5;
//        const int VERY_IMPORTANT = 25;
//        int[] weights = { 0, LITTLE_IMPORTANT, SOMEWHAT_IMPORTANT, VERY_IMPORTANT };

//        public void SeedTranslateQuestions(List<TranslateQuestion> quesList)
//        {
//            var db = new OkbDbContext();

//            foreach (var q in quesList)
//            {
//                db.TranslateQuestions.Add(q);                
//            }
//            var ret = db.SaveChanges();
//        }

//        /// <summary>
//        /// Constructor 
//        /// </summary>
//        /// <param name="connString"></param>
//        public SeedDb(string connString)
//        {
//            this.connString = connString;
//        }

//        /// <summary>
//        /// Seed the users by creating a OkbUser and Profile for each user.  Takes a list of provinces
//        /// and randomizes the location of each user.  Uses the UserProfileBulkReader to generate random
//        /// profiles.
//        /// </summary>
//        public void SeedUsers(int numOfUsers, List<Location> provinces, int commitCount = 1000)
//        {
//            var db = new OkbDbContext();            
//            var profileGen = new ProfileGenerator(provinces);

//            db.Configuration.AutoDetectChangesEnabled = false;

//            for (int i = 1; i < numOfUsers; i++)
//            {
//                var email = "test" + i + "@okboba.com";
//                var user = new OkbUser
//                {
//                    UserName = email,
//                    Email = email,
//                    JoinDate = DateTime.Now
//                };

//                var profile = profileGen.Next();

//                user.Profile = profile;
//                profile.UserId = user.Id;

//                db.Users.Add(user);

//                if (i % commitCount == 0)
//                {
//                    db.SaveChanges();
//                    db.Dispose();
//                    db = new OkbDbContext();
//                    db.Configuration.AutoDetectChangesEnabled = false;
//                }
//            }

//            db.SaveChanges();
//        }

//        /// <summary>
//        /// Seed the users by using bulk copy.  Only seeds the profiles but doesn't create a User.
//        /// Takes a list of provinces used to generate random locations for each profile. 
//        /// </summary>
//        public void SeedUsersBulkCopy(int numOfUsers, List<Location> provinces)
//        {
//            //Skip if users exist
//            OkbDbContext db = new OkbDbContext();
//            if(db.Profiles.Count() > 0)
//            {
//                db.Dispose();
//                return;
//            }

//            UserProfileBulkDataReader profileReader = new UserProfileBulkDataReader(numOfUsers, "", "Profiles", provinces);

//            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
//                SqlBulkCopyOptions.TableLock |
//                SqlBulkCopyOptions.UseInternalTransaction))
//            {
//                sbc.BatchSize = 1000;
//                sbc.DestinationTableName = "Profiles";

//                foreach (var col in profileReader.ColumnMappings)
//                {
//                    sbc.ColumnMappings.Add(col);
//                }

//                sbc.WriteToServer(profileReader);
//            }
//        }

//        /// <summary>
//        /// Seed the Answers using SqlBulkCopy for performance.  
//        /// </summary>
//        public void SeedAnswers(int numOfUsers, int numOfAnswersPerUser)
//        {
//            //Skip if answers exist
//            OkbDbContext context = new OkbDbContext();
//            if (context.Answers.Count() > 0)
//            {
//                context.Dispose();
//                return;
//            }
//            context.Dispose();

//            UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(numOfUsers, numOfAnswersPerUser, "", "Answers");

//            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
//                SqlBulkCopyOptions.TableLock |
//                SqlBulkCopyOptions.UseInternalTransaction))
//            {
//                sbc.BatchSize = 4000;
//                sbc.DestinationTableName = "Answers";

//                foreach (var col in bulkReader.ColumnMappings)
//                {
//                    sbc.ColumnMappings.Add(col);
//                }

//                sbc.WriteToServer(bulkReader);
//            }
//        }        

//        /// <summary>
//        /// Seed the Locations from the given file
//        /// </summary>
//        public void SeedLocations(string filename)
//        {
//            OkbDbContext db = new OkbDbContext();
//            if(db.Locations.Count() > 0)
//            {
//                db.Dispose();
//                return;
//            }

//            //Read from file
//            using (StreamReader sr = new StreamReader(filename))
//            {
//                string line;
//                Int16 provinceCount = 1;

//                while ((line = sr.ReadLine()) != null)
//                {
//                    var cities = line.Split(' ');

//                    //First entry is the province
//                    string province = cities[0];

//                    for(Int16 districtCount = 1; districtCount < cities.Length; districtCount++)
//                    {
//                        string district = cities[districtCount];

//                        var loc = new Location { LocationId1 = provinceCount,
//                                                 LocationId2 = districtCount,
//                                                 LocationName1 = province,
//                                                 LocationName2 = district };
//                        //Update the database
//                        db.Locations.Add(loc);
//                        db.SaveChanges();
//                    }
//                    provinceCount++;
//                }
//            }            
//        }

//        /// <summary>
//        /// Seed the Questions from the given file containg Okc Questions
//        /// </summary>
//        public void SeedOkcQuestions(string filename)
//        {
//            var db = new OkbDbContext();
//            if(db.Questions.Count() > 0) return;

//            //////////////// Insert Okcupid questions ////////////////////
//            var stream = new StreamReader(filename);
//            int count = 1;            

//            while (!stream.EndOfStream)
//            {
//                var line = stream.ReadLine();
                
//                int index = 1;
//                string answerLine;

//                //Loop thru answers
//                while ((answerLine = stream.ReadLine()) != "" && !stream.EndOfStream)
//                {
//                    db.QuestionChoices.Add(new QuestionChoice
//                    {
//                        QuestionId = (short)count,
//                        Index = (byte)index,
//                        Score = 0,
//                        Text = answerLine
//                    });

//                    index++;
//                }

//                //Add Question
//                db.Questions.Add(new Question
//                {
//                    Id = (short)count,
//                    Rank = count,
//                    Text = line,
//                    TraitId = null
//                });

//                count++;

//                db.SaveChanges();
//            }
//        }
        
//        /// <summary>
//        /// Seed the detail options from the given file
//        /// </summary>
//        public void SeedDetailOptions(string filename)
//        {
//            var sr = new StreamReader(filename);
//            var colName = "";
//            var details = new List<ProfileDetailOption>();
//            byte id = 0;

//            while (!sr.EndOfStream)
//            {
//                var line = sr.ReadLine();
//                if (line.IndexOf('\t') != -1)
//                {
//                    //detail option
//                    details.Add(new ProfileDetailOption
//                    {
//                        ColName = colName,
//                        Id = id,
//                        Value = line.Trim()
//                    });

//                    id++;
//                }
//                else
//                {
//                    //new detail
//                    colName = line.Split(',')[1].Trim();
//                    id = 1;
//                }
//            }

//            //Dump it out
//            foreach (var option in details)
//            {
//                Console.WriteLine("{0}, {1}, {2}", option.Id, option.ColName, option.Value );
//            }
//        }

//        /// <summary>
//        /// Simulations answering one question to see how fast we can insert answers into the DB.
//        /// Should be called in a loop and given a Random object.
//        /// </summary>
//        public void SimulateAnsweringQuestion(Random rand)
//        {
//            var db = new OkbDbContext();
//            var ans = new Answer
//            {
//                ProfileId = rand.Next(2, 200000),
//                QuestionId = (short)rand.Next(201, 1243),
//                ChoiceBit = 1,
//                ChoiceWeight = 1,
//                ChoiceAcceptable = 1,
//                LastAnswered = DateTime.Now
//            };
            
//            try
//            {
//                db.Answers.Add(ans);
//                db.SaveChanges();
//            }
//            catch (Exception)
//            {

//                throw;
//            }
            
//        }       

//        /// <summary>
//        /// Returns a random time accurate to minutes from 10 days ago to now. Used to seed
//        /// the activities with a random time.
//        /// </summary>
//        private DateTime RandomTime(Random rand)
//        {
//            DateTime start = DateTime.Now.AddDays(-10);
//            var range = (DateTime.Now - start).TotalMinutes;
//            return start.AddMinutes(rand.Next((int)range));
//        }

//        /// <summary>
//        /// Seed the Activity feed with a bunch of activities.
//        /// Can be Joined, AnsweredQuestion, UploadedPhoto, or EditedProfile
//        /// </summary>
//        public void SeedActivities(int n)
//        {
//            var db = new OkbDbContext();
//            var rand = new Random();

//            for (int i = 0; i < n; i++)
//            {
//                db.ActivityFeed.Add(new Activity
//                {
//                    Who = (rand.Next() % 1000) + 1,
//                    CategoryId = (rand.Next() % 4) + 1,
//                    When = RandomTime(rand),
//                    What = "this is some test text for metadata field. lorum ipsum dolor blah blah blah"
//                });
//            }

//            db.SaveChanges();
//        }
//    }
//}