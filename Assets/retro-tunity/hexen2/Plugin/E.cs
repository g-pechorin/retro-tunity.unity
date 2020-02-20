
using System;
using System.IO;
using System.IO.Compression;

public static class E
{
	public static BinaryReader SeekStream(this byte[] data, long offset)
	{
		return data.SeekStream(offset, data.Length - offset);
	}

	public static BinaryReader SeekStream(this byte[] data, long offset, long size)
	{
		byte[] subset = new byte[size];
		Array.Copy(data, offset, subset, 0, size);
		return new BinaryReader(new MemoryStream(subset));
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

	public static byte[] pizg(this byte[] data)
	{
		// decompress the stream
		using (Stream readStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
		{
			MemoryStream decompress = new MemoryStream();
			readStream.CopyTo(decompress);
			return decompress.ToArray();
		}

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

	public delegate O Fun<A0, O>(A0 a0);

	public static string drop(this string text, Fun<char, bool> q)
	{
		return ("" != text && q(text[0]))
			? text.Substring(1).drop(q)
			: text;
	}


	public static string take(this string text, Fun<char, bool> q)
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


