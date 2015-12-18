namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Profiles", "CurrentQuestionId", "dbo.Questions");
            DropIndex("dbo.Profiles", new[] { "CurrentQuestionId" });
            AddColumn("dbo.Profiles", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "JoinDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Profiles", "CurrentQuestionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "CurrentQuestionId", c => c.Short());
            DropColumn("dbo.AspNetUsers", "JoinDate");
            DropColumn("dbo.Profiles", "UserId");
            CreateIndex("dbo.Profiles", "CurrentQuestionId");
            AddForeignKey("dbo.Profiles", "CurrentQuestionId", "dbo.Questions", "Id");
        }
    }
}
