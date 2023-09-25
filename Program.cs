using System.Text;
using System.Text.Json;

public class SomeData
{
    public Double SomeDouble { get; set; }
    public Int64 SomeInt { get; set; }
}

public class JsonHack
{
    public List<SomeData> SomeDatas { get; set; } = new List<SomeData>(); 
}

internal class Program
{
    private static void Main(string[] args)
    {
        Random random = new Random();
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("\t\"someDatas\": [");

        List<Double> answers = new List<double>();
        Double total = 0.0d;
        Int32 count = 10;
        for(UInt32 index = 0; index < count; ++index)
        {
            Double someDouble = random.NextDouble();
            Int64 someInt = random.NextInt64();
            Double answer = someDouble*Math.Sqrt(someInt);
            answers.Add(answer);
            total += answer;

            stringBuilder.Append($"\t\t{{ \"someDouble\": {someDouble}, \"someInt\": {someInt} }}");
            stringBuilder.AppendLine((index == (count - 1)) ? "" : ",");
        }
        answers.Add(total);

        stringBuilder.AppendLine("\t]");
        stringBuilder.AppendLine("}");

        string json = stringBuilder.ToString();

        JsonHack? parsedJson = JsonSerializer.Deserialize<JsonHack>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if(parsedJson != null)
        {
            Double sum = 0;
            for(Int32 index = 0; index < count; ++index)
            {
                if(index < parsedJson.SomeDatas.Count)
                {
                    SomeData someData = parsedJson.SomeDatas[index];
                    Double expected = someData.SomeDouble*Math.Sqrt(someData.SomeInt);
                    Double actual = answers[index];
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

            if(sum != answers[count])
            {
                Console.WriteLine($"Error: total mismatch on, expected: {sum}, actual: {answers[count]}");
            }
        }
        else
        {
            Console.WriteLine("Error: failed to parse json");
        }
    }
}