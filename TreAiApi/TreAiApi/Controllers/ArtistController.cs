using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TreAiApi.DataLayer;

namespace TreAiApi.Controllers
{
	public class ArtistController : ApiController
	{
		public async Task<List<dArtist>> Get()
		{
			//var caller = new Spotify.SpotifyWebApi();

			using (var db = new AiApiDbContext())
			{
				var artistList = db.Artist.Where(x => x.Popularity > 90).Take(5).ToList();

				return artistList;
			}

			return new List<dArtist>();
		}
	}
}