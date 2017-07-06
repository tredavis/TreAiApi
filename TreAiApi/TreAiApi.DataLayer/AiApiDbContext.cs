using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SpotifyAPI.Web.Models;
using TreAiApi;
using TreAiApi.DataLayer.Interfaces;

namespace TreAiApi.DataLayer
{
	public class AiApiDbContext : DbContext
	{
		public AiApiDbContext()
		{

		}
		

		/// <summary>
		/// 
		/// </summary>
		public DbSet<dSavedTrack> SavedTracks { get; set; }

		/// <summary>
		/// The spotify users
		/// </summary>
		public DbSet<dPrivateProfile> PrivateProfiles { get; set; }

		/// <summary>
		/// The spotify users
		/// </summary>
		public DbSet<dFullTrack> Songs { get; set; }

		/// <summary>
		/// Spotify Artists
		/// </summary>
		public DbSet<dArtist> Artist { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DbSet<dPlayHistory> RecentlyPlayed { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DbSet<sLastSyncTable> LastSaveSyncTable { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DbSet<dTopArtist> TopArtists { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DbSet<dTopTrack> TopTracks { get; set; }

		/// <summary>
		/// The date of the last sync for the top tracks and top artist table.
		/// </summary>
		public DbSet<sTopSync> TopSyncDates { get; set; }


		//public void AddFullArtist(string id)
		//{
		//	foreach (var simpleArtist in track.Artists)
		//	{
		//		var artist = new dArtist()
		//		{
		//			Id = simpleArtist.Id,
		//			Name = simpleArtist.Name,
		//			Href = simpleArtist.Href,
		//			Uri = simpleArtist.Uri
		//		};
		//	}
		//}

		/// <summary>
		/// Gets all the saved tracks from the database
		/// </summary>
		public List<dSavedTrack> GetSavedTracksByUser(string userName)
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					var returnedRecords = db.SavedTracks.Where(x => x.UserName == userName).ToList();

					//removes all the records
					return returnedRecords;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}

			return new List<dSavedTrack>();
		}

		#region SyncDate

		public void UdpateSyncDate(string userId)
		{
			try
			{
				using (var db = new AiApiDbContext())
				{
					db.LastSaveSyncTable.Add(new sLastSyncTable { SavedTrackLastSync = DateTime.Today, UserId = userId});

					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public DateTime GetLastSyncDate(string userId)
		{
			try
			{
				using (var db = new AiApiDbContext())
				{
					var date = db.LastSaveSyncTable.Where(x => x.UserId == userId).ToList().Last();

					if (date != null)
					{
						return date.SavedTrackLastSync;
					}

					db.SaveChanges();
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return DateTime.Today;
		}

		#endregion

		#region Top Artists and Top Tracks

		public List<dTopTrack> GetTopTracks(string userId, Term term)
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					var returnedRecords = db.TopTracks.Where(x => x.UserName == userId).ToList();

					//removes all the records
					return returnedRecords;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}

			return new List<dTopTrack>();
		}

		public List<dTopArtist> GetTopArtists(string userId, Term term)
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					var returnedRecords = db.TopArtists.Where(x => x.UserName == userId).ToList();

					//removes all the records
					return returnedRecords;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}

			return new List<dTopArtist>();
		}

		#endregion
	}
}
