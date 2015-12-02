using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataUtilities;
using System.Data;
using okboba.Entities;

namespace okboba.Entities.Helpers
{
    public class UserProfileBulkDataReader : BulkDataReader
    {
        int rowCount = 0;
        int numOfUsers;
        string schemaName;
        string tableName;

        string[] maleNames = { "EnochGaylord", "Jose", "Hilton", "Damon", "Elvin", "Wilbert", "Hugo", "Allen", "Dudley", "Kent", "Roman", "Dion", "Don",
            "Ned", "Blaine", "Cary", "Brady", "Jefferey", "Pasquale", "Thanh", "Bruno", "Dustin", "Ulysses", "Wilber", "Jay", "Irving", "Jacinto", "Kristofer",
            "Gonzalo", "Jimmy", "Ezekiel", "Mariano", "Wilfredo", "Theron", "Jimmie", "Roland", "Jon", "Wilmer", "Nigel", "Josue", "Palmer", "Clifford", "Lance",
            "Scot", "Ezequiel", "Shawn", "Archie", "Chase", "Daron", "Chance", "Von", "Renato", "Newton", "Donald", "Jaime", "Antony", "Booker", "Jose", "Roosevelt",
            "Jack", "Harlan", "Hilario", "Milan", "Filiberto", "Curt", "Johnie", "Jamel", "Alejandro", "Bennie", "Nestor", "Al", "Karl", "Vern", "Lazaro", "Roderick",
            "Emil", "Salvatore", "Edwin", "Pedro", "Ray", "Maurice", "Wallace", "Eugene", "Harry", "Michale", "Guy", "Santos", "Blair", "Boyd", "Brock", "Quintin",
            "Cyril", "James", "Edmundo", "Ronny", "Anderson", "Mauricio", "Elias", "Darin" };

        string[] femaleNames = { "Alisa", "Sunday", "Molly", "Mireya", "Keira", "Arnette", "Aretha", "Arnita", "Cherryl", "Mana", "Chu", "Breanna", "Susanne", "Elva",
            "Norma", "Debbra", "Slyvia", "Julee", "Rae", "Dorene", "Candie", "Novella", "Natisha", "Dominga", "Reta", "Venetta", "Amiee", "Willette", "Marcelle",
            "Rosanna", "Odelia", "Adrianne", "Shalonda", "Iliana", "Angelia", "Maude", "Tracee", "Suanne", "Shantel", "Eve", "Niesha", "Christy", "Hollie", "Shay",
            "Salley", "Berna", "Emmy", "Nadine", "Clarinda", "Ashlie", "Willetta", "Berenice", "Vera", "Elease", "Josette", "Lori", "Leia", "Robena", "Coletta",
            "Waltraud", "Margeret", "Annie", "Laveta", "Victoria", "Chanda", "Cherly", "Hee", "Valentina", "Teri", "Holley", "Betsy", "Shakia", "Raelene", "Mae",
            "Mi", "Rosaria", "Lessie", "Clotilde", "Clelia", "Darleen", "Shayna", "Grazyna", "Porsche", "Malinda", "Lenita", "Star", "Deja", "Catina", "Arlene",
            "Sharron", "Lenna", "Gwyn", "Allegra", "Leeann", "Dora", "Maudie", "Gemma", "Michell", "Kenia", "Lucia" };

        Profile randomProfile;
        Random random;
        List<Location> provinces;

        public UserProfileBulkDataReader(int numOfUsers, string schemaName, string tableName, List<Location> provinces)
        {
            this.numOfUsers = numOfUsers;
            this.schemaName = schemaName;
            this.tableName = tableName;
            this.random = new Random();
            this.randomProfile = new Profile();
            this.provinces = provinces;
        }

        protected override string SchemaName
        {
            get
            {
                return schemaName;
            }
        }

        protected override string TableName
        {
            get
            {
                return tableName;
            }
        }


        public override object GetValue(int i)
        {
            switch (i)
            {
                case 0:
                    return randomProfile.Nickname;
                case 1:
                    return randomProfile.Birthdate;
                case 2:
                    return randomProfile.Gender;
                case 3:
                    return randomProfile.LocationId1;
                case 4:
                    return randomProfile.LocationId2;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            //Generate a new random profile
            if(random.Next() % 2 == 0)
            {
                //Male
                randomProfile.Gender = 1;
                randomProfile.Nickname = maleNames[random.Next() % maleNames.Length];                
            }
            else
            {
                //Female
                randomProfile.Gender = 2;
                randomProfile.Nickname = femaleNames[random.Next() % maleNames.Length];
            }

            randomProfile.Birthdate = DateTime.Now.AddYears(-random.Next(16, 99));

            //20% chance in beijing
            if(random.Next() % 5 == 0)
            {
                randomProfile.LocationId1 = 1;
            }
            else
            {
                randomProfile.LocationId1 = provinces[random.Next() % provinces.Count].LocationId1;
            }            
            randomProfile.LocationId2 = 1;

            return rowCount++ < numOfUsers;
        }

        protected override void AddSchemaTableRows()
        {
            AddSchemaTableRow("Nickname", 20, null, null, false, false, false, SqlDbType.NVarChar, null, null, null, null, null);
            AddSchemaTableRow("Birthdate", null, null, null, false, false, false, SqlDbType.Date, null, null, null, null, null);
            AddSchemaTableRow("Gender", 1, null, null, false, false, false, SqlDbType.NChar, null, null, null, null, null);
            AddSchemaTableRow("LocationId1", null, null, null, false, false, false, SqlDbType.SmallInt, null, null, null, null, null);
            AddSchemaTableRow("LocationId2", null, null, null, false, false, false, SqlDbType.SmallInt, null, null, null, null, null);
        }
    }
}