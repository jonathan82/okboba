using Newtonsoft.Json;
using okboba.Entities;
using okboba.Seeder;
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
using okboba.Repository.RedisRepository;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApp
{
    [Serializable]
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public Company Company { get; set; }
    }

    [Serializable]
    class Company
    {
        public int CompId { get; set; }
        public string CompName { get; set; }
    }

    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //////////////////// Seed the database ///////////////////////
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SeedDb seeder = new SeedDb(connString);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("你好");

            seeder.SeedOkbQuestions("../../../data/okb_1.txt");

            //Stopwatch timer = new Stopwatch();

            //Profile Detail Options   
            //seeder.SeedDetailOptions("../../../data/profile_details.txt");

            //// Questions
            //Console.WriteLine("Seeding Questions...");
            //seeder.SeedOkcQuestions("../../../data/okc_questions.txt");

            // Chinese Cities
            //Console.WriteLine("Seeding Locations...");
            //seeder.SeedLocations("../../../data/china_cities.txt");

            // Users
            //const int numOfUsers = 1000;
            //Console.WriteLine("Seeding {0} users...", numOfUsers);
            //timer.Start();
            //seeder.SeedUsers(numOfUsers, EntityLocationRepository.Instance.GetProvinces());
            //timer.Stop();
            //Console.WriteLine("{0} s",timer.ElapsedMilliseconds / 1000);

            // User answers
            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //seeder.SeedAnswers(500, 200);
            //timer.Stop();
            //Console.WriteLine("Total time for seeding answers: " + timer.ElapsedMilliseconds / 1000 + "s ");

            //Activity feed
            //seeder.SeedActivities(1000);


            //Pause so screen won't go away
            Console.WriteLine("done!");
            Console.ReadKey();

        }
    }
}
