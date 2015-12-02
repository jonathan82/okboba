namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Activity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        Metadata = c.String(maxLength: 100),
                        When = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            RenameColumn("dbo.Profiles", "Name", "Nickname");
            AddColumn("dbo.Profiles", "LookingForGender", c => c.Byte(nullable: false));
            DropColumn("dbo.Profiles", "Height");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "Height", c => c.Short(nullable: false));
            AddColumn("dbo.Profiles", "Name", c => c.String(maxLength: 20));
            DropColumn("dbo.Profiles", "LookingForGender");
            DropColumn("dbo.Profiles", "Nickname");
            DropTable("dbo.Activities");
        }
    }
}
