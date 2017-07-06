using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TreAiApi.Threads
{
	internal static class AiThreadManager
	{
		public static List<Thread> ActiveThreads { get; } = new List<Thread>();

		public static void AddThread(Thread thread)
		{
			if (!ActiveThreads.Contains(thread))
			{
				ActiveThreads.Add(thread);
				thread.Start();

				if(ActiveThreads.Count > 1)
					ActiveThreads.Last().Join();
			}
		}

		public static void RemoveThread(Thread thread)
		{
			if (ActiveThreads.Any() && ActiveThreads.Contains(thread))
				ActiveThreads.Remove(thread);

			if (!ActiveThreads.Any())
			{
				AbortAllThreads();
			}
		}

		internal static void AbortThread(Thread thread)
		{
			try
			{
				thread.Abort();
			}
			catch (ThreadAbortException ex)
			{
				Console.WriteLine(ex.InnerException);
			}
		}

		private static void AbortAllThreads()
		{
			try
			{
				ActiveThreads.ForEach(x => x.Abort());
			}
			catch (ThreadAbortException ex)
			{
				Console.WriteLine(ex.InnerException);
			}
		}
	}
}