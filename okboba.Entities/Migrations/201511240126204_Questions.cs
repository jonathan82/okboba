namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Questions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionChoices",
                c => new
                    {
                        QuestionId = c.Short(nullable: false),
                        Index = c.Byte(nullable: false),
                        Text = c.String(maxLength: 50),
                        Score = c.Short(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuestionId, t.Index });
            
            DropColumn("dbo.Questions", "ChoicesInternal");
            DropColumn("dbo.Questions", "TraitScores");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "TraitScores", c => c.Binary(maxLength: 10));
            AddColumn("dbo.Questions", "ChoicesInternal", c => c.String());
            DropTable("dbo.QuestionChoices");
        }
    }
}
