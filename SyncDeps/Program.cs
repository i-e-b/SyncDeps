using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace SyncDeps {
	class Program {
			// provide a list of '/bin' folders and a list of '/dependencies' folders.
			// binaries only ever move from '/bin' to '/depedencies'.


		static void Main (string[] args) {
			string dst_pattern;
			string rq_path;
			string src_pattern;
			if (!ValidateInput(args, out dst_pattern, out rq_path, out src_pattern)) return;

			var root_path = new DirectoryInfo(rq_path);

			var src_files = FindMatches(src_pattern, root_path);
			var dst_files = FindMatches(dst_pattern, root_path);

			int fails = 0;
			int copies = 0;
			int unsourced = 0;
			foreach (var filename in dst_files.Keys) {
				var match_list = dst_files[filename];
				if (match_list == null || match_list.Count < 1) continue;

				if (!src_files.ContainsKey(filename)) {
					unsourced++;
					continue;
				}

				var most_recent = src_files[filename].First(a => a.LastWriteTime == src_files[filename].Max(b => b.LastWriteTime));
				if (most_recent == null) {
					unsourced++;
					continue;
				}
				foreach (var file_info in match_list) {
					if (file_info == most_recent) throw new Exception("source was a dependency!");
					copies++;
					try {
						File.Copy(most_recent.FullName, file_info.FullName, true);
					} catch {
						Console.WriteLine("Failed to overwrite \"" + file_info.FullName + "\" with \"" + most_recent.FullName + "\"");
						fails++;
					}
				}
			}

			Console.WriteLine("Finished. " + copies + " copies attempted.");
			if (fails > 0 || unsourced > 0) Console.WriteLine(fails + " copies failed, "+unsourced+" targets had no update source");
			else Console.WriteLine("OK");
		}

		static bool ValidateInput(string[] args, out string dstPattern, out string rqPath, out string srcPattern)
		{
			if (args.Length != 3 && args.Length != 4)
			{
				ShowUsageMessage();
				dstPattern = null;rqPath = null;srcPattern = null;
				return false;
			}
			rqPath = (args.Length == 4) ? (args[1]) : (args[0]);
			srcPattern = (args.Length == 4) ? (args[2]) : (args[1]);
			dstPattern = (args.Length == 4) ? (args[3]) : (args[2]);
			if (!Directory.Exists(rqPath))
			{
				ShowPathMessage();
				return false;
			}
			return true;
		}

		private static Dictionary<string, List<FileInfo>> FindMatches (string pattern, DirectoryInfo rootPath) {
			var most_recents = new Dictionary<string, List<FileInfo>>(); // File name => all file infos

			string tmp;
			try {
				tmp = Path.GetFileName(pattern);
			} catch {
				tmp = pattern;
			}

			Debug.Assert(tmp != null, "tmp != null");
			var matches = rootPath.GetFiles(tmp, SearchOption.AllDirectories);

			foreach (var match in matches) {
				if (!match.FullName.CompareWildcard(pattern, true)) continue;
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
			Console.WriteLine("SyncDeps \"path\" \"src pattern\" \"dst pattern\"");
			Console.WriteLine();
			Console.WriteLine("Path: path to check. All files matching the pattern");
			Console.WriteLine("      in this path and sub paths will be replaced by");
			Console.WriteLine("      the most recent copy found by exact name.");
			Console.WriteLine();
			Console.WriteLine("Pattern: file name pattern to match against. May use");
			Console.WriteLine("      Wildcards '*' and '?'");
			Console.WriteLine();
			Console.WriteLine("Example:");
			Console.WriteLine("      SyncDeps \"C:\\Projects\\MyProject\\\" \"*\\bin\\Debug\\MyProject*.dll\" \"*\\Dependencies\\MyProject*.dll\" ");

			Console.WriteLine();
			Console.WriteLine("Press [ENTER] to continue...");
			Console.ReadKey();
		}
	}
}
