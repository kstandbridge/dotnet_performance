using System.Diagnostics;

namespace DotNetPerformance;

internal class Program
{
    private static void Main(string[] args)
    {
        Int64 startTime = Stopwatch.GetTimestamp();

        SampleData sampleData = SampleDataGenerator.Generate(100);

        ISomeDataParser[] parsers = new ISomeDataParser[]
        {
            new MicrosoftJsonParser(),
            new NewtonsoftJsonParser()
        };

        foreach(ISomeDataParser parser in parsers)
        {
            List<SomeData> someDatas = parser.ParseJson(sampleData.Json);
            if(!sampleData.Validate(someDatas))
            {
                Console.WriteLine($"Error: {parser.Label} is invalid");
            }
            else
            {
                Console.WriteLine($"{parser.Label} is valid");
            }
        }

        Int64 endTime = Stopwatch.GetTimestamp();
        Int64 elapsedTime = endTime - startTime;
        Double seconds = (Double)elapsedTime/(Double)Stopwatch.Frequency;
        Console.WriteLine($"Total {seconds*1000.0d:F2}ms");
    }
}