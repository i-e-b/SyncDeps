using System.Collections.Generic;

namespace SyncDeps
{
	public static class DictionaryHelpers
	{
		/// <summary>
		/// Return a value by key, or return null
		/// </summary>
		public static TValue Of<TValue,TKey>(this Dictionary<TKey, TValue> src, TKey key) where TValue:class
		{
			return src.ContainsKey(key) ? src[key] : default(TValue);
		}
	}
}
