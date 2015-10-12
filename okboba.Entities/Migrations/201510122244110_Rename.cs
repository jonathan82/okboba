namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserProfiles", newName: "Profiles");
            RenameTable(name: "dbo.UserAnswers", newName: "Answers");

            RenameColumn(table: "dbo.Answers", name: "UserProfileId", newName: "ProfileId");

            //DropForeignKey("dbo.UserAnswers", "QuestionId", "dbo.Questions");
            //DropForeignKey("dbo.UserAnswers", "UserProfileId", "dbo.UserProfiles");
            //DropIndex("dbo.UserAnswers", new[] { "UserProfileId" });
            //DropIndex("dbo.UserAnswers", new[] { "QuestionId" });
            //CreateTable(
            //    "dbo.Answers",
            //    c => new
            //        {
            //            ProfileId = c.Int(nullable: false),
            //            QuestionId = c.Short(nullable: false),
            //            ChoiceIndex = c.Byte(nullable: false),
            //            ChoiceWeight = c.Byte(nullable: false),
            //            ChoiceAcceptable = c.Byte(nullable: false),
            //            LastAnswered = c.DateTime(nullable: false, storeType: "smalldatetime"),
            //        })
            //    .PrimaryKey(t => new { t.ProfileId, t.QuestionId })
            //    .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
            //    .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
            //    .Index(t => t.ProfileId)
            //    .Index(t => t.QuestionId);
            
            //DropTable("dbo.UserAnswers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserAnswers",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false),
                        QuestionId = c.Short(nullable: false),
                        ChoiceIndex = c.Byte(nullable: false),
                        ChoiceWeight = c.Byte(nullable: false),
                        ChoiceAcceptable = c.Byte(nullable: false),
                        LastAnswered = c.DateTime(nullable: false, storeType: "smalldatetime"),
                    })
                .PrimaryKey(t => new { t.UserProfileId, t.QuestionId });
            
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Answers", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropIndex("dbo.Answers", new[] { "ProfileId" });
            DropTable("dbo.Answers");
            CreateIndex("dbo.UserAnswers", "QuestionId");
            CreateIndex("dbo.UserAnswers", "UserProfileId");
            AddForeignKey("dbo.UserAnswers", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserAnswers", "QuestionId", "dbo.Questions", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.Profiles", newName: "UserProfiles");
        }
    }
}
