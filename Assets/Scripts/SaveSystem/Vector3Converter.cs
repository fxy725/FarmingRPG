using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        JObject jo = new JObject
        {
            { "x", value.x },
            { "y", value.y },
            { "z", value.z }
        };
        jo.WriteTo(writer);
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        float x = jo["x"]?.Value<float>() ?? 0;
        float y = jo["y"]?.Value<float>() ?? 0;
        float z = jo["z"]?.Value<float>() ?? 0;
        return new Vector3(x, y, z);
    }
}
