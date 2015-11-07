namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Answers",
            //    c => new
            //        {
            //            ProfileId = c.Int(nullable: false),
            //            QuestionId = c.Short(nullable: false),
            //            ChoiceIndex = c.Byte(),
            //            ChoiceWeight = c.Byte(),
            //            ChoiceAcceptable = c.Byte(),
            //            LastAnswered = c.DateTime(nullable: false, storeType: "smalldatetime"),
            //        })
            //    .PrimaryKey(t => new { t.ProfileId, t.QuestionId })
            //    .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
            //    .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
            //    .Index(t => t.ProfileId)
            //    .Index(t => t.QuestionId);
            
            //CreateTable(
            //    "dbo.Profiles",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 20),
            //            Birthdate = c.DateTime(nullable: false, storeType: "date"),
            //            Gender = c.String(maxLength: 1),
            //            Height = c.Short(nullable: false),
            //            LocationId1 = c.Short(nullable: false),
            //            LocationId2 = c.Short(nullable: false),
            //            PhotosInternal = c.String(maxLength: 90),
            //            CurrentQuestionId = c.Short(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Questions", t => t.CurrentQuestionId)
            //    .ForeignKey("dbo.Locations", t => new { t.LocationId1, t.LocationId2 }, cascadeDelete: true)
            //    .Index(t => new { t.LocationId1, t.LocationId2 })
            //    .Index(t => t.CurrentQuestionId);
            
            //CreateTable(
            //    "dbo.Questions",
            //    c => new
            //        {
            //            Id = c.Short(nullable: false, identity: true),
            //            Text = c.String(maxLength: 255),
            //            Rank = c.Int(nullable: false),
            //            ChoicesInternal = c.String(),
            //            TraitScores = c.Binary(maxLength: 10),
            //            TraitId = c.Short(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Traits", t => t.TraitId)
            //    .Index(t => t.TraitId);
            
            //CreateTable(
            //    "dbo.Traits",
            //    c => new
            //        {
            //            Id = c.Short(nullable: false, identity: true),
            //            Name = c.String(maxLength: 30),
            //            Description = c.String(maxLength: 100),
            //        })
            //    .PrimaryKey(t => t.Id);
            
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
            //    "dbo.ProfileTexts",
            //    c => new
            //        {
            //            ProfileId = c.Int(nullable: false),
            //            Question1 = c.String(),
            //            Question2 = c.String(),
            //            Question3 = c.String(),
            //            Question4 = c.String(),
            //            Question5 = c.String(),
            //        })
            //    .PrimaryKey(t => t.ProfileId)
            //    .ForeignKey("dbo.Profiles", t => t.ProfileId)
            //    .Index(t => t.ProfileId);
            
            //CreateTable(
            //    "dbo.ConversationMaps",
            //    c => new
            //        {
            //            ProfileId = c.Int(nullable: false),
            //            ConversationId = c.Int(nullable: false),
            //            ToProfileId = c.Int(),
            //            ToPhoto = c.String(maxLength: 10),
            //            ToName = c.String(maxLength: 10),
            //            HasBeenRead = c.Boolean(nullable: false),
            //            HasReplies = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => new { t.ProfileId, t.ConversationId })
            //    .ForeignKey("dbo.Conversations", t => t.ConversationId, cascadeDelete: true)
            //    .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
            //    .ForeignKey("dbo.Profiles", t => t.ToProfileId)
            //    .Index(t => t.ProfileId)
            //    .Index(t => t.ConversationId)
            //    .Index(t => t.ToProfileId);
            
            //CreateTable(
            //    "dbo.Conversations",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Subject = c.String(),
            //            LastMessageDate = c.DateTime(),
            //            LastMessageBlurb = c.String(maxLength: 255),
            //            LastMessageFrom = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Profiles", t => t.LastMessageFrom)
            //    .Index(t => t.LastMessageFrom);
            
            //CreateTable(
            //    "dbo.Messages",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ConversationId = c.Int(nullable: false),
            //            FromProfileId = c.Int(nullable: false),
            //            MessageText = c.String(maxLength: 1000),
            //            Timestamp = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Conversations", t => t.ConversationId, cascadeDelete: true)
            //    .ForeignKey("dbo.Profiles", t => t.FromProfileId, cascadeDelete: true)
            //    .Index(t => t.ConversationId)
            //    .Index(t => t.FromProfileId);
            
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
            //    "dbo.TranslateQuestions",
            //    c => new
            //        {
            //            Id = c.Short(nullable: false, identity: true),
            //            QuesEng = c.String(maxLength: 255),
            //            QuesChin = c.String(maxLength: 255),
            //            Rank = c.Int(nullable: false),
            //            ChoicesInternalChin = c.String(),
            //            ChoicesInternalEng = c.String(),
            //            TraitScores = c.Binary(maxLength: 10),
            //            TraitId = c.Short(),
            //            Delete = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Traits", t => t.TraitId)
            //    .Index(t => t.TraitId);
            
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
            //            Profile_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Profiles", t => t.Profile_Id)
            //    .Index(t => t.UserName, unique: true, name: "UserNameIndex")
            //    .Index(t => t.Profile_Id);
            
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
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TranslateQuestions", "TraitId", "dbo.Traits");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Messages", "FromProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMaps", "ToProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "ConversationId", "dbo.Conversations");
            DropForeignKey("dbo.Conversations", "LastMessageFrom", "dbo.Profiles");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Answers", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ProfileTexts", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", new[] { "LocationId1", "LocationId2" }, "dbo.Locations");
            DropForeignKey("dbo.Profiles", "CurrentQuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "TraitId", "dbo.Traits");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Profile_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TranslateQuestions", new[] { "TraitId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Messages", new[] { "FromProfileId" });
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropIndex("dbo.Conversations", new[] { "LastMessageFrom" });
            DropIndex("dbo.ConversationMaps", new[] { "ToProfileId" });
            DropIndex("dbo.ConversationMaps", new[] { "ConversationId" });
            DropIndex("dbo.ConversationMaps", new[] { "ProfileId" });
            DropIndex("dbo.ProfileTexts", new[] { "ProfileId" });
            DropIndex("dbo.Questions", new[] { "TraitId" });
            DropIndex("dbo.Profiles", new[] { "CurrentQuestionId" });
            DropIndex("dbo.Profiles", new[] { "LocationId1", "LocationId2" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropIndex("dbo.Answers", new[] { "ProfileId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TranslateQuestions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.Conversations");
            DropTable("dbo.ConversationMaps");
            DropTable("dbo.ProfileTexts");
            DropTable("dbo.Locations");
            DropTable("dbo.Traits");
            DropTable("dbo.Questions");
            DropTable("dbo.Profiles");
            DropTable("dbo.Answers");
        }
    }
}
