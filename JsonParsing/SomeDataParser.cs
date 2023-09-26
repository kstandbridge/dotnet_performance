namespace DotNetPerformance;

public class JsonHack
{
    public List<SomeData> SomeDatas { get; set; } = new List<SomeData>(); 
}

public interface ISomeDataParser
{
    string Label { get; }
    public List<SomeData> ParseJson(string json);
}

public class MicrosoftJsonParser : ISomeDataParser
{
    public string Label => "System.Text.Json";

    public List<SomeData> ParseJson(string json)
    {
        List<SomeData> result = new();

        System.Text.Json.JsonSerializerOptions serializerOptions = new(){ PropertyNameCaseInsensitive = true };
        JsonHack? parsedJson = System.Text.Json.JsonSerializer.Deserialize<JsonHack>(json, serializerOptions);
        if(parsedJson != null)
        {
            result = parsedJson.SomeDatas;
        }

        return result;
    }
}

public class NewtonsoftJsonParser : ISomeDataParser
{
    public string Label => "Newtonsoft.Json";

    public List<SomeData> ParseJson(string json)
    {
        List<SomeData> result = new();

        JsonHack? parsedJson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonHack>(json);
        if(parsedJson != null)
        {
            result = parsedJson.SomeDatas;
        }

        return result;
    }
}