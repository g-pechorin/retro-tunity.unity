
using System.IO;
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
			// compute the path where we'll store this
			string path = context.assetPath.replaceAll("/[^/]+$", "/_pak/") + file._name;

			// ignore this file (but we should be keeping the .meta)
			GitIgnore.Ignore = path;

			// create any parent directories
			Directory.CreateDirectory(path.flip().drop(c => c != '/').flip());

			// dump the file itself
			File.WriteAllBytes(path, file.read(pak));

			// tell Unity to import the file
			AssetDatabase.ImportAsset(path);
		});
	}
}