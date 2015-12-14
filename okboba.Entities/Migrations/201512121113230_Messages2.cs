namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConversationMaps", "HasBeenRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ConversationMaps", "HasReplies", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConversationMaps", "HasReplies");
            DropColumn("dbo.ConversationMaps", "HasBeenRead");
        }
    }
}
