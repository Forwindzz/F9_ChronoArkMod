#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using System.IO;
using ChronoArkMod.InUnity;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using System.Linq;

public static class HotfixDllBuilder
{
    /*
    [MenuItem("Tools/Build No Editor DLL")]
    public static void BuildHotfixDll()
    {
        //string DLLName = "ChronoArkMod." + ChronoArkMod.InUnity.ModProjectSetting.ConvertToValidFileName(Setting.ModID) + ".ModAssembly.dll";
        string DLLName = "ChronoArkMod.RHA_Merankori.ModAssembly.dll";
        string outputPath = Path.Combine(ModProjectSetting.Setting.Info.DirectoryName, "Assemblies", DLLName);
        //string outputPath = Path.Combine(ChronoArkGameLocation.Constants.ScriptAssembliesPath, DLLName);

        //var scriptFiles = Directory.GetFiles("Assets/Scripts/", "*.cs", SearchOption.AllDirectories);

        // 临时修改 Scripting Define Symbols，去除 UNITY_EDITOR
        var originalSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, originalSymbols.Replace("UNITY_EDITOR;", ""));

        // 进行 DLL 构建
        //string outputPath = "Assets/Generated/Hotfix.dll";
        var scriptFiles = Directory.GetFiles("Assets/Scripts/", "*.cs", SearchOption.AllDirectories);

        var builder = new AssemblyBuilder(outputPath, scriptFiles)
        {
            buildTarget = EditorUserBuildSettings.activeBuildTarget,
            buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget),
            excludeReferences = new[] { "UnityEditor.dll" }
        };

        builder.buildFinished += (path, messages) =>
        {
            foreach (var m in messages)
                Debug.Log(m.message);
            Debug.Log($"Hotfix DLL built to {path}");
        };

        if (!builder.Build())
        {
            Debug.LogError("Failed to start Hotfix DLL build.");
        }

        // 还原原始的 Scripting Define Symbols
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, originalSymbols);
    }*/
}

public class CustomBuildScript
{
    [MenuItem("Build/Build Specific Asmdef Without UNITY_EDITOR Macro")]
    public static void BuildSpecificAsmdefWithoutUnityEditorMacro()
    {
        // 1. 获取当前的脚本编译宏
        string originalDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        string defineSymbolsWithoutEditor = RemoveUnityEditorMacro(originalDefineSymbols);

        // 2. 设置宏定义
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbolsWithoutEditor);

        // 3. 执行构建过程
        BuildSpecificAsmdef();

        // 4. 恢复原始宏定义
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, originalDefineSymbols);
    }

    private static string RemoveUnityEditorMacro(string currentDefineSymbols)
    {
        HashSet<string> symbols = new HashSet<string>(currentDefineSymbols.Split(';'));
        symbols.Remove("UNITY_EDITOR");
        return string.Join(";", symbols);
    }

    private static void BuildSpecificAsmdef()
    {

        BuildAssembly("Assets/Scripts/ChronoArkMod.ModAssembly.asmdef");
    }

    private static void BuildAssembly(string asmdefPath)
    {
        // 获取脚本文件
        string scriptPath = Path.GetDirectoryName(asmdefPath);
        string[] scriptFiles = Directory.GetFiles(scriptPath, "*.cs", SearchOption.AllDirectories);

        string DLLName = "ChronoArkMod.RHA_Merankori.ModAssembly.dll";
        string outputPath = Path.Combine(ModProjectSetting.Setting.Info.DirectoryName, "Assemblies", DLLName);
        string buildOutputPath = "Build/";
        Debug.Log($"Path: {outputPath}");
        string buildName = "out";
        // 设置构建选项
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new string[] { },
            target = BuildTarget.StandaloneWindows, // 设置目标平台
            locationPathName = buildOutputPath+ buildName+".exe" // 输出路径
        };

        // 执行构建
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildOptions);
        if (buildReport.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("Build failed!");
        }
        else
        {
            Debug.Log("Build successful!");
        }

        var targetPath = Path.Combine(Path.Combine(buildOutputPath+buildName+"_Data", "Managed"), DLLName);
        File.Copy(targetPath, outputPath, true);
        Debug.Log($"copy {targetPath} -> {outputPath}");
    }
}




#endif
