namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfileText : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfileTexts",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        Question1 = c.String(),
                        Question2 = c.String(),
                        Question3 = c.String(),
                        Question4 = c.String(),
                        Question5 = c.String(),
                    })
                .PrimaryKey(t => t.ProfileId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfileTexts", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ProfileTexts", new[] { "ProfileId" });
            DropTable("dbo.ProfileTexts");
        }
    }
}
