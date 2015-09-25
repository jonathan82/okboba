namespace okboba.MatchApi.Migrations
{
    using Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using okboba.MatchApi.Models;
    using System.Diagnostics;
    using DataUtilities;
    using System.Data.SqlClient;
    using Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<OkbDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(OkbDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();
            var timer = new Stopwatch();

            const int NUM_OF_USERS = 10000;
            const int NUM_OF_QUES = 1000;
            const int BATCH_INSERT_SIZE = 100;
            const int NUM_OF_USERS_ANSWERED = 1000;
            const int NUM_OF_QUES_PER_USER = 100;

            /////////////////////// Seed Users /////////////////////////////////////
            var ctx = new OkbDbContext();
            ctx.Configuration.AutoDetectChangesEnabled = false;

            if(ctx.UserProfiles.Count() == 0 ) 
            {
                timer.Start();

                for (int i = 0; i < NUM_OF_USERS; i++)
                {
                    UserProfile user;

                    if(i%2==0)
                    {
                        user = CreateUser("Jonathan", 'M', new DateTime(1982, 3, 24), "San Francisco");
                    } else
                    {
                        user = CreateUser("Maggie", 'F', new DateTime(1980, 12, 15), "San Francisco");
                    }

                    ctx.UserProfiles.Add(user);

                    if (i % BATCH_INSERT_SIZE == 0)
                    {
                        // Save changes and Dispose context for performance
                        ctx.SaveChanges();
                        ctx.Dispose();
                        ctx = new OkbDbContext();
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                    }
                }

                ctx.SaveChanges();
                ctx.Dispose();

                timer.Stop();
                Console.WriteLine("Elapse time: " + timer.ElapsedMilliseconds / 1000 + " s");
            }

            ///////////////// Seed Questions ///////////////////////////
            ctx = new OkbDbContext();
            ctx.Configuration.AutoDetectChangesEnabled = false;
            
            if(ctx.Questions.Count() == 0)
            {
                var choices = "This is Choice A;This is Choice B;This is Choice C;This is Choice D;This is Choice E";

                for(int i=1; i < NUM_OF_QUES; i++)
                {
                    Question q = CreateQuestion("Wha'ts your favorite color?", choices, i);
                    ctx.Questions.Add(q);
                }
                ctx.SaveChanges();
                ctx.Dispose();
            }

            ///////////////// Seed User Answers ///////////////////////////
            //UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(NUM_OF_USERS_ANSWERED, NUM_OF_QUES_PER_USER, "", "UserAnswers");            
            //SqlBulkCopy sbc = new SqlBulkCopy(context.Database.Connection.ConnectionString);
            //sbc.BatchSize = 1000;
            //sbc.DestinationTableName = "UserAnswers";

            //foreach (var col in bulkReader.ColumnMappings)
            //{
            //    sbc.ColumnMappings.Add(col);
            //}
            //sbc.WriteToServer(bulkReader);

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
