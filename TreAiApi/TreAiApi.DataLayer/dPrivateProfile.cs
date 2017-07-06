using System;
using System.ComponentModel.DataAnnotations;

namespace TreAiApi.DataLayer
{
	public class dPrivateProfile
	{
		[Key]
		public string Id { get; set; }

		public string DisplayName { get; set; }

		public string Email { get; set; }

		public string Birthdate { get; set; }

		public string Country { get; set; }

		public string Type { get; set; }

		public string product { get; set; }
	}
}
