using System.ComponentModel.DataAnnotations;

namespace TreAiApi.DataLayer
{
	public class dArtist
	{
		[Key]
		public string Id { get; set; }

		public string Href { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Uri { get; set; }

		public int? Popularity { get; set; }

		public string Genre1 { get; set; }
		public string Genre2 { get; set; }
		public string Genre3 { get; set; }
		public string Genre4 { get; set; }
		public string Genre5 { get; set; }
	}
}
