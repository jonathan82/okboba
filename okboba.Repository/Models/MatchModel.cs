using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.Models
{
    [Serializable]
    [JsonObject]
    public class MatchModel
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int MatchPercent { get; set; }
        public int FriendPercent { get; set; }
        public int EnemyPercent { get; set; }
        public string Photo { get; set; }
        public byte Gender { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }
    }
}
