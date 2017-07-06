using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreAiApi.DataLayer.Interfaces;

namespace TreAiApi.DataLayer
{
	public class dTopTrack : ITop
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public int Popularity { get; set; }
		public string Uri { get; set; }
		public string UserId { get; set; }
		public dPrivateProfile User { get; set; }
		public Term Term { get; set; }
		public int Rank { get; set; }


		public string FullTrackId { get; set; }
	}
}
