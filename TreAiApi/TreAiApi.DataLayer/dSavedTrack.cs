using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreAiApi.DataLayer
{

	public class dSavedTrack
	{
		/// <summary>
		/// Key for cSavedTrack 
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public new int Id { get; set; }

		/// <summary>
		/// Full track id which links the saved track to the full track
		/// </summary>
		public string FullTrackId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public dFullTrack FullTrack { get; set; }

		/// <summary>
		/// User name of the user who this track belongs too.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? AddedAt { get; set; }

		/// <summary>
		/// 
		/// </summary>
		//public new FullTrack FullTrack => null;

		/// <summary>
		/// Contructor for the saved tracked
		/// </summary>
		public dSavedTrack()
		{
		}

		/// <summary>
		/// Contructor for the saved tracked
		/// </summary>
		/// <param name="savedTrack"></param>
		public dSavedTrack(dSavedTrack savedTrack)
		{
			if (savedTrack != null)
			{
				//FullTrackId = savedTrack.Track?.Id;
			}
		}
	}
}
