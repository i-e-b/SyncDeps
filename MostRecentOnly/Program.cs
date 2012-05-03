using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MostRecentOnly {
	class Program {
		static void Main (string[] args) {
			#region Validation
			if (args.Length != 2 && args.Length != 3) {
				ShowUsageMessage();
				return;
			}
			var rq_path = (args.Length == 3) ? (args[1]) : (args[0]);
			var rq_pattern = (args.Length == 3) ? (args[2]) : (args[1]);
			if (!Directory.Exists(rq_path)) {
				ShowPathMessage();
				return;
			}
			#endregion

			var root_path = new DirectoryInfo(rq_path);

			var file_lists = FindMatches(rq_pattern, root_path);

			int fails = 0;
			int copies = 0;
			foreach (var filename in file_lists.Keys) {
				var match_list = file_lists[filename];
				if (match_list == null || match_list.Count < 2) continue;

				var most_recent = match_list.First(a => a.LastWriteTime == match_list.Max(b => b.LastWriteTime));
				if (most_recent == null) throw new Exception("Mismatch in list!");
				foreach (var file_info in match_list) {
					if (file_info == most_recent) continue;
					copies++;
					try {
						File.Copy(most_recent.FullName, file_info.FullName, true);
					} catch {
						Console.WriteLine("Failed to overwrite \""+file_info.FullName+"\" with \"" +most_recent.FullName+"\"");
						fails++;
					}
				}
			}

			Console.WriteLine("Finished. "+copies+" copies attempted.");
			if (fails > 0) Console.WriteLine(fails + " copies failed");
			else Console.WriteLine("OK");
		}

		private static Dictionary<string, List<FileInfo>> FindMatches (string Pattern, DirectoryInfo RootPath) {
			var most_recents = new Dictionary<string, List<FileInfo>>(); // File name => all file infos
			//var matches = RootPath.EnumerateFiles(Pattern, SearchOption.AllDirectories);

			var matches = RootPath.GetFiles(Pattern, SearchOption.AllDirectories);

			foreach (var match in matches) {
				var name = match.Name;
				if (most_recents.ContainsKey(name)) {
					most_recents[name].Add(match);
				} else {
					most_recents.Add(name, new List<FileInfo> { match });
				}
			}
			return most_recents;
		}

		private static void ShowPathMessage () {
			Console.WriteLine("Path not found or no permissions");
		}
		private static void ShowUsageMessage () {
			Console.WriteLine("Usage:");
			Console.WriteLine("MostRecentOnly \"path\" \"pattern\"");
			Console.WriteLine();
			Console.WriteLine("Path: path to check. All files matching the pattern");
			Console.WriteLine("      in this path and sub paths will be replaced by");
			Console.WriteLine("      the most recent copy found by exact name.");
			Console.WriteLine();
			Console.WriteLine("Pattern: file name pattern to match against. May use");
			Console.WriteLine("      Wildcards '*' and '?'");
			Console.WriteLine();
			Console.WriteLine("Example:");
			Console.WriteLine("      MostRecentOnly \"C:\\Projects\\Twofour.MediaFreedom\\\" \"Twofour.*.dll\"");

			Console.WriteLine();
			Console.WriteLine("Press [ENTER] to continue...");
			Console.ReadKey();
		}
	}
}
