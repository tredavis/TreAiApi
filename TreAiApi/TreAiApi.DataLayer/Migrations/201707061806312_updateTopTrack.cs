namespace TreAiApi.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTopTrack : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.dTopArtists", "Rank", c => c.Int(nullable: false));
            AddColumn("dbo.dTopTracks", "Rank", c => c.Int(nullable: false));
            AddColumn("dbo.dTopTracks", "FullTrackId", c => c.String(maxLength: 128));
            CreateIndex("dbo.dTopTracks", "FullTrackId");
            AddForeignKey("dbo.dTopTracks", "FullTrackId", "dbo.dFullTracks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dTopTracks", "FullTrackId", "dbo.dFullTracks");
            DropIndex("dbo.dTopTracks", new[] { "FullTrackId" });
            DropColumn("dbo.dTopTracks", "FullTrackId");
            DropColumn("dbo.dTopTracks", "Rank");
            DropColumn("dbo.dTopArtists", "Rank");
        }
    }
}
