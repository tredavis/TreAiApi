using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreAiApi.DataLayer.Interfaces
{
	public interface ITop
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		int Id { get; set; }

		string Name { get; set; }

		string UserName { get; set; }

		int Popularity { get; set; }

		string Uri { get; set; }

		string UserId { get; set; }
		dPrivateProfile User { get; set; }

		Term Term { get; set; }

		int Rank { get; set; }
	}

	public enum Term
	{
		Short = 1,
		Medium = 2,
		Long = 4
	}
}
