using Newtonsoft.Json;
using okboba.Entities;
using okboba.Entities.Helpers;
using okboba.MatchLoader;
using okboba.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(Watch =true)]

namespace ConsoleApp
{


    class Program
    {
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=okboba;Integrated Security=True";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

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
            var seed = new SeedDb(connString);
            var rand = new Random();
            var provinces = LocationRepository.Instance.GetProvinceList();
            string[] genders = { "M", "F" };

            Console.WriteLine("Caching answers in memory...");
            timer.Start();
            var cachedAnswers = seed.CacheAnswers();
            timer.Stop();
            Console.WriteLine("{0} sec to cache", timer.ElapsedMilliseconds / 1000);

            for (int i = 0; i < 10; i++)
            {
                var loc1 = provinces[rand.Next() % provinces.Count].LocationId1;
                var gender = genders[rand.Next() % 2];
                timer.Restart();
                //var matches = seed.SimulateMatchSearch(500, gender, loc1);
                var matches = seed.SimulateMatchSearchCache(500, gender, loc1, cachedAnswers);
                timer.Stop();
                Console.WriteLine("Found {1} matches in {0} ms", timer.ElapsedMilliseconds, matches.Count);
                for(int j=0; j < 10; j++)
                {
                    Console.Write("{0}-{1}% ", matches[j].Name, matches[j].PercentageMatch);
                }
                Console.WriteLine();
            }

            //Pause so screen won't go away
            Console.WriteLine("done!");
            Console.ReadKey();

        }
    }
}
