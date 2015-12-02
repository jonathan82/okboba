namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Activities", "ProfileId", "Who");
            RenameColumn("dbo.Activities", "Metadata", "What");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "Metadata", c => c.String(maxLength: 100));
            AddColumn("dbo.Activities", "ProfileId", c => c.Int(nullable: false));
            DropColumn("dbo.Activities", "What");
            DropColumn("dbo.Activities", "Who");
        }
    }
}
