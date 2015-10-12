namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Locations",
            //    c => new
            //        {
            //            LocationId1 = c.Short(nullable: false),
            //            LocationId2 = c.Short(nullable: false),
            //            LocationName1 = c.String(),
            //            LocationName2 = c.String(),
            //        })
            //    .PrimaryKey(t => new { t.LocationId1, t.LocationId2 });
            
            //CreateTable(
            //    "dbo.Questions",
            //    c => new
            //        {
            //            Id = c.Short(nullable: false, identity: true),
            //            Text = c.String(maxLength: 255),
            //            Rank = c.Int(nullable: false),
            //            Choices = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId })
            //    .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId)
            //    .Index(t => t.RoleId);
            
            //CreateTable(
            //    "dbo.UserAnswers",
            //    c => new
            //        {
            //            UserProfileId = c.Int(nullable: false),
            //            QuestionId = c.Short(nullable: false),
            //            ChoiceIndex = c.Byte(nullable: false),
            //            ChoiceWeight = c.Byte(nullable: false),
            //            ChoiceAcceptable = c.Byte(nullable: false),
            //            LastAnswered = c.DateTime(nullable: false, storeType: "smalldatetime"),
            //        })
            //    .PrimaryKey(t => new { t.UserProfileId, t.QuestionId })
            //    .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
            //    .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
            //    .Index(t => t.UserProfileId)
            //    .Index(t => t.QuestionId);
            
            //CreateTable(
            //    "dbo.UserProfiles",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 10),
            //            Birthdate = c.DateTime(nullable: false, storeType: "date"),
            //            Gender = c.String(maxLength: 1),
            //            Height = c.Short(nullable: false),
            //            LocationId1 = c.Short(nullable: false),
            //            LocationId2 = c.Short(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Locations", t => new { t.LocationId1, t.LocationId2 }, cascadeDelete: true)
            //    .Index(t => new { t.LocationId1, t.LocationId2 });
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(maxLength: 256),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(nullable: false, maxLength: 256),
            //            UserProfile_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
            //    .Index(t => t.UserName, unique: true, name: "UserNameIndex")
            //    .Index(t => t.UserProfile_Id);
            
            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserAnswers", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" }, "dbo.Locations");
            DropForeignKey("dbo.UserAnswers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "UserProfile_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" });
            DropIndex("dbo.UserAnswers", new[] { "QuestionId" });
            DropIndex("dbo.UserAnswers", new[] { "UserProfileId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.UserAnswers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Questions");
            DropTable("dbo.Locations");
        }
    }
}
