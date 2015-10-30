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
            //db.SeedUsers(200000, LocationRepository.Instance.GetProvinceList());

            //// User answers
            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //db.SeedAnswers(200000, 200);
            //timer.Stop();
            //Console.WriteLine("Total time for seeding answers: " + timer.ElapsedMilliseconds / 1000 + "s ");

            //////////////////////// Simulations //////////////////////////
            var seed = new SeedDb(connString);
            timer.Start();
            seed.SimulateMatchSearch(500, "F");
            timer.Stop();
            Console.WriteLine("Match search took {0} ms", timer.ElapsedMilliseconds);

            //Pause so screen won't go away
            Console.WriteLine("done!");
            Console.ReadKey();

        }
    }
}
