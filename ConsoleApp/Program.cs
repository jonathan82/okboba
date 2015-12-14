using Newtonsoft.Json;
using okboba.Entities;
using okboba.Entities.Helpers;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApp
{
    class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
    }

    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //////////////////// Mail ///////////////////////////
            var msg = new MailMessage("jonlin82@gmail.com", "jonlin82@gmail.com", "hello boba","hello boba");
            var smtp = new SmtpClient();
            smtp.Host = "smtp.1and1.com";
            smtp.EnableSsl = true;
            var cred = new NetworkCredential("jonlin82@gmail.com", "8da3m0nw1");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            smtp.Send(msg);

            //////////////////// Test Messages /////////////////////////
            //var repo = EntityMessageRepository.Instance;

            //// Add a bunch of messages between me and multiple users
            //repo.AddMessageAsync(100, 1000, "hello world").Wait();


            //var client = new RedisManagerPool("localhost").GetClient().As<Person>();
            //var key = "mykey";
            //client.RemoveEntry(key);
            //client.Lists[key].Add(new Person {Id = 3, Name = "jon", Age = 33, Description = "fdsads" });
            //client.ExpireIn(key, new TimeSpan(0, 5, 0));

            //////////////////// Test the match loader /////////////////
            //MatchLoader matches = new MatchLoader();
            //Stopwatch timer = new Stopwatch();

            //Console.WriteLine("Loading...");
            //timer.Start();
            //matches.LoadInitial();
            //timer.Stop();
            //Console.WriteLine("{0} users loaded", matches.UsersInMem.Count);
            //Console.WriteLine("{0} sec", timer.ElapsedMilliseconds / 1000);

            //while (true)
            //{
            //    Console.WriteLine("scanning...");
            //    timer.Reset();
            //    timer.Start();
            //    matches.ScanAll();
            //    timer.Stop();
            //    Console.WriteLine("{0} ms", timer.ElapsedMilliseconds);
            //    Console.ReadKey();
            //}

            //////////////////// Seed the database ///////////////////////
            //var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SeedDb seeder = new SeedDb(connString);
            //Stopwatch timer = new Stopwatch();

            //Profile Detail Options   
            //seeder.SeedDetailOptions("../../../data/profile_details.txt");         

            //// Questions
            //Console.WriteLine("Seeding Questions...");
            //seeder.SeedOkcQuestions("../../../data/okc_questions.txt");

            // Chinese Cities
            //Console.WriteLine("Seeding Locations...");
            //seeder.SeedLocations("../../../data/china_cities.txt");

            //// Users
            //Console.WriteLine("Seeding Users...");
            //seeder.SeedUsers(10000, EntityLocationRepository.Instance.GetProvinceList());

            // User answers
            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //seeder.SeedAnswers(10000, 100);
            //timer.Stop();
            //Console.WriteLine("Total time for seeding answers: " + timer.ElapsedMilliseconds / 1000 + "s ");

            //Activity feed
            //seeder.SeedActivities(1000);

            //////////////////////// Simulations //////////////////////////
            //var seed = new SeedDb(connString);
            //var rand = new Random();
            //var provinces = LocationRepository.Instance.GetProvinceList();
            //string[] genders = { "M", "F" };

            //Console.WriteLine("Caching answers in memory...");
            //timer.Start();
            //var cachedAnswers = seed.CacheAnswers();
            //timer.Stop();
            //Console.WriteLine("{0} sec to cache", timer.ElapsedMilliseconds / 1000);

            //for (int i = 0; i < 10; i++)
            //{
            //    var loc1 = provinces[rand.Next() % provinces.Count].LocationId1;
            //    var gender = genders[rand.Next() % 2];

            //    timer.Restart();
            //    //var matches = seed.SimulateMatchSearch(500, gender, loc1);
            //    //var matches = seed.SimulateMatchSearchCache(500, gender, loc1, cachedAnswers);
            //    var matches = seed.SimulateMatchSearchNoJoin(500, gender, loc1);
            //    timer.Stop();

            //    Console.WriteLine("Found {1} matches in {0} ms", timer.ElapsedMilliseconds, matches.Count);

            //    //Print out first 10 matches
            //    int cnt = 0;
            //    foreach (var m in matches)
            //    {
            //        if (cnt++ > 10) break;
            //        Console.Write("{0}-{1}% ", m.Name, m.PercentageMatch);
            //    }

            //    Console.WriteLine();
            //}

            ///////////////// Simulate answering questions //////////////////////            
            //const int NUM_OF_QUES_TO_ANSWER = 1000;
            //timer.Start();
            //for(int i=0; i < NUM_OF_QUES_TO_ANSWER; i++)
            //{
            //    seed.SimulateAnsweringQuestion(rand);
            //}
            //timer.Stop();
            //Console.WriteLine("answered 1000 questions in {0} ms, {1} ms / ques", timer.ElapsedMilliseconds, timer.ElapsedMilliseconds / NUM_OF_QUES_TO_ANSWER);

            //Pause so screen won't go away
            Console.WriteLine("done!");
            Console.ReadKey();

        }
    }
}
