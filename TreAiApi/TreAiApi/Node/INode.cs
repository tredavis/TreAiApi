using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreAiApi.Node
{
	/// <summary>
	/// 
	/// </summary>
	public interface INode
	{
		/// <summary>
		/// 
		/// </summary>
		void OnLoad();

		/// <summary>
		/// 
		/// </summary>
		void Reload();

		/// <summary>
		/// 
		/// </summary>
		void ParlayMessage();


	}

	/// <summary>
	/// 
	/// </summary>
	public static class NodeManager
	{
		/// <summary>
		/// 
		/// </summary>
		private static List<Guid> IdList { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public static List<NodeParcel> ActiveParcels { get; set; }

		public static void AddParcel()
		{
			
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NodeParcel : INode 
	{
		public void OnLoad()
		{
			throw new NotImplementedException();
		}

		public void Reload()
		{
			throw new NotImplementedException();
		}

		public void ParlayMessage()
		{
			throw new NotImplementedException();
		}
	}
}