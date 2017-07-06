using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SpotifyAPI.Web;
using TreAiApi.DataLayer;

namespace TreAiApi.Controllers
{
	public class HomeController : Controller
	{
		public async Task<ActionResult> Index()
		{
			ViewBag.Title = "Home Page";

			


			//var caller = new Spotify.SpotifyWebApi();

			////let's make sure we have a acces token because if not we cannot make any calls
			//if (caller.AccessToken != null)
			//{
			//	var userName = caller.GetCurrentUser().Id;

			//	if (!string.IsNullOrWhiteSpace(userName))
			//	{
			//		caller.ResyncSavedTracks(userName);
			//	}
			//}

			return View();
		}
	}
}
