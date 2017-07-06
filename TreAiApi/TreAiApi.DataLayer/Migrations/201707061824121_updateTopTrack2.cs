namespace TreAiApi.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTopTrack2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.dTopTracks", "FullTrackId", "dbo.dFullTracks");
            DropIndex("dbo.dTopTracks", new[] { "FullTrackId" });
            AlterColumn("dbo.dTopTracks", "FullTrackId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.dTopTracks", "FullTrackId", c => c.String(maxLength: 128));
            CreateIndex("dbo.dTopTracks", "FullTrackId");
            AddForeignKey("dbo.dTopTracks", "FullTrackId", "dbo.dFullTracks", "Id");
        }
    }
}
