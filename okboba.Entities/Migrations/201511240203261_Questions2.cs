namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Questions2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.QuestionChoices", "QuestionId");
            AddForeignKey("dbo.QuestionChoices", "QuestionId", "dbo.Questions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionChoices", "QuestionId", "dbo.Questions");
            DropIndex("dbo.QuestionChoices", new[] { "QuestionId" });
        }
    }
}
