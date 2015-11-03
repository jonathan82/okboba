using Newtonsoft.Json;
using okboba.Entities;
using okboba.Entities.Helpers;
using okboba.Repository;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

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
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=okboba;Integrated Security=True";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

            var x = new RedisManagerPool("localhost");
            var client = x.GetClient().As<Person>();
            client.RemoveEntry("mylist");

            var mylist = client.Lists["mylist"];
            
            for(int i=0; i < 10; i++)
            {
                mylist.Add(new Person
                {
                    Id = client.GetNextSequence(),
                    Name = "jonahtan",
                    Age = 33,
                    Description = "cool guy"
                });
            }

            var client2 = x.GetClient();
            var jsonList = client2.GetRangeFromList("mylist", 1, 4);

            foreach (var p in jsonList)
            {
                Console.WriteLine(p);
            }

            Console.WriteLine("list length: {0}", client2.GetListCount("mylist"));

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
            //SeedDb db = new SeedDb(connString);
            Stopwatch timer = new Stopwatch();

            //// Questions
            //Console.WriteLine("Seeding Questions...");
            //db.SeedOkcQuestions("../../../Design/okc_questions.txt");

            //// Chinese Cities
            //Console.WriteLine("Seeding Locations...");
            //db.SeedLocations("../../data/china_cities.txt");

            //// Users
            //Console.WriteLine("Seeding Users...");
            //db.SeedUsers(100000, LocationRepository.Instance.GetProvinceList());

            //// User answers
            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //db.SeedAnswers(100000, 200);
            //timer.Stop();
            //Console.WriteLine("Total time for seeding answers: " + timer.ElapsedMilliseconds / 1000 + "s ");

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
