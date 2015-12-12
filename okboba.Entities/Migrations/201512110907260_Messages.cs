namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Conversations", "LastMessageFrom", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "ToProfileId", "dbo.Profiles");
            DropIndex("dbo.ConversationMaps", new[] { "ToProfileId" });
            DropIndex("dbo.Conversations", new[] { "LastMessageFrom" });
            RenameColumn(table: "dbo.Messages", name: "FromProfileId", newName: "From");
            RenameColumn(table: "dbo.ConversationMaps", name: "ToProfileId", newName: "Other");
            RenameIndex(table: "dbo.Messages", name: "IX_FromProfileId", newName: "IX_From");
            AddColumn("dbo.ConversationMaps", "LastMessage_Id", c => c.Int());
            AlterColumn("dbo.ConversationMaps", "Other", c => c.Int(nullable: false));
            AlterColumn("dbo.Conversations", "Subject", c => c.String(maxLength: 100));
            CreateIndex("dbo.ConversationMaps", "Other");
            CreateIndex("dbo.ConversationMaps", "LastMessage_Id");
            AddForeignKey("dbo.ConversationMaps", "LastMessage_Id", "dbo.Messages", "Id");
            AddForeignKey("dbo.ConversationMaps", "Other", "dbo.Profiles", "Id", cascadeDelete: false);
            DropColumn("dbo.ConversationMaps", "ToPhoto");
            DropColumn("dbo.ConversationMaps", "ToName");
            DropColumn("dbo.ConversationMaps", "HasBeenRead");
            DropColumn("dbo.ConversationMaps", "HasReplies");
            DropColumn("dbo.Conversations", "LastMessageDate");
            DropColumn("dbo.Conversations", "LastMessageBlurb");
            DropColumn("dbo.Conversations", "LastMessageFrom");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conversations", "LastMessageFrom", c => c.Int());
            AddColumn("dbo.Conversations", "LastMessageBlurb", c => c.String(maxLength: 255));
            AddColumn("dbo.Conversations", "LastMessageDate", c => c.DateTime());
            AddColumn("dbo.ConversationMaps", "HasReplies", c => c.Boolean(nullable: false));
            AddColumn("dbo.ConversationMaps", "HasBeenRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ConversationMaps", "ToName", c => c.String(maxLength: 10));
            AddColumn("dbo.ConversationMaps", "ToPhoto", c => c.String(maxLength: 10));
            DropForeignKey("dbo.ConversationMaps", "Other", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "LastMessage_Id", "dbo.Messages");
            DropIndex("dbo.ConversationMaps", new[] { "LastMessage_Id" });
            DropIndex("dbo.ConversationMaps", new[] { "Other" });
            AlterColumn("dbo.Conversations", "Subject", c => c.String());
            AlterColumn("dbo.ConversationMaps", "Other", c => c.Int());
            DropColumn("dbo.ConversationMaps", "LastMessage_Id");
            RenameIndex(table: "dbo.Messages", name: "IX_From", newName: "IX_FromProfileId");
            RenameColumn(table: "dbo.ConversationMaps", name: "Other", newName: "ToProfileId");
            RenameColumn(table: "dbo.Messages", name: "From", newName: "FromProfileId");
            CreateIndex("dbo.Conversations", "LastMessageFrom");
            CreateIndex("dbo.ConversationMaps", "ToProfileId");
            AddForeignKey("dbo.ConversationMaps", "ToProfileId", "dbo.Profiles", "Id");
            AddForeignKey("dbo.Conversations", "LastMessageFrom", "dbo.Profiles", "Id");
        }
    }
}
