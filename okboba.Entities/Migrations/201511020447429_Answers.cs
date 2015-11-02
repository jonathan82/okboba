namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Answers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Answers", "ChoiceIndex", c => c.Byte());
            AlterColumn("dbo.Answers", "ChoiceWeight", c => c.Byte());
            AlterColumn("dbo.Answers", "ChoiceAcceptable", c => c.Byte());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Answers", "ChoiceAcceptable", c => c.Byte(nullable: false));
            AlterColumn("dbo.Answers", "ChoiceWeight", c => c.Byte(nullable: false));
            AlterColumn("dbo.Answers", "ChoiceIndex", c => c.Byte(nullable: false));
        }
    }
}
