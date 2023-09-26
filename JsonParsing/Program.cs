namespace DotNetPerformance;

internal class Program
{
    private static void Main(string[] args)
    {
        SampleData sampleData = SampleDataGenerator.Generate(1000);

        ISomeDataParser[] parsers = new ISomeDataParser[]
        {
            new MicrosoftJsonParser(),
            new NewtonsoftJsonParser()
        };

        bool AreParsersValid = true;
        foreach(ISomeDataParser parser in parsers)
        {
            List<SomeData> someDatas = parser.ParseJson(sampleData.Json);
            if(!sampleData.Validate(someDatas))
            {
                Console.WriteLine($"Error: {parser.Label} is invalid");
                AreParsersValid = false;
                break;
            }
        }

        if(AreParsersValid == true)
        {
            foreach(ISomeDataParser parser in parsers)
            {
                Console.Write($"\n--- {parser.Label} ---\n");
                Profiler profiler = new Profiler();
                profiler.NewTestWave(sampleData.Json.Length, 1);
                while(profiler.IsTesting())
                {
                    profiler.Begin();
                    parser.ParseJson(sampleData.Json);
                    profiler.End();

                    profiler.CountBytes(sampleData.Json.Length);
                }
            }
        }

        Console.WriteLine("");
    }
}