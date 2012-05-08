using System;
using System.Linq;

namespace SyncDeps
{
	public class ArgumentParser
	{
		public ArgumentParser Read(string[] args)
		{
			string[] realArgs;

			int expectedArgs = 3;
			if (args.Last().StartsWith("-log:")) expectedArgs++;

			if (args.Length == expectedArgs) realArgs = args;
			else if (args.Length == expectedArgs +1) realArgs = args.Skip(1).ToArray();
			else throw new ArgumentException();

			BasePath = realArgs[0];
			SourcePattern = realArgs[1];
			DestPattern = realArgs[2];
			LogPath = (realArgs.Length > 3) ? (realArgs[3].Substring(5).Trim('"')) : (null);
			return this;
		}

		public string BasePath { get; private set; }

		public string SourcePattern { get; private set; }

		public string DestPattern{ get; private set; }

		public string LogPath { get; private set; }
	}
}