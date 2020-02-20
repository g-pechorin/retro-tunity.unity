
using System.IO;
using UnityEditor.Experimental.AssetImporters;

[ScriptedImporter(1, "pak")]
public class PakImport : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext context)
	{
		PakArchive.ImportHexen2Archive(context, File.ReadAllBytes(context.assetPath));
	}
}
