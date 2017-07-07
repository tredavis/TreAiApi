namespace TreAiApi.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playhistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.dPlayHistories", "PrivateProfilesId", c => c.String());
            AddColumn("dbo.dPlayHistories", "PrivateProfile_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.dPlayHistories", "PrivateProfile_Id");
            AddForeignKey("dbo.dPlayHistories", "PrivateProfile_Id", "dbo.dPrivateProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dPlayHistories", "PrivateProfile_Id", "dbo.dPrivateProfiles");
            DropIndex("dbo.dPlayHistories", new[] { "PrivateProfile_Id" });
            DropColumn("dbo.dPlayHistories", "PrivateProfile_Id");
            DropColumn("dbo.dPlayHistories", "PrivateProfilesId");
        }
    }
}
