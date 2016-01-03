namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileIdToUsers : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "Profile_Id", newName: "ProfileId");
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_Profile_Id", newName: "IX_ProfileId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_ProfileId", newName: "IX_Profile_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "ProfileId", newName: "Profile_Id");
        }
    }
}
