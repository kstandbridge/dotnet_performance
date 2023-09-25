using System.Diagnostics;

namespace DotNetPerformance;

public class JsonHack
{
    public List<SomeData> SomeDatas { get; set; } = new List<SomeData>(); 
}

internal class Program
{
    private static void Main(string[] args)
    {
        Int64 startTime = Stopwatch.GetTimestamp();

        SampleData sampleData = SampleDataGenerator.Generate(100);
        
        {
            System.Text.Json.JsonSerializerOptions serializerOptions = new(){ PropertyNameCaseInsensitive = true };
            JsonHack? parsedJson = System.Text.Json.JsonSerializer.Deserialize<JsonHack>(sampleData.Json, serializerOptions);
            if((parsedJson == null) || 
               !sampleData.Validate(parsedJson.SomeDatas))
            {
                Console.WriteLine($"Error: System.Text.Json invalid");
            }
        }

        {
            JsonHack? parsedJson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonHack>(sampleData.Json);
            if((parsedJson == null) ||
               !sampleData.Validate(parsedJson.SomeDatas))
            {
                Console.WriteLine($"Error: Newtonsoft.Json invalid");
            }
        }

        Int64 endTime = Stopwatch.GetTimestamp();
        Int64 elapsedTime = endTime - startTime;
        Double seconds = (Double)elapsedTime/(Double)Stopwatch.Frequency;
        Console.WriteLine($"Total {seconds*1000.0d:F2}ms");
    }
}