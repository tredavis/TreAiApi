namespace TreAiApi.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class azuretrial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dArtists",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Href = c.String(),
                        Name = c.String(),
                        Type = c.String(),
                        Uri = c.String(),
                        Popularity = c.Int(),
                        Genre1 = c.String(),
                        Genre2 = c.String(),
                        Genre3 = c.String(),
                        Genre4 = c.String(),
                        Genre5 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.sLastSyncTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SavedTrackLastSync = c.DateTime(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.dPrivateProfiles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                        Email = c.String(),
                        Birthdate = c.String(),
                        Country = c.String(),
                        Type = c.String(),
                        product = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.dPlayHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayedAt = c.DateTime(),
                        TrackId = c.String(),
                        DiscNumber = c.Int(),
                        DurationMs = c.Int(),
                        Explicit = c.Boolean(),
                        Href = c.String(),
                        Name = c.String(),
                        PreviewUrl = c.String(),
                        TrackNumber = c.Int(),
                        Type = c.String(),
                        Uri = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.dSavedTracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullTrackId = c.String(maxLength: 128),
                        UserName = c.String(),
                        AddedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.dFullTracks", t => t.FullTrackId)
                .Index(t => t.FullTrackId);
            
            CreateTable(
                "dbo.dFullTracks",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Explicit = c.Boolean(nullable: false),
                        Type = c.String(),
                        PreviewUrl = c.String(),
                        TrackNumber = c.Int(nullable: false),
                        Uri = c.String(),
                        Artist1Id = c.String(),
                        Artist2Id = c.String(),
                        DatePlayed = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.dTopArtists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserName = c.String(),
                        Popularity = c.Int(nullable: false),
                        Uri = c.String(),
                        UserId = c.String(maxLength: 128),
                        Term = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.dPrivateProfiles", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.sTopSyncs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SavedTrackLastSync = c.DateTime(nullable: false),
                        UserId = c.String(),
                        SyncTop = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.dTopTracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserName = c.String(),
                        Popularity = c.Int(nullable: false),
                        Uri = c.String(),
                        UserId = c.String(maxLength: 128),
                        Term = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.dPrivateProfiles", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dTopTracks", "UserId", "dbo.dPrivateProfiles");
            DropForeignKey("dbo.dTopArtists", "UserId", "dbo.dPrivateProfiles");
            DropForeignKey("dbo.dSavedTracks", "FullTrackId", "dbo.dFullTracks");
            DropIndex("dbo.dTopTracks", new[] { "UserId" });
            DropIndex("dbo.dTopArtists", new[] { "UserId" });
            DropIndex("dbo.dSavedTracks", new[] { "FullTrackId" });
            DropTable("dbo.dTopTracks");
            DropTable("dbo.sTopSyncs");
            DropTable("dbo.dTopArtists");
            DropTable("dbo.dFullTracks");
            DropTable("dbo.dSavedTracks");
            DropTable("dbo.dPlayHistories");
            DropTable("dbo.dPrivateProfiles");
            DropTable("dbo.sLastSyncTables");
            DropTable("dbo.dArtists");
        }
    }
}
