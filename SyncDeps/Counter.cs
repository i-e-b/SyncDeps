namespace SyncDeps
{
	public class Counter
	{
		public int Fails, Copies, Manuals, Unsourced;

		public override string ToString()
		{
			var msg = "Finished. " + Copies + " copies attempted. ";
			if (Fails > 0) msg += Fails + " copies failed. ";
			if (Unsourced > 0) msg += Unsourced + " targets had no update source. ";
			if (Manuals > 0) msg += Manuals + " targets were newer than source (manual update?). ";
			return msg;
		}
	}
}