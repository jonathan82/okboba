using okboba.Entities;
using okboba.MatchApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace okboba.MatchApi.Helpers
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

                for (int i = 1; i < numOfQues; i++)
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
            if(context.UserProfiles.Count() > 0)
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

        private UserProfile CreateUser(string name, char gender, DateTime dob, string location)
        {
            return new UserProfile
            {
                Name = name,
                Gender = gender,
                Birthdate = dob,
                Location = location
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