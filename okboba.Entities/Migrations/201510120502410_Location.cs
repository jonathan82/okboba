namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", new[] { "Location_LocationId1", "Location_LocationId2" }, "dbo.Locations");
            DropIndex("dbo.UserProfiles", new[] { "Location_LocationId1", "Location_LocationId2" });
            RenameColumn(table: "dbo.UserProfiles", name: "Location_LocationId1", newName: "LocationId1");
            RenameColumn(table: "dbo.UserProfiles", name: "Location_LocationId2", newName: "LocationId2");
            AlterColumn("dbo.UserProfiles", "LocationId1", c => c.Short(nullable: false));
            AlterColumn("dbo.UserProfiles", "LocationId2", c => c.Short(nullable: false));
            CreateIndex("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" });
            AddForeignKey("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" }, "dbo.Locations", new[] { "LocationId1", "LocationId2" }, cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" }, "dbo.Locations");
            DropIndex("dbo.UserProfiles", new[] { "LocationId1", "LocationId2" });
            AlterColumn("dbo.UserProfiles", "LocationId2", c => c.Short());
            AlterColumn("dbo.UserProfiles", "LocationId1", c => c.Short());
            RenameColumn(table: "dbo.UserProfiles", name: "LocationId2", newName: "Location_LocationId2");
            RenameColumn(table: "dbo.UserProfiles", name: "LocationId1", newName: "Location_LocationId1");
            CreateIndex("dbo.UserProfiles", new[] { "Location_LocationId1", "Location_LocationId2" });
            AddForeignKey("dbo.UserProfiles", new[] { "Location_LocationId1", "Location_LocationId2" }, "dbo.Locations", new[] { "LocationId1", "LocationId2" });
        }
    }
}
