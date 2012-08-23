using System;
using System.Collections.Generic;
using Mono.Options;

namespace SyncDeps
{
	public class ArgumentParser
	{
		public ArgumentParser Read(string[] args)
		{
			Masters = new List<string>();
			GetOptions(args);
			return this;
		}

		void GetOptions(IEnumerable<string> args)
		{
			var p = new OptionSet{
				{ "b=|base=", "The path to search under",
				v => BasePath = strip(v)
    			},
				{ "s=|src=", "The source file pattern",
				v => SourcePattern = strip(v)
    			},
				{ "d=|dst=", "The destination file pattern",
				v => DestPattern = strip(v)
    			},
				{ "log=", "Log file",
				v => LogPath = strip(v)
    			},
				{ "m=|masters=", "Directory containing master copies. These supercede sources regardless of age",
				v => Masters.Add(v)
    			}
			};

			try {
				p.Parse(args);
				
				if (string.IsNullOrWhiteSpace(BasePath)) throw new OptionException("Must supply a base path", "b");
				if (string.IsNullOrWhiteSpace(SourcePattern)) throw new OptionException("Must supply a source pattern", "s");
				if (string.IsNullOrWhiteSpace(DestPattern)) throw new OptionException("Must supply a destination pattern", "d");
				
			} catch (OptionException e) {
				Console.Write ("SyncDeps: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `SyncDeps --help' for more information.");
				throw;
			}
		}

		string strip(string s)
		{
			return s.Trim('"', '\'');
		}

		public string BasePath { get; private set; }

		public string SourcePattern { get; private set; }

		public string DestPattern{ get; private set; }

		public string LogPath { get; private set; }

		public List<string> Masters { get; private set; }
	}
}