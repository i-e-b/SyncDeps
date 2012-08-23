using System;

/// <summary>
/// A set of useful extension methods for strings.
/// </summary>
public static class StringHelpers {
	/// <summary>
	/// Compare this string to a path-style wildcard mask.
	/// Mask may contain literal characters, '*' or '?'
	/// There is no way to escape wildcard characters.
	/// </summary>
	/// <param name="Mask">Pattern to check against </param>
	/// <param name="IgnoreCase">If true, case differences between this string and the mask will be ignored</param>
	/// <param name="WildString">String to check</param>
	/// <returns>True if this string fits the mask string. False otherwise.</returns>
	public static bool CompareWildcard (this string WildString, string Mask, bool IgnoreCase) {
		int i = 0, k = 0;

		while (k != WildString.Length) {
			switch (Mask[i]) {
				case '*':
					if ((i + 1) == Mask.Length) return true;

					while (k != WildString.Length) {
						if (CompareWildcard(WildString.Substring(k + 1), Mask.Substring(i + 1), IgnoreCase)) return true;
						k += 1;
					}
					return false;

				case '?':
					break;

				default:
					if (IgnoreCase == false && WildString[k] != Mask[i]) return false;
					if (IgnoreCase && Char.ToLower(WildString[k]) != Char.ToLower(Mask[i])) return false;
					break;
			}
			i += 1;
			k += 1;
		}

		if (k == WildString.Length) {
			if (i == Mask.Length || Mask[i] == ';' || Mask[i] == '*') return true;
		}

		return false;
	}


	/// <summary>
	/// Compute the edit distance (number of single character substitutions, inserts or deletes) between this and another string.
	/// Memory usage and processor time increase greatly with long strings.
	/// </summary>
	/// <returns>Number of edits (single character substitutions, inserts or deletes) between the two strings</returns>
	public static int EditDistance (this string s, string Other) {
		// http://en.wikipedia.org/wiki/Levenshtein_distance
		var t = Other;
		int n = s.Length;
		int m = t.Length;
		var d = new int[n + 1, m + 1];

		if (n == 0) return m;
		if (m == 0) return n;

		for (int i = 0; i <= n; d[i, 0] = i++) { }
		for (int j = 0; j <= m; d[0, j] = j++) { }

		for (int i = 1; i <= n; i++) {
			for (int j = 1; j <= m; j++) {
				int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
				d[i, j] = Math.Min(
					Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
					d[i - 1, j - 1] + cost);
			}
		}
		return d[n, m];
	}

	/// <summary>
	/// Converts a Hex-string into a byte array.
	/// Remember to check for network order issues!
	/// </summary>
	public static byte[] ToByteArray (this String HexString) {
		int NumberChars = HexString.Length;
		var bytes = new byte[NumberChars / 2];
		for (int i = 0; i < NumberChars; i += 2) {
			bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
		}
		return bytes;
	}

	/// <summary>
	/// Return the substring up to but not including the first instance of 'c'.
	/// If 'c' is not found, the entire string is returned.
	/// </summary>
	public static string SubstringBefore (this String src, char c) {
		if (String.IsNullOrEmpty(src)) return "";

		int idx = Math.Min(src.Length, src.IndexOf(c));
		if (idx < 0) return src;
		return src.Substring(0, idx);
	}


	/// <summary>
	/// Return the substring up to but not including the last instance of 'c'.
	/// If 'c' is not found, the entire string is returned.
	/// </summary>
	public static string SubstringBeforeLast (this String src, char c) {
		if (String.IsNullOrEmpty(src)) return "";

		int idx = Math.Min(src.Length, src.LastIndexOf(c));
		if (idx < 0) return src;
		return src.Substring(0, idx);
	}

	/// <summary>
	/// Return the substring after to but not including the first instance of 'c'.
	/// If 'c' is not found, the entire string is returned.
	/// </summary>
	public static string SubstringAfter (this String src, char c) {
		if (String.IsNullOrEmpty(src)) return "";

		int idx = Math.Min(src.Length - 1, src.IndexOf(c) + 1);
		if (idx < 0) return src;
		return src.Substring(idx);
	}


	/// <summary>
	/// Return the substring after to but not including the last instance of 'c'.
	/// If 'c' is not found, the entire string is returned.
	/// </summary>
	public static string SubstringAfterLast (this String src, char c) {
		if (String.IsNullOrEmpty(src)) return "";

		int idx = Math.Min(src.Length - 1, src.LastIndexOf(c) + 1);
		if (idx < 0) return src;
		return src.Substring(idx);
	}
}
