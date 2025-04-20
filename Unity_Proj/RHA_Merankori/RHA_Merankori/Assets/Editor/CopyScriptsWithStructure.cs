using UnityEditor;
using UnityEngine;
using System.IO;

public class CopyScriptsWithStructure
{
    
    private const string targetCopyToRoot = "../../../../Mod Projects/RHA_Merankori/UnityScripts";
    private const string assetScanRoot = "Content";

    [MenuItem("Tools/Export All C# Scripts (Preserve Structure)")]
    public static void CopyAllScriptsWithStructure()
    {
        string targetRoot = Path.Combine(Application.dataPath, targetCopyToRoot);
        if (!Directory.Exists(targetRoot))
        {
            Directory.CreateDirectory(targetRoot);
        }

        string assetRoot = Path.Combine(Application.dataPath, assetScanRoot); 
        string[] csFiles = Directory.GetFiles(assetRoot, "*.cs", SearchOption.AllDirectories);

        int copiedCount = 0;

        foreach (string srcPath in csFiles)
        {
            if (srcPath.EndsWith(".meta")) continue;

            string relativePath = srcPath.Substring(assetRoot.Length + 1); // +1 for slash
            string destPath = Path.Combine(targetRoot, relativePath);

            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(srcPath, destPath, overwrite: true);
            copiedCount++;
        }

        Debug.Log($"Copy {copiedCount} C# to {targetRoot}");
        EditorUtility.DisplayDialog("Export Finished!", $"Copy {copiedCount} Scripts\nPath:\n{targetRoot}", "OK!");
    }
}
