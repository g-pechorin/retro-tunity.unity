
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "pak-gz")]
public class GZPakImport : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext context)
	{
		if (null != context)
		{
			throw new UnityException("this is out of date - needs to ignore copyrighted data");
		}

		PakArchive.ImportHexen2Archive(context, File.ReadAllBytes(context.assetPath).pizg());
	}
}
