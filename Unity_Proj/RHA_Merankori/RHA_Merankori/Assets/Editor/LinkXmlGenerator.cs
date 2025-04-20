using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security;

public class LinkXmlGenerator
{
    [MenuItem("Tools/Generate Link.xml for Non-Handled Types")]
    public static void GenerateLinkXmlForNestedTypes()
    {
        // 你的 asmdef 名称（不包含扩展名）
        string assemblyName = "ChronoArkMod.RHA_Merankori.ModAssembly";
        string dllPath = Path.Combine("Library/ScriptAssemblies", assemblyName + ".dll");

        if (!File.Exists(dllPath))
        {
            Debug.LogError($"找不到 DLL: {dllPath}");
            return;
        }

        Assembly asm = Assembly.LoadFile(Path.GetFullPath(dllPath));
        if (asm == null)
        {
            Debug.LogError("加载程序集失败");
            return;
        }

        var types = asm.GetTypes()
            .Where(t => !t.IsSubclassOf(typeof(UnityEngine.Object))) // 非静态嵌套类
            .SelectMany(t => new Type[] { t, typeof(List<>).MakeGenericType(t), t.MakeArrayType() })
            .GroupBy(t => t.DeclaringType?.Namespace ?? "") // 按程序集内顶层 namespace 分组
            .ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<linker>");

        foreach (var group in types)
        {
            foreach (var root in group.GroupBy(t => t.DeclaringType))
            {
                var declaringType = root.Key;
                if (declaringType == null) continue;

                string fullTypeName = declaringType.FullName.Replace("+", "/");
                string assemblyFullName = asm.GetName().FullName;

                sb.AppendLine($"  <assembly fullname=\"{SecurityElement.Escape(assemblyFullName)}\">");
                sb.AppendLine($"    <type fullname=\"{SecurityElement.Escape(fullTypeName)}\" preserve=\"all\">");

                foreach (var nested in root)
                {
                    sb.AppendLine($"      <type fullname=\"{SecurityElement.Escape(nested.FullName)}\" preserve=\"all\" />");
                }

                sb.AppendLine($"    </type>");
                sb.AppendLine($"  </assembly>");
            }
        }

        sb.AppendLine("</linker>");

        string outputPath = "Assets/link.xml";
        File.WriteAllText(outputPath, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"link.xml 已生成: {outputPath}");
    }
}
