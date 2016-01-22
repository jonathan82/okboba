namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "Deleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Profiles", "UserId", c => c.String(maxLength: 11));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Profiles", "Deleted");
        }
    }
}
