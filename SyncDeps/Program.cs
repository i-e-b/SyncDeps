﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SyncDeps {
	class Program {
			// provide a list of '/bin' folders and a list of '/dependencies' folders.
			// binaries only ever move from '/bin' to '/depedencies'.


		static void Main (string[] args) {
			#region Validation
			if (args.Length != 3 && args.Length != 4) {
				ShowUsageMessage();
				return;
			}
			var rq_path = (args.Length == 4) ? (args[1]) : (args[0]);
			var src_pattern = (args.Length == 4) ? (args[2]) : (args[1]);
			var dst_pattern = (args.Length == 4) ? (args[3]) : (args[2]);
			if (!Directory.Exists(rq_path)) {
				ShowPathMessage();
				return;
			}
			#endregion

			var root_path = new DirectoryInfo(rq_path);

			var src_files = FindMatches(src_pattern, root_path);
			var dst_files = FindMatches(dst_pattern, root_path);

			int fails = 0;
			int copies = 0;
			foreach (var filename in dst_files.Keys) {
				var match_list = dst_files[filename];
				if (match_list == null || match_list.Count < 1) continue;

				if (!src_files.ContainsKey(filename)) {
					//Console.WriteLine("Couldn't find a source for \"" + filename + "\"");
					continue;
				}

				var most_recent = src_files[filename].First(a => a.LastWriteTime == src_files[filename].Max(b => b.LastWriteTime));
				if (most_recent == null) {
					//Console.WriteLine("Couldn't find a source for \""+filename+"\"");
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
			if (fails > 0) Console.WriteLine(fails + " copies failed");
			else Console.WriteLine("OK");
		}

		private static Dictionary<string, List<FileInfo>> FindMatches (string Pattern, DirectoryInfo RootPath) {
			var most_recents = new Dictionary<string, List<FileInfo>>(); // File name => all file infos

			var tmp = Pattern;
			try {
				tmp = Path.GetFileName(Pattern);
			} catch {
				tmp = Pattern;
			}

			var matches = RootPath.GetFiles(tmp, SearchOption.AllDirectories);

			foreach (var match in matches) {
				if (!match.FullName.CompareWildcard(Pattern, true)) continue;
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
			Console.WriteLine("MostRecentOnly \"path\" \"src pattern\" \"dst pattern\"");
			Console.WriteLine();
			Console.WriteLine("Path: path to check. All files matching the pattern");
			Console.WriteLine("      in this path and sub paths will be replaced by");
			Console.WriteLine("      the most recent copy found by exact name.");
			Console.WriteLine();
			Console.WriteLine("Pattern: file name pattern to match against. May use");
			Console.WriteLine("      Wildcards '*' and '?'");
			Console.WriteLine();
			Console.WriteLine("Example:");
			Console.WriteLine("      MostRecentOnly \"C:\\Projects\\Twofour.MediaFreedom\\\" \"*\\bin\\Debug\\Twofour.MediaFreedom*.dll\" \"*\\Dependencies\\Twofour Assemblies\\Twofour.MediaFreedom.*.dll\" ");

			Console.WriteLine();
			Console.WriteLine("Press [ENTER] to continue...");
			Console.ReadKey();
		}
	}
}