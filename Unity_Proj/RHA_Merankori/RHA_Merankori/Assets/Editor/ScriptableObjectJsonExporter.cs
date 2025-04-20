using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using static Library_SpriteStudio6.Data;
using static JsonSpriteLoader;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using UnityEngine.EventSystems;




[CustomEditor(typeof(Script_SpriteStudio6_DataCellMap), true)]
public class ScriptableObjectJsonExporter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("TableCellMap 导出为 JSON"))
        {
            Script_SpriteStudio6_DataCellMap obj = target as Script_SpriteStudio6_DataCellMap;

            CellMapListWrapper temp = new CellMapListWrapper();

            temp.TableCellMap = obj.TableCellMap;
            string json = JsonUtility.ToJson(temp, true);

            string path = AssetDatabase.GetAssetPath(obj);
            string jsonPath = Path.ChangeExtension(path, "_data.json");
            File.WriteAllText(jsonPath, json);
            Debug.Log($"导出JSON成功: {jsonPath}");
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("刷新数据"))
        {
            EditorUtility.SetDirty(target);
        }
    }

}


[CustomEditor(typeof(Script_SpriteStudio6_DataAnimation), true)]
public class ScriptableObjectJsonExporterDataAnimation : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("DataAnimation 部分数据导出为 JSON"))
        {
            Script_SpriteStudio6_DataAnimation obj = target as Script_SpriteStudio6_DataAnimation;
            JsonSerializerSettings settings = JsonSpriteLoader.GetJsonSetting();
            string json = JsonConvert.SerializeObject(obj, settings);

            string path = AssetDatabase.GetAssetPath(obj);
            string jsonPath = Path.ChangeExtension(path, "_data.json");
            File.WriteAllText(jsonPath, json);
            Debug.Log($"导出JSON成功: {jsonPath}");
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("刷新数据"))
        {
            EditorUtility.SetDirty(target);
        }
    }

}


[CustomEditor(typeof(Script_SpriteStudio6_Root))]
public class Script_SpriteStudio6_RootEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Draw default inspector fields
        //DrawDefaultInspector();

        // Add a custom "Export" button
        if (GUILayout.Button("部分数据导出为 JSON"))
        {
            Script_SpriteStudio6_Root obj = target as Script_SpriteStudio6_Root;
            JsonSerializerSettings settings = JsonSpriteLoader.GetJsonSetting();
            string json = JsonConvert.SerializeObject(obj, settings);

            string path = AssetDatabase.GetAssetPath(obj);
            string jsonPath = Path.ChangeExtension(path, "_SS6Root_data.json");
            File.WriteAllText(jsonPath, json);
            Debug.Log($"导出JSON成功: {jsonPath}");
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("刷新数据"))
        {
            EditorUtility.SetDirty(target);
        }
    }
}

[CustomEditor(typeof(DepthInterpInfoList))]
public class DepthInterpInfoListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Draw default inspector fields
        //DrawDefaultInspector();

        // Add a custom "Export" button
        if (GUILayout.Button("将动画插值数据导出为 JSON"))
        {
            DepthInterpInfoList obj = target as DepthInterpInfoList;
            JsonSerializerSettings settings = JsonSpriteLoader.GetJsonSetting();
            string json = JsonConvert.SerializeObject(obj, settings);

            string path = AssetDatabase.GetAssetPath(obj);
            string jsonPath = Path.ChangeExtension(path, "_SSBlend_data.json");
            File.WriteAllText(jsonPath, json);
            Debug.Log($"导出JSON成功: {jsonPath}");
            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("刷新数据"))
        {
            EditorUtility.SetDirty(target);
        }
    }
}