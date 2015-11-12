using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class ProfileDetailOption
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string ColName { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte Id { get; set; }

        [StringLength(30)]
        public string Value { get; set; }
    }

    public class ProfileDetail
    {
        [Key]
        [ForeignKey("Profile")]        
        public int ProfileId { get; set; }

        public short Height { get; set; }
        public byte Education { get; set; }
        public byte RelationshipStatus { get; set; }
        public byte HaveChildren { get; set; }
        public byte WantChildren { get; set; }
        public byte Nationality { get; set; }
        public byte MonthlyIncome { get; set; }
        public byte LivingSituation { get; set; }
        public byte CarSituation { get; set; }
        // Lifestyle
        public byte Smoke { get; set; }
        public byte Drink { get; set; }
        public byte Exercise { get; set; }
        public byte Eating { get; set; }
        public byte Shopping { get; set; }
        public byte Religion { get; set; }
        public byte SleepSchedule { get; set; }
        public byte SocialCircle { get; set; }
        public byte MostMoney { get; set; }
        public byte Housework { get; set; }
        public byte LovePets { get; set; }
        public byte HavePets { get; set; }
        //Job
        public byte Job { get; set; }
        public byte Industry { get; set; }
        public byte WorkHours { get; set; }
        public byte CareerAndFamily { get; set; }
        //Appearance
        public byte BodyType { get; set; }
        public byte FaceType { get; set; }
        public byte EyeColor { get; set; }
        public byte EyeShape { get; set; }
        public byte HairColor { get; set; }
        public byte HairType { get; set; }
        public byte HairLength { get; set; }
        public byte SkinType { get; set; }
        public byte Muscle { get; set; }
        public byte HealthCondition { get; set; }
        public byte DressStyle { get; set; }
        //Personality
        public byte Sociability { get; set; }
        public byte Humour { get; set; }
        public byte Temper { get; set; }
        public byte Feelings { get; set; }
        public byte LookingFor { get; set; }
        //Finance
        public byte EconomicConcept { get; set; }
        //public byte[] Investments { get; set; }

        //Navigation properties
        public Profile Profile { get; set; }
    }
}
