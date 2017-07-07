using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyAPI.Web.Models;

namespace TreAiApi.DataLayer
{
	public class dPlayHistory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }

		
		public string PrivateProfilesId { get; set; }

		public dPrivateProfile PrivateProfile { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? PlayedAt { get; set; }

		/// <summary>
		/// The track id.
		/// </summary>
		public string TrackId { get; set; }

		/// <summary>
		/// The disc number of the track.
		/// </summary>
		public int? DiscNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int? DurationMs { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Boolean? Explicit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string Href { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string PreviewUrl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int? TrackNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Uri { get; set; }

		#region Static Methods
		public static async Task<bool> AddHistoryToDatabase(PlaybackContext data, string userId)
		{
			using (var db = new AiApiDbContext())
			{
				try
				{
					
					var fullTrack = new dFullTrack
					{
						Id = data.Item.Id,
						Explicit = data.Item.Explicit,
						Name = data.Item.Name,
						PreviewUrl = data.Item.PreviewUrl,
						TrackNumber = data.Item.TrackNumber,
						Uri = data.Item.Uri,
						Type = data.Item.Type
					};

					if (data.Item.Artists.Any())
					{
						fullTrack.Artist1Id = data.Item.Artists[0].Id;

						if (data.Item.Artists.Count > 1)
							fullTrack.Artist2Id = data.Item.Artists[1].Id;
					}

					//let's make sure we don't already have the song in here
					if (!db.Songs.Any(s => s.Name == fullTrack.Name && s.Artist1Id == fullTrack.Artist1Id))
					{
						//add the track to query
						db.Songs.Add(fullTrack);
					}

					//then we'll grab the instance of it.
					var playHistory = new dPlayHistory
					{
						PrivateProfile = db.PrivateProfiles.SingleOrDefault(x => x.Id == userId),
						PrivateProfilesId = userId,
						Name = data.Item.Name,
						PlayedAt = DateTime.Now,
						TrackId = data.Item.Id
					};

					db.RecentlyPlayed.Add(playHistory);

					await db.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					return false;
				}
				return true;
			}
		}
		#endregion
	}
}
