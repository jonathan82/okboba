using Microsoft.AspNet.Identity.EntityFramework;
using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace okboba.Entities
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))] //MySQL
    public class OkbDbContext : IdentityDbContext<OkbobaUser>
    {
        public OkbDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static OkbDbContext Create()
        {
            return new OkbDbContext();
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ProfileText> ProfileTexts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationMap> ConversationMap { get; set; }
    }
}