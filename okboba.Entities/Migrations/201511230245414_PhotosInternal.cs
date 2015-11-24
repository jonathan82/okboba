namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotosInternal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Profiles", "PhotosInternal", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "PhotosInternal", c => c.String(maxLength: 90));
        }
    }
}
