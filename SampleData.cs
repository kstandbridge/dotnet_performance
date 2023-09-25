namespace DotNetPerformance;

public class SampleData
{
    public List<Double> Answers { get; set; } = new List<Double>();
    public string Json { get; set; } = "";
    public Int32 Count;

    public bool Validate(List<SomeData> someDatas)
    {
        bool result = false;

        Double sum = 0;
        for(Int32 index = 0; index < Count; ++index)
        {
            if(index < someDatas.Count)
            {
                SomeData someData = someDatas[index];
                Double expected = SampleDataGenerator.GetAnswer(someData.SomeDouble, someData.SomeInt);
                Double actual = Answers[index];
                sum += expected;
                if(expected != actual)
                {
                    Console.WriteLine($"Error: answer mismatch on {index}, expected: {expected}, actual: {actual}");
                    break;
                }
            }
            else
            {
                Console.WriteLine($"Error: index out of range, expected: {index}, actual: {someDatas.Count}");
                break;
            }
        }

        if(sum != Answers[Count])
        {
            Console.WriteLine($"Error: total mismatch on, expected: {sum}, actual: {Answers[Count]}");
        }
        else
        {
            result = true;
        }

        return result;
    }
}