using System;
using HypeFramework;

namespace Lounging
{
	public class Lounger
	{
		public Lounger()
		{

		}

		public Lounger(HYPInstance instance)
		{
			Instance = instance;
			Id = instance.StringIdentifier;
		}

		public string Id { get; set; }

		public HYPInstance Instance { get; set; }

		public Lounge Lounge { get; set; }

		public string UserName { get; set; }

		public string FirstName { get; set; }

		public char LastInitial { get; set; }
	}
}
