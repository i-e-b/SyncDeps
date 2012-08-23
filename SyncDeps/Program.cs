using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace SyncDeps {
	class Program {
		static void Main(string[] args)
		{
			ArgumentParser settings;
			try {
				settings = new ArgumentParser().Read(args);
			} catch (ArgumentException) {
				ShowUsageMessage();
				return;
			}

			if (!Directory.Exists(settings.BasePath))
			{
				ShowPathMessage(settings.BasePath);
				ShowUsageMessage();
				return;
			}

			var log = new StupidLogger(settings.LogPath);
			var root_path = new DirectoryInfo(settings.BasePath);
			var counter = new Counter();

			SyncroniseDependencies(log, settings, root_path, counter);

			Console.WriteLine(counter);
		}

		static void SyncroniseDependencies(StupidLogger log, ArgumentParser settings, DirectoryInfo root_path, Counter counter)
		{
			var src_files = FindMatches(settings.SourcePattern, root_path);
			var dst_files = FindMatches(settings.DestPattern, root_path);

			foreach (var filename in dst_files.Keys)
			{
				var match_list = dst_files[filename];
				if (match_list == null || match_list.Count < 1) continue;

				if (HasNoSource(log, counter, dst_files, filename, src_files)) continue;

				var newest_source = newest(src_files[filename]);

				UpdateDestinations(log, counter, newest_source, match_list);
			}
		}

		static void UpdateDestinations(StupidLogger log, Counter counter, FileInfo newest_source, IEnumerable<FileInfo> match_list)
		{
			foreach (var potential_destination in match_list)
			{
				if (potential_destination == newest_source) throw new Exception("Source was a dependency! Check your src and dst patterns don't overlap.");

				if (potential_destination.LastWriteTime > newest_source.LastWriteTime)
				{
					log.Write("Skipped \"" + potential_destination.FullName + "\" because the target was newer.");
					counter.Manuals++;
					continue;
				}

				counter.Copies++;
				try
				{
					File.Copy(newest_source.FullName, potential_destination.FullName, true);
				}
				catch
				{
					log.Write("Failed to overwrite \"" + potential_destination.FullName + "\" with \"" + newest_source.FullName + "\"");
					counter.Fails++;
				}
			}
		}

		static FileInfo newest(List<FileInfo> srcFiles)
		{
			return srcFiles.First(a => a.LastWriteTime == srcFiles.Max(b => b.LastWriteTime));
		}

		static bool HasNoSource(StupidLogger log, Counter counter, Dictionary<string, List<FileInfo>> dst_files, string filename, Dictionary<string, List<FileInfo>> src_files)
		{
			if (!src_files.ContainsKey(filename))
			{
				counter.Unsourced++;
				log.Write("No source: ");
				foreach (var filespec in dst_files[filename]) log.Write(filespec.FullName + "; ");
				log.Write("\r\n");
				return true;
			}
			return false;
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

		private static void ShowPathMessage (string basePath) {
			Console.WriteLine("Path not found or no permissions: "+basePath);
		}

		private static void ShowUsageMessage () {
			Console.WriteLine("Usage:");
			Console.WriteLine("SyncDeps \"path\" \"src pattern\" \"dst pattern\" [\"log path\"]");
			Console.WriteLine();
			Console.WriteLine("Path: path to check. All files matching the pattern");
			Console.WriteLine("      in this path and sub paths will be replaced by");
			Console.WriteLine("      the most recent copy found by exact name.");
			Console.WriteLine("      Destinations are not replaced if they are newer");
			Console.WriteLine("      than the newest source.");
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
