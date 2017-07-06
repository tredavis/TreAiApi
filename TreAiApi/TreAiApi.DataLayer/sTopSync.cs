using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreAiApi.DataLayer.Interfaces;

namespace TreAiApi.DataLayer
{
	public class sTopSync : ISync
	{
		private DateTime m_currentDate = DateTime.Today;
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public DateTime SavedTrackLastSync
		{
			get => m_currentDate;
			set
			{
				if (value != DateTime.Today)
				{
					m_currentDate = value;
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string UserId { get; set; }

		public SpotifyTopOptions SyncTop { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string  Type { get; set; }
	}
}
