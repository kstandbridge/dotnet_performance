namespace DotNetPerformance;

public static class SampleDataGenerator
{
    private static Random _random = new();

    public static Double GetAnswer(Double someDouble, Int64 someInt)
    {
        Double result = someDouble*Math.Sqrt(someInt);
        return result;
    }

    public static SampleData Generate(Int32 count)
    {
        SampleData result = new SampleData
        {
            Count = count,
        };

        System.Text.StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("\t\"someDatas\": [");

        Double total = 0.0d;
        for(UInt32 index = 0; index < count; ++index)
        {
            Double someDouble = _random.NextDouble();
            Int64 someInt = _random.NextInt64();
            Double answer = GetAnswer(someDouble, someInt);
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
}
