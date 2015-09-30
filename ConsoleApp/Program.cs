﻿using okboba.MatchLoader;
using System;
using System.Diagnostics;

namespace ConsoleApp
{
    class Program
    {
        private static int NUM_OF_USERS = 500000;
        private static int NUM_OF_QUES_PER_USER = 200;
        private static int NUM_OF_QUES = 1000;
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=okboba;Integrated Security=True";

        static void Main(string[] args)
        {
            //////////////////// Test the match loader /////////////////
            MatchLoader matches = new MatchLoader();
            Stopwatch timer = new Stopwatch();

            Console.WriteLine("Loading...");
            timer.Start();
            matches.LoadInitial();
            timer.Stop();
            Console.WriteLine("{0} users loaded", matches.UsersInMem.Count);
            Console.WriteLine("{0} sec", timer.ElapsedMilliseconds / 1000);

            while (true)
            {
                Console.WriteLine("scanning...");
                timer.Reset();
                timer.Start();
                matches.ScanAll();
                timer.Stop();
                Console.WriteLine("{0} ms", timer.ElapsedMilliseconds);
                Console.ReadKey();
            }
            
            Console.WriteLine("Done!");
            Console.ReadKey();

            //////////////////// Seed the database ///////////////////////
            //SeedDb db = new SeedDb(connString);
            //Stopwatch timer = new Stopwatch();

            //db.SeedQuestions(NUM_OF_QUES);
            //db.SeedUsers(NUM_OF_USERS);

            //Console.WriteLine("Seeding answers...");
            //timer.Start();
            //db.SeedAnswers(NUM_OF_USERS, NUM_OF_QUES_PER_USER);
            //timer.Stop();

            //Console.WriteLine("Total time: " + timer.ElapsedMilliseconds / 1000 + "s ");          
            //Console.ReadKey();            
        }
    }
}