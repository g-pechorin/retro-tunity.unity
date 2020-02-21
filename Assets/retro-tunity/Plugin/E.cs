
using System;
using System.Text.RegularExpressions;

public static class E
{
	public static void require(bool test)
	{
		if (!test)
		{
			throw new Exception("nope");
		}
	}

	public static string replaceAll(this string from, string regex, string pattern)
	{
		return Regex.Replace(from, regex, pattern);
	}

	public static string flip(this string text)
	{
		string output = "";

		foreach (char c in text)
		{
			output = c + output;
		}

		return output;
	}

	public static T[] flip<T>(this T[] data)
	{
		T[] output = new T[data.Length];

		for (int i = 0; i < data.Length; ++i)
		{
			output[i] = data[data.Length - (i + 1)];
		}

		return output;
	}

	public delegate O Map<I, O>(I i);

	public static string drop(this string text, Map<char, bool> q)
	{
		return ("" != text && q(text[0]))
			? text.Substring(1).drop(q)
			: text;
	}


	public static string take(this string text, Map<char, bool> q)
	{
		return ("" != text && q(text[0]))
			? text[0] + text.tail().take(q)
			: "";
	}

	public static string tail(this string text)
	{
		return ("" != text) ? text.Substring(1) : text;
	}

	public static T[] tail<T>(this T[] full)
	{
		T[] output = new T[full.Length - 1];

		for (int i = 1; i < full.Length; ++i)
		{
			output[i - 1] = full[i];
		}

		return output;
	}

	public static T[] cons<T>(this T head, T[] tail)
	{
		T[] output = new T[tail.Length + 1];

		output[0] = head;

		for (int i = 0; i < tail.Length; ++i)
		{
			output[i + 1] = tail[i];
		}

		return output;
	}
}


