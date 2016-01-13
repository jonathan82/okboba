namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feed : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Activities", "What", "Field1");
            RenameColumn("dbo.Activities", "When", "Timestamp");
            AddColumn("dbo.Activities", "Field2", c => c.String(maxLength: 100));

            //AddColumn("dbo.Activities", "Timestamp", c => c.DateTime(nullable: false));
            //DropColumn("dbo.Activities", "What");
            //DropColumn("dbo.Activities", "When");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "When", c => c.DateTime(nullable: false));
            AddColumn("dbo.Activities", "What", c => c.String(maxLength: 100));
            DropColumn("dbo.Activities", "Timestamp");
            DropColumn("dbo.Activities", "Field2");
            DropColumn("dbo.Activities", "Field1");
        }
    }
}
