namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConversationMaps",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        ConversationId = c.Int(nullable: false),
                        ToProfileId = c.Int(),
                        ToPhoto = c.String(maxLength: 10),
                        ToName = c.String(maxLength: 10),
                        HasBeenRead = c.Boolean(nullable: false),
                        HasReplies = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfileId, t.ConversationId })
                .ForeignKey("dbo.Conversations", t => t.ConversationId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ToProfileId)
                .Index(t => t.ProfileId)
                .Index(t => t.ConversationId)
                .Index(t => t.ToProfileId);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        LastMessageDate = c.DateTime(),
                        LastMessageBlurb = c.String(maxLength: 255),
                        LastMessageFrom = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.LastMessageFrom)
                .Index(t => t.LastMessageFrom);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConversationId = c.Int(nullable: false),
                        FromProfileId = c.Int(nullable: false),
                        MessageText = c.String(maxLength: 1000),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conversations", t => t.ConversationId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.FromProfileId, cascadeDelete: true)
                .Index(t => t.ConversationId)
                .Index(t => t.FromProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "FromProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropForeignKey("dbo.ConversationMaps", "ToProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ConversationMaps", "ConversationId", "dbo.Conversations");
            DropForeignKey("dbo.Conversations", "LastMessageFrom", "dbo.Profiles");
            DropIndex("dbo.Messages", new[] { "FromProfileId" });
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropIndex("dbo.Conversations", new[] { "LastMessageFrom" });
            DropIndex("dbo.ConversationMaps", new[] { "ToProfileId" });
            DropIndex("dbo.ConversationMaps", new[] { "ConversationId" });
            DropIndex("dbo.ConversationMaps", new[] { "ProfileId" });
            DropTable("dbo.Messages");
            DropTable("dbo.Conversations");
            DropTable("dbo.ConversationMaps");
        }
    }
}
