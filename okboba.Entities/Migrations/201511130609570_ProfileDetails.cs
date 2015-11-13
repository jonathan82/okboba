namespace okboba.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfileDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfileDetailOptions",
                c => new
                    {
                        ColName = c.String(nullable: false, maxLength: 20),
                        Id = c.Byte(nullable: false),
                        Value = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => new { t.ColName, t.Id });
            
            CreateTable(
                "dbo.ProfileDetails",
                c => new
                    {
                        ProfileId = c.Int(nullable: false),
                        Height = c.Short(nullable: false),
                        Education = c.Byte(nullable: false),
                        RelationshipStatus = c.Byte(nullable: false),
                        HaveChildren = c.Byte(nullable: false),
                        WantChildren = c.Byte(nullable: false),
                        Nationality = c.Byte(nullable: false),
                        MonthlyIncome = c.Byte(nullable: false),
                        LivingSituation = c.Byte(nullable: false),
                        CarSituation = c.Byte(nullable: false),
                        Smoke = c.Byte(nullable: false),
                        Drink = c.Byte(nullable: false),
                        Exercise = c.Byte(nullable: false),
                        Eating = c.Byte(nullable: false),
                        Shopping = c.Byte(nullable: false),
                        Religion = c.Byte(nullable: false),
                        SleepSchedule = c.Byte(nullable: false),
                        SocialCircle = c.Byte(nullable: false),
                        MostMoney = c.Byte(nullable: false),
                        Housework = c.Byte(nullable: false),
                        LovePets = c.Byte(nullable: false),
                        HavePets = c.Byte(nullable: false),
                        Job = c.Byte(nullable: false),
                        Industry = c.Byte(nullable: false),
                        WorkHours = c.Byte(nullable: false),
                        CareerAndFamily = c.Byte(nullable: false),
                        BodyType = c.Byte(nullable: false),
                        FaceType = c.Byte(nullable: false),
                        EyeColor = c.Byte(nullable: false),
                        EyeShape = c.Byte(nullable: false),
                        HairColor = c.Byte(nullable: false),
                        HairType = c.Byte(nullable: false),
                        HairLength = c.Byte(nullable: false),
                        SkinType = c.Byte(nullable: false),
                        Muscle = c.Byte(nullable: false),
                        HealthCondition = c.Byte(nullable: false),
                        DressStyle = c.Byte(nullable: false),
                        Sociability = c.Byte(nullable: false),
                        Humour = c.Byte(nullable: false),
                        Temper = c.Byte(nullable: false),
                        Feelings = c.Byte(nullable: false),
                        LookingFor = c.Byte(nullable: false),
                        EconomicConcept = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ProfileId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfileDetails", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ProfileDetails", new[] { "ProfileId" });
            DropTable("dbo.ProfileDetails");
            DropTable("dbo.ProfileDetailOptions");
        }
    }
}
