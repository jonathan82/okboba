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

namespace ConsoleApp
{
    class Program
    {
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=okboba;Integrated Security=True";

        static void Main(string[] args)
        {
            //////////////// Test Microsoft Azure Storage /////////////////

            // Get the storage connection string
            var str = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            Console.WriteLine(str);

            //get the filestream            
            var fileStream = File.OpenRead(@"C:\Users\Public\Pictures\Sample Pictures\desert.jpg");            

            //Get the photo repo
            var repo = PhotoRepository.Instance;
            repo.StorageConnectionString = str;
            //repo.UploadPhoto(fileStream, 0, 0, 300, 2002);

            Console.WriteLine("done!");

            
            /////////////////// Json.NET ///////////////////////////
            //OkbDbContext db = new OkbDbContext();

            //Dictionary<Int16, string> provinceDict = new Dictionary<short, string>();
            //Dictionary<Int16, string> districtDict = new Dictionary<short, string>();

            //foreach (var loc in db.Locations)
            //{
            //    //provinceDict.Add(loc.LocationId1, loc.LocationName1);
            //    provinceDict[loc.LocationId1] = loc.LocationName1;
            //    districtDict.Add(loc.LocationId2, loc.LocationName2);                
            //}

            //string str = JsonConvert.SerializeObject(provinceDict);

            //Console.WriteLine(str);

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
            //Stopwatch timer = new Stopwatch();

            //// Questions
            //Console.WriteLine("Seeding Questions...");
            //db.SeedQuestions(500);

            //// Chinese Cities
            //Console.WriteLine("Seeding Locations...");
            //db.SeedLocations("../../data/china_cities.txt");

            //// Users
            //Console.WriteLine("Seeding Users...");
            //db.SeedUsers(500000);

            //// User answers
            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //db.SeedAnswers(400, 100);
            //timer.Stop();
            //Console.WriteLine("Total time for seeding answers: " + timer.ElapsedMilliseconds / 1000 + "s ");

            //Pause so screen won't go away
            Console.ReadKey();
        }
    }
}
