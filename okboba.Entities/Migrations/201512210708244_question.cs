namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class question : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Answers", "ChoiceBit", "ChoiceIndex");
            RenameColumn("dbo.Answers", "ChoiceAcceptable", "ChoiceAccept");

            //AddColumn("dbo.Answers", "ChoiceIndex", c => c.Byte());
            //AddColumn("dbo.Answers", "ChoiceAccept", c => c.Byte(nullable: false));
            //DropColumn("dbo.Answers", "ChoiceBit");
            //DropColumn("dbo.Answers", "ChoiceAcceptable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answers", "ChoiceAcceptable", c => c.Byte(nullable: false));
            AddColumn("dbo.Answers", "ChoiceBit", c => c.Byte());
            DropColumn("dbo.Answers", "ChoiceAccept");
            DropColumn("dbo.Answers", "ChoiceIndex");
        }
    }
}
