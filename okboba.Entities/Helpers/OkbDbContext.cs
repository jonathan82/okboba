using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace okboba.Entities.Helpers
{
    public class OkbDbContext : DbContext
    {
        public OkbDbContext() 
            : base("DefaultConnection")
        {

        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

    }
}