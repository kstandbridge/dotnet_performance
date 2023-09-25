public class SomeData
{
    public Double SomeDouble { get; set; }
    public Int64 SomeInt { get; set; }
}

public class JsonHack
{
    public List<SomeData> SomeDatas { get; set; } = new List<SomeData>(); 
}

public class SampleData
{
    public List<Double> Answers { get; set; } = new List<Double>();
    public string Json { get; set; } = "";
    public Int32 Count;
}

internal class Program
{
    private static SampleData GenerateSampleData(Int32 count)
    {
        SampleData result = new SampleData
        {
            Count = count,
        };

        Random random = new();
        System.Text.StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("\t\"someDatas\": [");

        Double total = 0.0d;
        for(UInt32 index = 0; index < count; ++index)
        {
            Double someDouble = random.NextDouble();
            Int64 someInt = random.NextInt64();
            Double answer = someDouble*Math.Sqrt(someInt);
            result.Answers.Add(answer);
            total += answer;

            stringBuilder.Append($"\t\t{{ \"someDouble\": {someDouble}, \"someInt\": {someInt} }}");
            stringBuilder.AppendLine((index == (count - 1)) ? "" : ",");
        }
        result.Answers.Add(total);

        stringBuilder.AppendLine("\t]");
        stringBuilder.AppendLine("}");

        result.Json= stringBuilder.ToString();

        return result;
    }

    public static bool ValidateSampleData(SampleData sampleData, JsonHack? parsedJson)
    {
        bool result = false;

        if(parsedJson != null)
        {
            Double sum = 0;
            for(Int32 index = 0; index < sampleData.Count; ++index)
            {
                if(index < parsedJson.SomeDatas.Count)
                {
                    SomeData someData = parsedJson.SomeDatas[index];
                    Double expected = someData.SomeDouble*Math.Sqrt(someData.SomeInt);
                    Double actual = sampleData.Answers[index];
                    sum += expected;
                    if(expected != actual)
                    {
                        Console.WriteLine($"Error: answer mismatch on {index}, expected: {expected}, actual: {actual}");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: index out of range, expected: {index}, actual: {parsedJson.SomeDatas.Count}");
                    break;
                }
            }

            if(sum != sampleData.Answers[sampleData.Count])
            {
                Console.WriteLine($"Error: total mismatch on, expected: {sum}, actual: {sampleData.Answers[sampleData.Count]}");
            }
            else
            {
                result = true;
            }
        }
        else
        {
            Console.WriteLine("Error: failed to parse json");
        }

        return result;
    }

    private static void Main(string[] args)
    {
        SampleData sampleData = GenerateSampleData(10);
        
        {
            System.Text.Json.JsonSerializerOptions serializerOptions = new(){ PropertyNameCaseInsensitive = true };
            JsonHack? parsedJson = System.Text.Json.JsonSerializer.Deserialize<JsonHack>(sampleData.Json, serializerOptions);
            if(ValidateSampleData(sampleData, parsedJson))
            {
                Console.WriteLine($"System.Text.Json valid");
            }
        }

        {
            JsonHack? parsedJson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonHack>(sampleData.Json);
            if(ValidateSampleData(sampleData, parsedJson))
            {
                Console.WriteLine($"Newtonsoft.Json valid");
            }
        }
    }
}