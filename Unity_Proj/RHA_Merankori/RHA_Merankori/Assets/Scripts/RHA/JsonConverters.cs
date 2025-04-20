using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Library_SpriteStudio6.Data;
using static Script_SpriteStudio6_Root;

//TODO: Unity的反序列化很糟糕，因此我们必须引入一套额外的序列化系统，用于处理常规数据，引用则交给Unity处理

public class UnityColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("r");
        writer.WriteValue(value.r);
        writer.WritePropertyName("g");
        writer.WriteValue(value.g);
        writer.WritePropertyName("b");
        writer.WriteValue(value.b);
        writer.WritePropertyName("a");
        writer.WriteValue(value.a);
        writer.WriteEndObject();
    }

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        return new Color(
            obj["r"]?.Value<float>() ?? 0f,
            obj["g"]?.Value<float>() ?? 0f,
            obj["b"]?.Value<float>() ?? 0f,
            obj["a"]?.Value<float>() ?? 1f
        );
    }
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        return new Vector2(
            obj["x"]?.Value<float>() ?? 0f,
            obj["y"]?.Value<float>() ?? 0f
        );
    }
}

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WritePropertyName("z"); writer.WriteValue(value.z);
        writer.WriteEndObject();
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        return new Vector3(
            obj["x"]?.Value<float>() ?? 0f,
            obj["y"]?.Value<float>() ?? 0f,
            obj["z"]?.Value<float>() ?? 0f
        );
    }
}

public class RectConverter : JsonConverter<Rect>
{
    public override void WriteJson(JsonWriter writer, Rect value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WritePropertyName("width"); writer.WriteValue(value.width);
        writer.WritePropertyName("height"); writer.WriteValue(value.height);
        writer.WriteEndObject();
    }

    public override Rect ReadJson(JsonReader reader, Type objectType, Rect existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        return new Rect(
            obj["x"]?.Value<float>() ?? 0f,
            obj["y"]?.Value<float>() ?? 0f,
            obj["width"]?.Value<float>() ?? 0f,
            obj["height"]?.Value<float>() ?? 0f
        );
    }
}

public class RecursiveTypeConverter<T> : JsonConverter<T>
{
    private static readonly Dictionary<Type, FieldInfo[]> _propertyCache = new Dictionary<Type, FieldInfo[]>();

    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        SerializeObject(writer, value, serializer);
        writer.WriteEndObject();
    }

    private void SerializeObject(JsonWriter writer, object obj, JsonSerializer serializer)
    {
        if (obj == null)
            return;

        var type = obj.GetType();

        if (!_propertyCache.TryGetValue(type, out var props))
        {
            props = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            _propertyCache[type] = props;
        }
        //Debug.Log($"Write {props.Length} props for: {typeof(T).Name}:\t {type.Name} => {obj}");
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(obj);
            //Debug.Log($"  Prop Write {typeof(T).Name}:\t {type?.Name}.{prop?.Name} = {propValue} (Type={propValue?.GetType()})");
            writer.WritePropertyName(prop.Name);

            if (propValue == null)
            {
                writer.WriteNull();
                continue;
            }

            var propType = prop.FieldType;

            if (IsDefaultProcessedType(propType))
            {
                if (propType.IsEnum)
                {
                    writer.WriteValue(Convert.ToInt32(propValue)); // 或者写成 writer.WriteValue(propValue.ToString()) 输出字符串名
                }
                else
                {
                    serializer.Serialize(writer, propValue);
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propType) && propType != typeof(string))
            {
                writer.WriteStartArray();
                foreach (var item in (IEnumerable)propValue)
                {
                    if (item == null)
                    {
                        writer.WriteNull();
                        continue;
                    }

                    if (IsDefaultProcessedType(item.GetType()))
                        serializer.Serialize(writer, item);
                    else
                    {
                        writer.WriteStartObject();
                        SerializeObject(writer, item, serializer);
                        writer.WriteEndObject();
                    }

                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteStartObject();
                SerializeObject(writer, propValue, serializer);
                writer.WriteEndObject();
            }
        }
    }

    public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return default;

        JObject jObject = JObject.Load(reader);
        return (T)DeserializeObject(jObject, typeof(T));
    }

    private object DeserializeObject(JObject jObject, Type type)
    {
        object obj = Activator.CreateInstance(type);
        if (!_propertyCache.TryGetValue(type, out var props))
        {
            props = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            _propertyCache[type] = props;
        }

        foreach (var prop in props)
        {
            if (!jObject.TryGetValue(prop.Name, out var token))
                continue;

            Type propType = prop.FieldType;

            if (IsDefaultProcessedType(propType))
            {
                prop.SetValue(obj, ConvertSimpleToken(token, propType));
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propType) && propType != typeof(string))
            {
                Type elementType = null;

                if (propType.IsArray)
                {
                    // 优先处理数组类型
                    elementType = propType.GetElementType();
                    var arrayToken = token as JArray;
                    if (arrayToken != null)
                    {
                        Array array = Array.CreateInstance(elementType, arrayToken.Count);
                        for (int i = 0; i < arrayToken.Count; i++)
                        {
                            array.SetValue(arrayToken[i].ToObject(elementType), i);
                        }
                        prop.SetValue(obj, array);
                    }
                }
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // 其次处理 List<T>
                    elementType = propType.GenericTypeArguments[0];
                    Type listType = typeof(List<>).MakeGenericType(elementType);
                    IList list = (IList)token.ToObject(listType);
                    prop.SetValue(obj, list);
                }
            }
            else
            {
                var nestedObj = DeserializeObject((JObject)token, propType);
                prop.SetValue(obj, nestedObj);
            }
        }

        return obj;
    }

    private object ConvertSimpleToken(JToken token, Type targetType)
    {
        if (token == null || token.Type == JTokenType.Null)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, token.ToString(), true);
        }

        // Use Convert.ChangeType for primitives (int, float, bool, etc.)
        return Convert.ChangeType(token.ToObject(typeof(object)), targetType);
    }


    private bool IsDefaultProcessedType(Type propType)
    {
        return propType.IsPrimitive || propType == typeof(string) || propType.IsEnum;
    }
}


