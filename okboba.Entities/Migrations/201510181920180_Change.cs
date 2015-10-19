namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Traits",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Profiles", "CurrentQuestionId", c => c.Short());
            AddColumn("dbo.Questions", "NextQuestionId", c => c.Short());
            RenameColumn("dbo.Questions", "Choices", "ChoicesInternal");
            AddColumn("dbo.Questions", "TraitScores", c => c.Binary(maxLength: 10));
            AddColumn("dbo.Questions", "TraitId", c => c.Short());
            CreateIndex("dbo.Profiles", "CurrentQuestionId");
            CreateIndex("dbo.Questions", "TraitId");
            AddForeignKey("dbo.Questions", "TraitId", "dbo.Traits", "Id");
            AddForeignKey("dbo.Profiles", "CurrentQuestionId", "dbo.Questions", "Id");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "Choices", c => c.String());
            DropForeignKey("dbo.Profiles", "CurrentQuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "TraitId", "dbo.Traits");
            DropIndex("dbo.Questions", new[] { "TraitId" });
            DropIndex("dbo.Profiles", new[] { "CurrentQuestionId" });
            DropColumn("dbo.Questions", "TraitId");
            DropColumn("dbo.Questions", "TraitScores");
            DropColumn("dbo.Questions", "ChoicesInternal");
            DropColumn("dbo.Questions", "NextQuestionId");
            DropColumn("dbo.Profiles", "CurrentQuestionId");
            DropTable("dbo.Traits");
        }
    }
}
