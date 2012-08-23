using System.IO;

namespace SyncDeps
{
	public class StupidLogger
	{
		readonly string logFilePath;

		public StupidLogger(string filepath)
		{
			if (filepath == null)
			{
				logFilePath = null;
				return;
			}
			try
			{
				using(File.Create(filepath))
				{
				}
			}
			catch
			{
				logFilePath = null;
				return;
			}
			logFilePath = filepath;
		}

		public void Write(string message)
		{
			if (logFilePath == null) return;

			File.AppendAllText(logFilePath, message + "\r\n");
		}
	}
}
