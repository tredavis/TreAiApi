using Newtonsoft.Json;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TreAiApi.DataLayer;
using TreAiApi.DataLayer.Interfaces;
using TreAiApi.Threads;

namespace TreAiApi.Spotify
{
	public class SpotifyWebApi
	{
		#region Members

		static AutorizationCodeAuth auth;
		private List<SavedTrack> _usersSavedTracks = new List<SavedTrack>();
		private List<FullTrack> _usersTopTracks = new List<FullTrack>();
		private List<FullArtist> _shortTermArtists = new List<FullArtist>();
		private List<FullArtist> _medTermArtists = new List<FullArtist>();
		private List<FullArtist> _longTermArtists = new List<FullArtist>();
		private HttpClient _httpClient;
		private static string _tokenType;
		private static string _accessToken;

		private const string _connectionString = @"Server=tcp:dgn0w5s295.database.windows.net,1433;Initial Catalog=TreMusicDatabase;Persist Security Info=False;User ID=jwadDb;Password=Mnimld18;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the token type
		/// </summary>
		public string TokenType
		{
			get { return _tokenType; }
			set { _tokenType = value; }
		}

		/// <summary>
		/// Gets or Sets the spotify access token
		/// </summary>
		public string AccessToken
		{
			get { return _accessToken; }
			set { _accessToken = value; }
		}

		#endregion

		#region Events 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		private static void auth_OnResponseReceivedEvent(AutorizationCodeAuthResponse response)
		{
			//NEVER DO THIS! You would need to provide the ClientSecret.
			//You would need to do it e.g via a PHP-Script.
			Token token = auth.ExchangeAuthCode(response.Code, "714ba0785d9b41d3828a7ad30c782d52");

			//Stop the HTTP Server, done.
			auth.StopHttpServer();

			_tokenType = token.TokenType;
			_accessToken = token.AccessToken;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		internal SpotifyWebApi()
		{
			//Do authenication
			DoAuth();
		}

		/// <summary>
		/// 
		/// </summary>
		private void DoAuth()
		{
			//Create the auth object
			auth = new AutorizationCodeAuth()
			{
				//Your client Id
				ClientId = "ce833ef1272046489c5cb86af277d9c0",
				//Set this to localhost if you want to use the built-in HTTP Server
				RedirectUri = "http://localhost",
				//RedirectUri = "http://treaiapi20170628103254.azurewebsites.net",
				//How many permissions we need?
				Scope = Scope.All
			};
			//This will be called, if the user cancled/accept the auth-request
			auth.OnResponseReceivedEvent += auth_OnResponseReceivedEvent;
			//a local HTTP Server will be started (Needed for the response)
			auth.StartHttpServer();
			//This will open the spotify auth-page. The user can decline/accept the request
			auth.DoAuth();

			Thread.Sleep(5000);
			//auth.StopHttpServer();
			Console.WriteLine("Too long, didnt respond, exiting now...");
		}

		#region Saved Track Methods
		/// <summary>
		/// Returns the signed in users saved tracks
		/// </summary>
		/// <param name="nextUrl">Are we using the next url provided by spotify</param>
		/// <returns>the list of the total tracks</returns>
		public List<SavedTrack> GetAllSavedTracks(string nextUrl = null)
		{
			//lets create our client
			_httpClient = new HttpClient();

			var url = String.IsNullOrWhiteSpace(nextUrl) ? "https://api.spotify.com/v1/me/tracks?&limit=50&access_token=" + AccessToken : nextUrl + "&access_token=" + AccessToken;

			var response = _httpClient.GetAsync(url).Result;
			if (response.IsSuccessStatusCode)
			{
				//let's deseriale the data
				var data = JsonConvert.DeserializeObject<Paging<SavedTrack>>(response.Content.ReadAsStringAsync().Result);

				//let's add the data to the return list.
				_usersSavedTracks.AddRange(data.Items);

				if (data.HasNextPage())
				{
					GetAllSavedTracks(data.Next);
				}
			}

			return _usersSavedTracks;
		}

		/// <summary>
		/// Resyncs the current user's synced tracks
		/// </summary>
		public async Task<bool> ResyncSavedTracks(string userName)
		{
			//get's all the tracks from the database
			var tracks = GetAllSavedTracks();

			using (var db = new AiApiDbContext())
			{

				if (db.SavedTracks.Any())
				{
					//let's clear out all the other tracks in the database
					RemoveSavedTracks(userName);
				}

				try
				{
					//update the lave sync date
					db.UdpateSyncDate(userName);

					var artistList = new List<string>();

					var index = 0;
					foreach (var track in tracks)
					{
						if (artistList.Count < 50)
						{
							track.Track.Artists.ForEach(x =>
							{
								if (!artistList.Contains(x.Id))
								{
									artistList.Add(x.Id);
								}
							});
						}
						else
						{
							await GetArtistFromSpotify(artistList);
							artistList.Clear();
							continue; ;
						}
						//let's extract what we need from the track object
						dSavedTrack savedTrack = new dSavedTrack();
						savedTrack.Id = index;
						savedTrack.FullTrackId = track.Track.Id;
						savedTrack.AddedAt = track.AddedAt;
						savedTrack.UserName = userName;

						dFullTrack fullTrack = new dFullTrack
						{
							Id = track.Track.Id,
							Name = track.Track.Name,
							Explicit = track.Track.Explicit,
							Type = track.Track.Type,
							PreviewUrl = track.Track.PreviewUrl,
							TrackNumber = track.Track.TrackNumber,
							Uri = track.Track.Uri,
						};

						//if the track has an artist (which it should)
						//and the artist is not already saved in my library than save the artist out
						if (track.Track.Artists.Any())
						{
							fullTrack.Artist1Id = track.Track.Artists[0].Id;

							if (track.Track.Artists.Count > 1)
								fullTrack.Artist2Id = track.Track.Artists[1].Id;
						}

						if (db.SavedTracks.Find(savedTrack.Id) == null)
						{
							//add the track to query
							db.SavedTracks.Add(savedTrack);
						}

						if (db.Songs.Find(fullTrack.Id) == null)
						{
							db.Songs.Add(fullTrack);
						}

						//let's increment the index;
						index++;
					}

					await db.SaveChangesAsync();

					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("=====================");
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.InnerException);
					Console.WriteLine(ex.StackTrace);
					Console.WriteLine("=====================");

					return false;
				}
			}

			return false;
		}