public class UnityObjectIgnoreJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // 判断类型是否为 Material
        return
            objectType == typeof(UnityEngine.Material)
            ;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // 跳过序列化Material
        writer.WriteNull(); // 写入空值
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return existingValue;
    }
}

public class Library_SpriteStudio6_Data_Animation_Label_Converter : JsonConverter<Library_SpriteStudio6.Data.Animation.Label>
{
    public override void WriteJson(JsonWriter writer, Library_SpriteStudio6.Data.Animation.Label value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Name");
        writer.WriteValue(value.Name);
        writer.WritePropertyName("Frame");
        writer.WriteValue(value.Frame);
        writer.WriteEndObject();
    }

    public override Library_SpriteStudio6.Data.Animation.Label ReadJson(JsonReader reader, Type objectType, Library_SpriteStudio6.Data.Animation.Label existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        return new Library_SpriteStudio6.Data.Animation.Label
        {
            Name = obj["Name"]?.ToObject<string>() ?? string.Empty,
            Frame = obj["Frame"]?.ToObject<int>() ?? 0
        };
    }
}

public class ScriptSpriteStudio6RootConverter : JsonConverter<Script_SpriteStudio6_Root>
{
    public override void WriteJson(JsonWriter writer, Script_SpriteStudio6_Root value, JsonSerializer serializer)
    {
        // Start an object
        writer.WriteStartObject();

        // Serialize only the specific fields
        writer.WritePropertyName("LimitTrack");
        serializer.Serialize(writer, value.LimitTrack);

        writer.WritePropertyName("TableInformationPlay");
        serializer.Serialize(writer, value.TableInformationPlay);

        //writer.WritePropertyName("TableControlParts");
        //serializer.Serialize(writer, value.TableControlParts);

        // End the object
        writer.WriteEndObject();
    }

    public override Script_SpriteStudio6_Root ReadJson(JsonReader reader, Type objectType, Script_SpriteStudio6_Root existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Deserialize the JSON data into a JObject
        JObject obj = JObject.Load(reader);

        // Create a new instance of Script_SpriteStudio6_Root
        Script_SpriteStudio6_Root instance = new Script_SpriteStudio6_Root();

        // Read and assign values for the specified properties
        instance.LimitTrack = obj["LimitTrack"]?.ToObject<int>() ?? 0;
        instance.TableInformationPlay = obj["TableInformationPlay"]?.ToObject<InformationPlay[]>() ?? Array.Empty<InformationPlay>();
        //instance.TableControlParts = obj["TableControlParts"]?.ToObject<Library_SpriteStudio6.Control.Animation.Parts[]>() ?? Array.Empty<Library_SpriteStudio6.Control.Animation.Parts>();

        return instance;
    }
}

[System.Serializable]
public class CellMapListWrapper
{
    public CellMap[] TableCellMap;
}

