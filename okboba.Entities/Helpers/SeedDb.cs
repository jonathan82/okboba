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
        public void SeedUsers(int numOfUsers)
        {
            //Skip if users exist
            OkbDbContext context = new OkbDbContext();
            if(context.Profiles.Count() > 0)
            {
                context.Dispose();
                return;
            }
            context.Dispose();

            UserProfileBulkDataReader profileReader = new UserProfileBulkDataReader(numOfUsers, "", "UserProfiles");

            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.BatchSize = 1000;
                sbc.DestinationTableName = "UserProfiles";

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

            UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(numOfUsers, numOfAnswersPerUser, "", "UserAnswers");

            using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.BatchSize = 4000;
                sbc.DestinationTableName = "UserAnswers";

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
                Choices = choices
            };
        }
    }
}