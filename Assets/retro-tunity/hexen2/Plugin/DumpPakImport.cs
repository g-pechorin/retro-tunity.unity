
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

[ScriptedImporter(1, "gz-pak")]
public class DumpPakImport : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext context)
	{
		GitIgnore.Ignore = context.assetPath;

		byte[] pak = File.ReadAllBytes(context.assetPath).pizg();
		PakArchive.ParseHexen2PakFile(pak, file =>
		{
			// compute the pat where we'll store this
			string path = context.assetPath + "_target/" + file._name;
			GitIgnore.Ignore = path;

			Directory.CreateDirectory(path.flip().drop(c => c != '/').flip());

			File.WriteAllBytes(path, file.read(pak));
			AssetDatabase.ImportAsset(path);
		});
	}
}