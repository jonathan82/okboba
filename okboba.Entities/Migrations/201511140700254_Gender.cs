namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gender : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.Profiles", "Gender", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "Gender", c => c.String(maxLength: 1));
        }
    }
}
