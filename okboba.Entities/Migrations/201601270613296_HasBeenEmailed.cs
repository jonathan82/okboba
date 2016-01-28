namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HasBeenEmailed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConversationMaps", "HasBeenEmailed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConversationMaps", "HasBeenEmailed");
        }
    }
}
