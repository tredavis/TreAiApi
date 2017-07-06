using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;

namespace TreAiApi.DataLayer
{
	public class dFullTrack
	{
		[Key]
		public string Id { get; set; }

		public string Name { get; set; }

		public bool Explicit { get; set; }

		public string Type { get; set; }

		public string PreviewUrl { get; set; }

		public int TrackNumber { get; set; }

		public string Uri { get; set; }

		public string Artist1Id { get; set; }


		public string Artist2Id { get; set; }

		/// <summary>
		/// This is the date that the track was played at. 
		/// NOTE: This is only set if the track is coming from the the recently played played
		/// </summary>
		public DateTime? DatePlayed { get; set; }

		#region 
		/// <summary>
		/// Adds and saved the full track to the database.
		/// If the track already exists it skips adding it.
		/// </summary>
		/// <param name="track">The spotify "FullTrack" object to add</param>
		public static void AddFullTrackToDb(FullTrack track)
		{
			using (var db = new AiApiDbContext())
			{
				var fullTrack = new dFullTrack
				{
					Id = track.Id,
					Explicit = track.Explicit,
					Name = track.Name,
					PreviewUrl = track.PreviewUrl,
					TrackNumber = track.TrackNumber,
					Uri = track.Uri,
					Type = track.Type
				};

				if (track.Artists.Any())
				{
					fullTrack.Artist1Id = track.Artists[0].Id;

					if (track.Artists.Count > 1)
						fullTrack.Artist2Id = track.Artists[1].Id;
				}

				//let's make sure we don't already have the song in here
				if (!db.Songs.Any(s => s.Name == fullTrack.Name && s.Artist1Id == fullTrack.Artist1Id))
				{
					//add the track to query
					db.Songs.Add(fullTrack);
				}

				db.SaveChangesAsync();
			}
		}

		#endregion

	}
}
