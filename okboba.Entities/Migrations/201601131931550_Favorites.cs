namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Favorites : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favorites",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        FavoriteId = c.Int(nullable: false),
                        FavoriteDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfileId, t.FavoriteId })
                .ForeignKey("dbo.Profiles", t => t.FavoriteId, cascadeDelete: false)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.FavoriteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favorites", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Favorites", "FavoriteId", "dbo.Profiles");
            DropIndex("dbo.Favorites", new[] { "FavoriteId" });
            DropIndex("dbo.Favorites", new[] { "ProfileId" });
            DropTable("dbo.Favorites");
        }
    }
}
