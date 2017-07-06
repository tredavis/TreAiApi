using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyAPI.Web.Models;

namespace TreAiApi.DataLayer
{
	public class dPlayHistory
	{
		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }

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
	}
}
