using System.Linq;

public static class StringExtension
{
	public static string Fmt (this string format, object arg)
	{
		return string.Format (format, arg);
	}

	public static string Fmt (this string format, object arg1, object arg2)
	{
		return string.Format (format, arg1, arg2);
	}

	public static string Fmt (this string format, object arg1, object arg2, object arg3)
	{
		return string.Format (format, arg1, arg2, arg3);
	}

	public static string Fmt (this string format, params object[] args)
	{
		return string.Format (format, args);
	}

	public static string Color (this string text, string color)
	{
		#if UNITY_EDITOR
		return "<color=" + color + ">" + text + "</color>";
		#else
		return text;
		#endif
	}

	public static string Red (this string text)
	{
		return text.Color ("red");
	}
	public static string Yellow (this string text)
	{
		return text.Color ("yellow");
	}
	public static string Blue (this string text)
	{
		return text.Color ("blue");
	}
	public static string Green (this string text)
	{
		return text.Color ("green");
	}

	public static string Lime (this string text)
	{
		return text.Color ("lime");
	}

	public static bool IsWhiteSpace (this string text)
	{
		return text.Trim () == "";
	}

	public static bool IsNullOrEmpty (this string text)
	{
		return string.IsNullOrEmpty (text);
	}

	public static bool IsNullOrWhiteSpace (this string text)
	{
		return text == null || text.Trim () == "";
	}

	public static int CountChars (this string text, char val)
	{
		return (from c in text
			where c == val
			select c).Count ();
	}
}
