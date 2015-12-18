using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Seeder
{
    public class ProfileGenerator
    {
        private Random _rand;
        private IList<LocationPinyinModel> _provinces;

        private string[] _maleNames = { "EnochGaylord", "Jose", "Hilton", "Damon", "Elvin", "Wilbert", "Hugo", "Allen", "Dudley", "Kent", "Roman", "Dion", "Don",
            "Ned", "Blaine", "Cary", "Brady", "Jefferey", "Pasquale", "Thanh", "Bruno", "Dustin", "Ulysses", "Wilber", "Jay", "Irving", "Jacinto", "Kristofer",
            "Gonzalo", "Jimmy", "Ezekiel", "Mariano", "Wilfredo", "Theron", "Jimmie", "Roland", "Jon", "Wilmer", "Nigel", "Josue", "Palmer", "Clifford", "Lance",
            "Scot", "Ezequiel", "Shawn", "Archie", "Chase", "Daron", "Chance", "Von", "Renato", "Newton", "Donald", "Jaime", "Antony", "Booker", "Jose", "Roosevelt",
            "Jack", "Harlan", "Hilario", "Milan", "Filiberto", "Curt", "Johnie", "Jamel", "Alejandro", "Bennie", "Nestor", "Al", "Karl", "Vern", "Lazaro", "Roderick",
            "Emil", "Salvatore", "Edwin", "Pedro", "Ray", "Maurice", "Wallace", "Eugene", "Harry", "Michale", "Guy", "Santos", "Blair", "Boyd", "Brock", "Quintin",
            "Cyril", "James", "Edmundo", "Ronny", "Anderson", "Mauricio", "Elias", "Darin" };

        private string[] _femaleNames = { "Alisa", "Sunday", "Molly", "Mireya", "Keira", "Arnette", "Aretha", "Arnita", "Cherryl", "Mana", "Chu", "Breanna", "Susanne", "Elva",
            "Norma", "Debbra", "Slyvia", "Julee", "Rae", "Dorene", "Candie", "Novella", "Natisha", "Dominga", "Reta", "Venetta", "Amiee", "Willette", "Marcelle",
            "Rosanna", "Odelia", "Adrianne", "Shalonda", "Iliana", "Angelia", "Maude", "Tracee", "Suanne", "Shantel", "Eve", "Niesha", "Christy", "Hollie", "Shay",
            "Salley", "Berna", "Emmy", "Nadine", "Clarinda", "Ashlie", "Willetta", "Berenice", "Vera", "Elease", "Josette", "Lori", "Leia", "Robena", "Coletta",
            "Waltraud", "Margeret", "Annie", "Laveta", "Victoria", "Chanda", "Cherly", "Hee", "Valentina", "Teri", "Holley", "Betsy", "Shakia", "Raelene", "Mae",
            "Mi", "Rosaria", "Lessie", "Clotilde", "Clelia", "Darleen", "Shayna", "Grazyna", "Porsche", "Malinda", "Lenita", "Star", "Deja", "Catina", "Arlene",
            "Sharron", "Lenna", "Gwyn", "Allegra", "Leeann", "Dora", "Maudie", "Gemma", "Michell", "Kenia", "Lucia" };

        public ProfileGenerator(IList<LocationPinyinModel> prov)
        {
            _rand = new Random();
            _provinces = prov;
        }

        public Profile Next()
        {
            //Generate a new random profile
            var profile = new Profile();

            if (_rand.Next() % 2 == 0)
            {
                //Male
                profile.Gender = 1;
                profile.LookingForGender = 2;
                profile.Nickname = _maleNames[_rand.Next() % _maleNames.Length];
            }
            else
            {
                //Female
                profile.Gender = 2;
                profile.LookingForGender = 1;
                profile.Nickname = _femaleNames[_rand.Next() % _maleNames.Length];
            }

            profile.Birthdate = DateTime.Now.AddYears(-_rand.Next(16, 99));

            //20% chance in beijing
            if (_rand.Next() % 5 == 0)
            {
                profile.LocationId1 = 1;
            }
            else
            {
                profile.LocationId1 = (short)_provinces[_rand.Next() % _provinces.Count()].LocationId;
            }

            profile.LocationId2 = 1;

            return profile;
        }
    }
}
