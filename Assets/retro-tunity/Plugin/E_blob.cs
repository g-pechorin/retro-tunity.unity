using System;
using System.IO;
using System.IO.Compression;

public static class E_blob
{
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
}
