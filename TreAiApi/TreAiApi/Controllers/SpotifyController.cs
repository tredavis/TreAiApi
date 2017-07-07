using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using TreAiApi.DataLayer;
using TreAiApi.Spotify;
using TreAiApi.Threads;

namespace TreAiApi.Controllers
{
	public class SpotifyController : ApiController
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<List<object>> Get()
		{

			//all of this code up here may not be neccessary 
			//I need a way to speed up the application.
			var list = new List<object>();

			using (var db = new AiApiDbContext())
			{
				var tracks = db.RecentlyPlayed.ToList();

				list.AddRange(tracks);
			}

			Thread kickThread = null;

			kickThread = new Thread(async ()  =>
			{
				await KickoffSpotifySync();

				AiThreadManager.RemoveThread(kickThread);
			});

			AiThreadManager.AddThread(kickThread);


			if (list.Any())
			{
				return list;
			}


			return new List<object>();
		}

		/// <summary>
		/// Kicks off the spotify sync methods. The will run in the background while we fetch the users data.
		/// </summary>
		internal async Task<bool> KickoffSpotifySync()
		{
			var SController = new Spotify.SpotifyWebApi();
			var trackedSynced = true;

			//let's make sure we have a acces token because if not we cannot make any calls
			if (SController.AccessToken != null)
			{
				//get the current user
				var user = SController.GetCurrentUser();

				if (user != null)
				{
					//get and save the currently playing track.
					await SController.GetCurrentlyPlayingTrack(user.Id);

					DateTime date;

					using (var db = new AiApiDbContext())
					{
						//when was the last time we synched
						date = db.GetLastSyncDate(user.Id);

						//this call attemps to save the user
						//if the user is already in the database it wont save them again.
						SController.SaveUser(user);

						var syncNeeded = DateTime.Compare(date, DateTime.Today);

						if (!string.IsNullOrWhiteSpace(user.Id) && syncNeeded < 0)
						{
							//await SController.ResyncSavedTracks(user.Id);
						}

						//caller.GetRecentlyPlayedArtist();

						//this determines if we should sync up the users top tracks and artists
						//SController.SyncTopList(user);
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Calls the init methods for grabbing a users top artists. 
		/// </summary>
		/// <param name="SController">The controller which conatains all the spotify calls</param>
		/// <param name="user">The current signed in user. This should never be called with a null value here.</param>
		/// <returns></returns>
		private async Task<bool> GetCurrentUserTopArtists(SpotifyWebApi SController, PrivateProfile user)
		{
			try
			{
				// fetches the current user's top artists. It grabs the short, med and long term data.
				await SController.GetUsersTopArtistsShortTerm(user.Id);
				await SController.GetUsersTopArtistsMeduimTerm(user.Id);
				await SController.GetUsersTopArtistsLongTerm(user.Id);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);

				return false;
			}

			//if there were no expections then we assume everything went well. 
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SController"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		private async Task<bool> GetCurrentUserTopTracks(SpotifyWebApi SController, PrivateProfile user)
		{
			try
			{
				// fetches the current user's top artists. It grabs the short, med and long term data.
				await SController.GetUsersTopTracksShortTerm(user.Id);
				await SController.GetUsersTopTracksMeduimTerm(user.Id);
				await SController.GetUsersTopTracksLongTerm(user.Id);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);

				return false;
			}

			//if there were no expections then we assume everything went well. 
			return true;
		}

		private List<dFullTrack> ProcessTracks(List<FullTrack> fTrack)
		{


			return new List<dFullTrack>();
		}
	}
}