		/// <summary>
		/// Removes the saved tracks from the database
		/// </summary>
		public async void RemoveSavedTracks(string userId)
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					//removes all the records
					var tracksToRemove = db.SavedTracks.Where(x => x.UserName == userId).ToList();

					db.SavedTracks.RemoveRange(tracksToRemove);


					await db.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}
		}

		//public void BuildArtistList()
		//{
		//	var artistList = new HashSet<string>();

		//	if (artistList.Count < 50)
		//	{
		//		track.Track.Artists.ForEach(x =>
		//		{
		//			artistList.Add(x.Id);
		//		});
		//	}
		//	else
		//	{
		//		await GetArtistFromSpotify(artistList);
		//		artistList.Clear();
		//		continue; ;
		//	}
		//}

		/// <summary>
		/// Gets all the saved tracks from the database
		/// </summary>
		public async Task<List<dSavedTrack>> GetAllSavedTracksFromDb()
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					var returnedRecords = db.SavedTracks.ToListAsync();

					//removes all the records
					return await returnedRecords;

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}

			return new List<dSavedTrack>();
		}

		/// <summary>
		/// Gets all the saved tracks from the database
		/// </summary>
		public List<dSavedTrack> GetSavedTrackFromDb(string userId, int retRec)
		{
			try
			{
				//launch up a new Db context
				using (var db = new AiApiDbContext())
				{
					var returnedRecords = db.SavedTracks.Where(x => x.UserName == userId).Take(retRec).ToList();

					//removes all the records
					return returnedRecords;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("=====================");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.InnerException);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("=====================");
			}

			return new List<dSavedTrack>();
		}


		#endregion

		#region Get Current User
		public PrivateProfile GetCurrentUser()
		{
			//lets create our client
			_httpClient = new HttpClient();

			var url = "https://api.spotify.com/v1/me?&access_token=" + AccessToken;

			var response = _httpClient.GetAsync(url).Result;
			if (response.IsSuccessStatusCode)
			{
				//let's deseriale the data
				var result = response.Content.ReadAsStringAsync().Result;

				return JsonConvert.DeserializeObject<PrivateProfile>(result);
			}

			return new PrivateProfile();
		}
		#endregion

		#region GetUsersTopTracks

		/// <summary>
		/// Returns the current logged in users top tracks from the last 4 weeks. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullTrack>> GetUsersTopTracksShortTerm(string userId)
		{
			var shortTermTracks = new List<FullTrack>();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/tracks?time_range=short_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullTrack>>(responseContent);

					_usersTopTracks.AddRange(data.Items);

					using (var db = new AiApiDbContext())
					{

						var index = 1;
						shortTermTracks.ForEach(x =>
						{

							//lets try to add the fulltrack (spotify verson) of the track to the database
							dFullTrack.AddFullTrackToDb(x);

							var newTrack = new dTopTrack
							{
								Name = x.Name,
								FullTrackId = x.Id,
								Popularity = x.Popularity,
								Uri = x.Uri,
								Term = Term.Short,
								Rank = index++,
								UserName = userId
							};

							db.TopTracks.Add(newTrack);
						});

						await db.SaveChangesAsync();
					}
				}
			}

			return _usersTopTracks;
		}

		/// <summary>
		/// Returns the current logged in users top tracks from the last 6 months. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullTrack>> GetUsersTopTracksMeduimTerm(string userId)
		{
			var medTermTracks = new List<FullTrack>();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/tracks?time_range=medium_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullTrack>>(responseContent);

					_usersTopTracks.AddRange(data.Items);

					using (var db = new AiApiDbContext())
					{
						var index = 1;
						medTermTracks.ForEach(x =>
						{
							//lets try to add the fulltrack (spotify verson) of the track to the database

							dFullTrack.AddFullTrackToDb(x);

							var newTrack = new dTopTrack
							{
								Name = x.Name,
								FullTrackId = x.Id,
								Popularity = x.Popularity,
								Uri = x.Uri,
								Term = Term.Medium,
								Rank = index++,
								UserName = userId
							};

							db.TopTracks.Add(newTrack);
						});

						await db.SaveChangesAsync();
					}
				}
			}

			return medTermTracks;
		}

		/// <summary>
		/// Returns the current logged in users top tracks from the last Sereral years years. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullTrack>> GetUsersTopTracksLongTerm(string userId)
		{
			var longTermTracks = new List<FullTrack>();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/tracks?time_range=long_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullTrack>>(responseContent);

					longTermTracks.AddRange(data.Items);

					using (var db = new AiApiDbContext())
					{
						var index = 1;
						longTermTracks.ForEach(x =>
						{
							//lets try to add the fulltrack (spotify verson) of the track to the database
							dFullTrack.AddFullTrackToDb(x);

							var newTrack = new dTopTrack
							{
								Name = x.Name,
								FullTrackId = x.Id,
								Popularity = x.Popularity,
								Uri = x.Uri,
								Term = Term.Long,
								Rank = index++,
								UserName = userId
							};

							db.TopTracks.Add(newTrack);
						});

						await db.SaveChangesAsync();
					}
				}
			}

			return longTermTracks;
		}

		#endregion

		#region GetUsersTopArtist

		List<FullArtist> _artist = new List<FullArtist>();

		/// <summary>
		/// Gets artist from spotify and then saved them out to the database
		/// </summary>
		/// <param name="nextUrl"></param>
		/// <returns></returns>
		public async Task<bool> GetArtistFromSpotify(List<string> ids)
		{
			_artist.Clear();
			try
			{
				using (var db = new AiApiDbContext())
				{
					if (ids.Count > 50)
						ids.RemoveAt(ids.Count - 1);
					var empString = "";
					foreach (var id in ids)
					{
						if (string.IsNullOrWhiteSpace(empString))
						{
							empString += $"{id}";
						}
						else
						{
							empString += $",{id}";
						}
					}
					using (var client = new HttpClient())
					{
						client.DefaultRequestHeaders.Accept.Clear();
						client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

						HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/artists?ids=" + empString);

						if (responseMessage.IsSuccessStatusCode)
						{
							var responseContent = await responseMessage.Content.ReadAsStringAsync();
							var data = JsonConvert.DeserializeObject<SeveralArtists>(responseContent);

							data.Artists.ForEach(_artist.Add);
						}
					}


					foreach (var a in _artist)
					{
						var dArtist = new dArtist()
						{
							Id = a.Id,
							Popularity = a.Popularity,
							Name = a.Name,
							Uri = a.Uri,
							Type = a.Type
						};

						foreach (var genreString in a.Genres)
						{
							if (string.IsNullOrWhiteSpace(dArtist.Genre1))
							{
								dArtist.Genre1 = genreString;
								continue;
							}

							if (string.IsNullOrWhiteSpace(dArtist.Genre2))
							{
								dArtist.Genre2 = genreString;
								continue;
							}

							if (string.IsNullOrWhiteSpace(dArtist.Genre3))
							{
								dArtist.Genre3 = genreString;
								continue;
							}

							if (string.IsNullOrWhiteSpace(dArtist.Genre4))
							{
								dArtist.Genre4 = genreString;
								continue;
							}

							if (string.IsNullOrWhiteSpace(dArtist.Genre5))
							{
								dArtist.Genre5 = genreString;
								break;
							}
						}

						var t = db.Artist.Where(x => x.Id == dArtist.Id).ToList();

						if (!db.Artist.Where(x => x.Id == dArtist.Id).ToList().Any())
						{
							db.Artist.Add(dArtist);
						}
					}

					await db.SaveChangesAsync();
					_artist.Clear();
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex);
				return false;
			}
		}

		/// <summary>
		/// Returns the current logged in users top tracks from the last 4 weeks. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullArtist>> GetUsersTopArtistsShortTerm(string userId)
		{
			_shortTermArtists.Clear();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/artists?time_range=short_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullArtist>>(responseContent);

					_shortTermArtists.AddRange(data.Items);

					try
					{
						using (var db = new AiApiDbContext())
						{
							var rank = 1;
							_shortTermArtists.ForEach(x =>
							{
								var artist = new dTopArtist
								{
									Name = x.Name,
									Popularity = x.Popularity,
									Term = Term.Short,
									UserId = userId,
									Uri = x.Uri,
									Rank = rank++
								};

								if (!db.TopArtists.Any(a => a.Name == artist.Name && a.Term == artist.Term && a.UserId == userId))
								{
									db.TopArtists.Add(artist);
								}
							});

							await db.SaveChangesAsync();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			}

			return _shortTermArtists;
		}

		/// <summary>
		/// Returns the current logged in users top tracks from the last 6 months. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullArtist>> GetUsersTopArtistsMeduimTerm(string userId)
		{
			_medTermArtists.Clear();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/artists?time_range=medium_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullArtist>>(responseContent);

					_medTermArtists.AddRange(data.Items);

					try
					{
						using (var db = new AiApiDbContext())
						{
							_medTermArtists.ForEach(x =>
							{
								var artist = new dTopArtist
								{
									Name = x.Name,
									Popularity = x.Popularity,
									Term = Term.Medium,
									UserId = userId,
									Uri = x.Uri
								};

								if (!db.TopArtists.Any(a => a.Name == artist.Name && a.Term == artist.Term && a.UserId == userId))
								{
									db.TopArtists.Add(artist);
								}
							});

							await db.SaveChangesAsync();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			}

			return _medTermArtists;
		}

		/// <summary>
		/// Returns the current logged in users top tracks from the last Sereral years years. 
		/// </summary>
		/// <returns></returns>
		public async Task<List<FullArtist>> GetUsersTopArtistsLongTerm(string userId)
		{
			_longTermArtists.Clear();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/top/artists?time_range=long_term");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<Paging<FullArtist>>(responseContent);

					_longTermArtists.AddRange(data.Items);


					using (var db = new AiApiDbContext())
					{
						_longTermArtists.ForEach(x =>
						{
							var artist = new dTopArtist
							{
								Name = x.Name,
								Popularity = x.Popularity,
								Term = Term.Long,
								UserId = userId,
								Uri = x.Uri
							};

							if (!db.TopArtists.Any(a => a.Name == artist.Name && a.Term == artist.Term && a.UserId == userId))
							{
								db.TopArtists.Add(artist);
							}
						});

						await db.SaveChangesAsync();
					}
				}
			}

			return _longTermArtists;
		}

		public async void GetRecentlyPlayedArtist()
		{
			var recentTracks = new List<PlayHistory>();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/player/recently-played?limit=50");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<CursorPaging<PlayHistory>>(responseContent);

					recentTracks.AddRange(data.Items);
				}

				using (var db = new AiApiDbContext())
				{
					var index = 0;
					recentTracks.ForEach(x =>
					{
						var dPlayHistory = new dPlayHistory();

						dPlayHistory.TrackId = x.Track.Id;
						dPlayHistory.PlayedAt = x.PlayedAt;
						dPlayHistory.Id = index;

						var existingTrack = db.Songs.Find(dPlayHistory.TrackId);

						//if (existingTrack != null)
						//{
						//	db.Songs.Add()
						//}

						db.RecentlyPlayed.Add(dPlayHistory);

						index++;
					});
				}
			}
		}

		#endregion

		#region User

		public void SaveUser(PrivateProfile user)
		{
			var dProfile = new dPrivateProfile
			{
				Id = user.Id,
				Birthdate = user.Birthdate,
				Country = user.Country,
				DisplayName = user.DisplayName,
				Email = user.Email,
				product = user.Product,
				Type = user.Type
			};

			//lets send the user to the database
			using (var db = new AiApiDbContext())
			{
				// if we don't find the user in the data base
				if (db.PrivateProfiles.Find(dProfile.Id) == null)
				{
					db.PrivateProfiles.Add(dProfile);
				}

				db.SaveChanges();
			}
		}
		#endregion

		#region GetCurrentTrack

		public async Task<FullTrack> GetCurrentlyPlayingTrack(string userId)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

				HttpResponseMessage responseMessage = await client.GetAsync("https://api.spotify.com/v1/me/player/currently-playing");

				if (responseMessage.IsSuccessStatusCode)
				{
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<PlaybackContext>(responseContent);
					
					await dPlayHistory.AddHistoryToDatabase(data, userId);

					return data.Item;
				}
			}

			return new FullTrack();
		}

		#endregion

		#region Multithreaded Calls

		/// <summary>
		/// Deletes and rewrites the Top Artists and Top Tracks from spotify. Asynchnonously while also launching up two new threads.
		/// </summary>
		/// <param name="user">The user to get this information for.</param>
		public void SyncTopList(PrivateProfile user)
		{
			Thread artistThread = null;
			Thread trackThread = null;

			try
			{
				//the new thread for the artists objects
				artistThread = new Thread(async () =>
				{
					await GetUsersTopArtistsShortTerm(user.Id);
					await GetUsersTopArtistsMeduimTerm(user.Id);
					await GetUsersTopArtistsLongTerm(user.Id);

					AiThreadManager.RemoveThread(artistThread);
				});

				AiThreadManager.AddThread(artistThread);

				//the new thread for the track objects
				trackThread = new Thread(async () =>
				{
					await GetUsersTopTracksShortTerm(user.Id);
					await GetUsersTopTracksMeduimTerm(user.Id);
					await GetUsersTopTracksLongTerm(user.Id);

					AiThreadManager.RemoveThread(trackThread);
				});

				AiThreadManager.AddThread(trackThread);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
			}
			finally
			{
				using (var db = new AiApiDbContext())
				{
					db.TopSyncDates.Add(new sTopSync()
					{
						UserId = user.Id,
						SyncTop = SpotifyTopOptions.Artists
					});

					db.TopSyncDates.Add(new sTopSync()
					{
						UserId = user.Id,
						SyncTop = SpotifyTopOptions.Tracks
					});

					db.SaveChangesAsync();
				}
			}
		}

		#endregion
	}
}