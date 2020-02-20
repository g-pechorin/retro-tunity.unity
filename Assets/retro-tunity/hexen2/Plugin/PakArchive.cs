
using System;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;


public static class PakArchive
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

		public byte[] read(byte[] full)
		{
			byte[] subset = new byte[_size];
			Array.Copy(full, _offset, subset, 0, _size);
			return subset;
		}
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

		// create the root game-object (i guess)
		GameObject root = new GameObject
		{
			name = context.assetPath.Substring(context.assetPath.LastIndexOf('/') + 1)
		};

		ParseHexen2PakFile(bytes, file_header =>
		{
			GameObject file = new GameObject
			{
				name = file_header._name
			};


			nest(context, "", root, path(file_header._name), file);

			context.AddObjectToAsset(file_header._name, file);
		});

		context.AddObjectToAsset(".pak", root);
		context.SetMainObject(root);
	}

	public delegate void NoteWad(q_file_header file);
	public static void ParseHexen2PakFile(byte[] bytes, NoteWad noteWad)
	{
		// read the header
		q_header header = new q_header(bytes.SeekStream(0));

		// read the file entries
		for (int i = 0; i < header.EntryCount; ++i)
		{
			using (BinaryReader stream = bytes.SeekStream(header._offset + (64 * i), 64))
			{
				noteWad(new q_file_header(stream));
			}
		}
	}
}
