using DotNetPerformance;

public enum ShapeType
{
    Square,
    Rectangle,
    Triangle,
    Circle,

    Count
}

public interface IShapeTest
{
    public string Label { get; }
    public float CalulateAreas();
}

public class ShapeTestProfiler
{
    public Profiler Profiler { get; }
    public IShapeTest Test { get; }

    public ShapeTestProfiler(Profiler profiler, IShapeTest test)
    {
        Profiler = profiler;
        Test = test;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        Random random = new Random();

        Console.WriteLine("");

        int shapeCount = (int)Math.Pow(2, 16);
        bool IsValid = true;

        Console.WriteLine($"Generating {shapeCount} shapes...");
        float expectedAreas = 0.0f;
        AbstractShape[] abstractShapes = new AbstractShape[shapeCount];
        for(int ShapeIndex = 0; ShapeIndex < shapeCount; ++ShapeIndex)
        {
            ShapeType type = (ShapeType)random.Next(0, (int)ShapeType.Count);
            switch(type)
            {
                case ShapeType.Square:
                {
                    float sideInit = random.NextSingle();
                    abstractShapes[ShapeIndex] = new Square(sideInit);
                } break;

                case ShapeType.Rectangle:
                {
                    float widthInit = random.NextSingle();
                    float heightInit = random.NextSingle();
                    abstractShapes[ShapeIndex] = new Rectangle(widthInit, heightInit);
                } break;

                case ShapeType.Triangle:
                {
                    float baseInit = random.NextSingle();
                    float heightInit = random.NextSingle();
                    abstractShapes[ShapeIndex] = new Triangle(baseInit, heightInit);
                } break;

                case ShapeType.Circle:
                {
                    float radiusInit = random.NextSingle();
                    abstractShapes[ShapeIndex] = new Circle(radiusInit);
                } break;
                
                case ShapeType.Count:
                default:
                {
                    Console.WriteLine($"ERROR: Tried to create invalid shape {type}");
                    IsValid = false;
                } break;
            }
            expectedAreas += abstractShapes[ShapeIndex].Area();
        }

        ShapeTestProfiler[] testProfilers = new ShapeTestProfiler[]
        {
            new(new Profiler().WithTargetEntities(shapeCount), new AbstractShapeTest(abstractShapes, shapeCount)),
        };

        if(IsValid)
        {
            foreach(ShapeTestProfiler entry in testProfilers)
            {
                Console.WriteLine($"Validing {entry.Test.Label}...");
                float actualAreas = entry.Test.CalulateAreas();
                if(expectedAreas != actualAreas)
                {
                    Console.WriteLine("ERROR: areas mismatch for AbstractShapes");
                    IsValid = false;
                }
            }
        }

        if(IsValid)
        {
            Console.WriteLine("Profling...");
            // for(;;)
            {
                foreach(ShapeTestProfiler entry in testProfilers)
                {
                    Console.Write($"\n--- {entry.Test.Label} ---\n");
                    entry.Profiler.NewTestWave(1);
                    while(entry.Profiler.IsTesting())
                    {
                        entry.Profiler.Begin();
                        entry.Test.CalulateAreas();
                        entry.Profiler.End();

                        entry.Profiler.CountEntities(shapeCount);
                    }
                }
            }
        }

        Console.WriteLine("");
    }
}