using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreAiApi.DataLayer.Interfaces
{
	public interface ISync
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		int Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		DateTime SavedTrackLastSync { get; set; }

		/// <summary>
		/// 
		/// </summary>
		string UserId { get; set; }

		/// <summary>
		/// 0 is Tracks, 1 is Artist
		/// </summary>
		SpotifyTopOptions SyncTop { get; set; }
	}

	/// <summary>
	/// The options spotify provides for synching up a user's top data.
	/// </summary>
	public enum SpotifyTopOptions
	{
		Tracks = 0,
		Artists = 1
	}
}
