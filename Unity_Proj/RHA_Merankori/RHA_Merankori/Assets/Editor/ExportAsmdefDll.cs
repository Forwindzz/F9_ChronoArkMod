using System.IO;
using UnityEditor;
using UnityEngine;

public static class ExportAsmdefDll
{
    //output asmdef
    private const string AsmdefName = "RHA_merankori_unity";

    private const string ExportDirectory = "../../../RHA_Merankori/Assemblies";
    private const string ExportDirectoryAll = "../../../../Mod Projects/RHA_Merankori/bin/Debug";

    [MenuItem("Tools/Export Asmdef DLL")]
    public static void ExportHotfixDll()
    {
        string unityProjectPath = Directory.GetCurrentDirectory();
        string scriptAssembliesPath = Path.Combine(unityProjectPath, "Library/ScriptAssemblies");

        string dllPath = Path.Combine(scriptAssembliesPath, $"{AsmdefName}.dll");
        string pdbPath = Path.Combine(scriptAssembliesPath, $"{AsmdefName}.pdb");

        if (!File.Exists(dllPath))
        {
            Debug.LogError($"DLL 未找到：{dllPath}，请确认 {AsmdefName}.asmdef 存在并已被 Unity 编译。");
            return;
        }

        // 拷贝 DLL
        string exportPath = Path.Combine(unityProjectPath, ExportDirectory);
        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);
        File.Copy(dllPath, Path.Combine(exportPath, $"{AsmdefName}.dll"), true);

        // 拷贝 DLL 和 PDB
        exportPath = Path.Combine(unityProjectPath, ExportDirectoryAll);
        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);
        File.Copy(dllPath, Path.Combine(exportPath, $"{AsmdefName}.dll"), true);
        if (File.Exists(pdbPath))
            File.Copy(pdbPath, Path.Combine(exportPath, $"{AsmdefName}.pdb"), true);

        AssetDatabase.Refresh();
        Debug.Log($"已导出 {AsmdefName}.dll 到：{exportPath}");
    }
}
