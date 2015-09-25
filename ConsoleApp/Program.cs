using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.MatchApi.Helpers;
using System.Data.SqlClient;
using System.Diagnostics;
using okboba.MatchApi.Models;

namespace ConsoleApp
{
    class Program
    {
        private static int NUM_OF_USERS = 500000;
        private static int NUM_OF_QUES_PER_USER = 10;
        private static int NUM_OF_USERS_ANSWERED = 1000;
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=okboba;Integrated Security=True";

        static void Main(string[] args)
        {
            SeedDb db = new SeedDb(connString);
            db.SeedQuestions(500);

            //var timer = new Stopwatch();

            /////////////////// Seed Users ///////////////////////////
            //UserProfileBulkDataReader profileReader = new UserProfileBulkDataReader(NUM_OF_USERS, "", "UserProfiles");

            //using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
            //    SqlBulkCopyOptions.TableLock |
            //    SqlBulkCopyOptions.UseInternalTransaction))
            //{
            //    sbc.BatchSize = 1000;
            //    sbc.DestinationTableName = "UserProfiles";

            //    foreach(var col in profileReader.ColumnMappings)
            //    {
            //        sbc.ColumnMappings.Add(col);
            //    }

            //    Console.WriteLine("Starting insert into UserProfiles...");

            //    timer.Start();
            //    sbc.WriteToServer(profileReader);
            //    timer.Stop();
            //}

            //Console.WriteLine(NUM_OF_USERS / (timer.ElapsedMilliseconds / 1000) + " records / s");
            //Console.WriteLine("Total Time: " + timer.ElapsedMilliseconds / 1000 + " s");
            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            /////////////////// Seed User Answers ///////////////////////////
            //UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(NUM_OF_USERS_ANSWERED, NUM_OF_QUES_PER_USER, "", "UserAnswers");

            //using (SqlBulkCopy sbc = new SqlBulkCopy(connString,
            //    SqlBulkCopyOptions.TableLock |
            //    SqlBulkCopyOptions.UseInternalTransaction))
            //{
            //    sbc.BatchSize = 4000;
            //    sbc.DestinationTableName = "UserAnswers";

            //    foreach (var col in bulkReader.ColumnMappings)
            //    {
            //        sbc.ColumnMappings.Add(col);
            //    }

            //    Console.WriteLine("Starting insert in UserAnswers...");

            //    timer.Reset();
            //    timer.Start();
            //    sbc.WriteToServer(bulkReader);
            //    timer.Stop();
            //}
                    
            //Console.WriteLine((NUM_OF_QUES_PER_USER * NUM_OF_USERS_ANSWERED) / (timer.ElapsedMilliseconds / 1000) + " records / s");
            //Console.WriteLine("Total Time: " + timer.ElapsedMilliseconds / 1000 + " s");
            //Console.ReadKey();
        }
    }
}
