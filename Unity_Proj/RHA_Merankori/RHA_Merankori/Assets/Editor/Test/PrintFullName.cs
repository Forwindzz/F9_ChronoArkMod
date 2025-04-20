using Newtonsoft.Json;
using System;
using Test;
using UnityEditor;
using UnityEngine;
using static Test.StaticClass2.StaticClass;

namespace Test
{
    [System.Serializable]
    public static class StaticClass2
    {
        [System.Serializable]
        public static class StaticClass
        {

            [System.Serializable]
            public struct MyStruct
            {
                public int i_v;
                public float f_value;
                public string s_value;
            }
        }
    }
    

    [System.Serializable]
    public class MyClass
    {
        public MyStruct data;
        public MyStruct[] datas;
    }

}

public class PrintFullName : EditorWindow
{
    [MenuItem("Tools/Test/Test_Json")]
    private static void PrintSelectedObjectFullName()
    {
        MyClass obj = new MyClass();
        obj.data.i_v = 233;
        int count = 3;
        obj.datas = new MyStruct[count];
        for (int i = 0; i < count; i++)
        {
            obj.datas[i] = new MyStruct()
            {
                i_v = i,
                f_value = i * 0.01f,
                s_value = i.ToString() +"#"
            };
        }
        Debug.Log("JsonUtility Result:");
        string json = JsonUtility.ToJson(obj);
        MyClass result = JsonUtility.FromJson<MyClass>(json);
        Debug.Log(json);

        Debug.Log("JsonUtility Result s->de->s:");
        Debug.Log(JsonUtility.ToJson(json));

        Debug.Log("NewtonJson Result");
        json = JsonConvert.SerializeObject(obj);
        Debug.Log(json);

        Debug.Log("NewtonJson Result s->de->s:");
        result = JsonConvert.DeserializeObject<MyClass>(json);
        Debug.Log(JsonConvert.SerializeObject(result));
    }
}
