
using System.IO;
using System.IO.Compression;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class Blob : ScriptableObject
{
	public byte[] _raw;
}

internal static class E
{
	public static BinaryReader SeekStream(this byte[] data, long offset)
	{
		return data.SeekStream(offset, data.Length - offset);
	}

	public static BinaryReader SeekStream(this byte[] data, long offset, long size)
	{
		MemoryStream stream = new MemoryStream(data);
		stream.Seek(offset, SeekOrigin.Begin);
		return new BinaryReader(stream);
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


[ScriptedImporter(1, "pak-gz")]
public class GZPakArchive : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext context)
	{
		byte[] bytes = File.ReadAllBytes(context.assetPath);

		// decompress the stream
		using (Stream readStream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
		{
			MemoryStream decompress = new MemoryStream();
			readStream.CopyTo(decompress);
			PakArchive.ImportHexen2Archive(context, decompress.ToArray());
		}
	}
}


[ScriptedImporter(1, "pak")]
public class PakArchive : ScriptedImporter
{
	private static void require(bool test)
	{
		if (!test)
		{
			throw new UnityException("nope");
		}
	}

	public struct q_header
	{
		public readonly int _offset;
		public readonly int _size;

		public int EntryCount => _size / 64;

		public q_header(BinaryReader binaryReader)
		{
			PakArchive.require('P' == binaryReader.ReadChar());
			PakArchive.require('A' == binaryReader.ReadChar());
			PakArchive.require('C' == binaryReader.ReadChar());
			PakArchive.require('K' == binaryReader.ReadChar());

			_offset = binaryReader.ReadInt32();
			_size = binaryReader.ReadInt32();

			Debug.Assert((64 * EntryCount) == _size);
		}
	}

	public struct q_file_header
	{
		public readonly string _name;
		public readonly int _offset;
		public readonly int _size;

		public q_file_header(BinaryReader binaryReader)
		{
			string name = "";

			bool live = true;
			for (int i = 0; i < 56; ++i)
			{
				// TODO; this will die horribly if it finds a multibyte char
				char c = binaryReader.ReadChar();
				if ('\0' != c && live)
				{
					name = name + c;
				}
				else
				{
					live = false;
				}
			}

			_name = name;
			_offset = binaryReader.ReadInt32();
			_size = binaryReader.ReadInt32();
		}
	}

	public override void OnImportAsset(AssetImportContext context)
	{
		ImportHexen2Archive(context, File.ReadAllBytes(context.assetPath));
	}



	private static string[] path(string full)
	{
		if (!full.Contains("/"))
		{
			return new string[] { full };
		}

		return full.take(c => c != '/').cons(path(full.drop(c => c != '/').tail()));
	}

	private static void nest(AssetImportContext context, string left, GameObject into, string[] todo, GameObject leaf)
	{


		require(1 <= todo.Length);

		if (1 == todo.Length)
		{
			leaf.name = todo[0];
			leaf.transform.parent = into.transform;
		}
		else
		{
			foreach (Transform next in into.transform)
			{
				if (next.name == todo[0])
				{
					nest(context, left + "/" + todo[0], next.gameObject, todo.tail(), leaf);
					return;
				}
			}
			{
				GameObject next = new GameObject
				{
					name = todo[0]
				};
				next.transform.parent = into.transform;
				context.AddObjectToAsset(left + "/" + todo[0], next);

				nest(context, left + "/" + todo[0], next, todo.tail(), leaf);
			}
		}
	}

	public static void ImportHexen2Archive(AssetImportContext context, byte[] bytes)
	{

		// read the header
		q_header header = new q_header(bytes.SeekStream(0));

		// create the root game-object (i guess)
		GameObject root = new GameObject
		{
			name = context.assetPath.Substring(context.assetPath.LastIndexOf('/') + 1)
		};

		string clob = "";

		// read the file entries
		using (BinaryReader stream = bytes.SeekStream(header._offset, header._size))
		{
			//for (int i = 0; i < 2 && i < header.EntryCount; ++i)
			for (int i = 0; i < header.EntryCount; ++i)
			{
				q_file_header file_header = new q_file_header(stream);

				GameObject file = new GameObject
				{
					name = file_header._name
				};


				nest(context, "", root, path(file_header._name), file);

				context.AddObjectToAsset(file_header._name, file);
			}
		}

		System.IO.File.WriteAllText(@"C:\Users\Peter\Desktop\WriteText.txt", clob);

		context.AddObjectToAsset(".pak", root);
		context.SetMainObject(root);
	}
}
