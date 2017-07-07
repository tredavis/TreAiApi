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
			
			return View();
		}
	}
}
