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
        int shapeCount = (int)Math.Pow(2, 24);

        Random random = new Random();

        Console.WriteLine("");

        bool IsValid = true;

        Console.WriteLine($"Generating {shapeCount} shapes...");
        float expectedAreas = 0.0f;
        AbstractShape[] abstractShapes = new AbstractShape[shapeCount];
        StructShape[] structShapes = new StructShape[shapeCount];
        DataShapes dataShapes = new DataShapes(shapeCount);
        for(int shapeIndex = 0; shapeIndex < shapeCount; ++shapeIndex)
        {
            ShapeType type = (ShapeType)random.Next(0, (int)ShapeType.Count);
            float width = random.NextSingle();
            float height = random.NextSingle();
            switch(type)
            {
                case ShapeType.Square:    { abstractShapes[shapeIndex] = new Square(width); } break;
                case ShapeType.Rectangle: { abstractShapes[shapeIndex] = new Rectangle(width, height); } break;
                case ShapeType.Triangle:  { abstractShapes[shapeIndex] = new Triangle(width, height); } break;
                case ShapeType.Circle:    { abstractShapes[shapeIndex] = new Circle(width); } break;
                case ShapeType.Count:
                default:
                {
                    Console.WriteLine($"ERROR: Tried to create invalid shape {type}");
                    IsValid = false;
                } break;
            }
            expectedAreas += abstractShapes[shapeIndex].Area();

            structShapes[shapeIndex].type = type;
            structShapes[shapeIndex].width = width;
            if ((type == ShapeType.Square) || 
                (type == ShapeType.Circle))
            {
                structShapes[shapeIndex].height = width;
            }
            else
            {
                structShapes[shapeIndex].height = height;
            }

            dataShapes.Set(shapeIndex, type, width, height);
        }

        ShapeTestProfiler[] testProfilers = new ShapeTestProfiler[]
        {
            new(new Profiler().WithTargetEntities(shapeCount), new AbstractShapeTest(abstractShapes, shapeCount)),
            new(new Profiler().WithTargetEntities(shapeCount), new StructShapeTest(structShapes, shapeCount)),
            new(new Profiler().WithTargetEntities(shapeCount), new StructShapeNoCopyTest(structShapes, shapeCount)),
            new(new Profiler().WithTargetEntities(shapeCount), new StructShapeTableTest(structShapes, shapeCount)),
            new(new Profiler().WithTargetEntities(shapeCount), new DataShapeTest(dataShapes, shapeCount)),
            new(new Profiler().WithTargetEntities(shapeCount), new DataShapeTableTest(dataShapes, shapeCount)),
        };

        if(IsValid)
        {
            foreach(ShapeTestProfiler entry in testProfilers)
            {
                Console.WriteLine($"Validing {entry.Test.Label}...");
                float actualAreas = entry.Test.CalulateAreas();
                if(expectedAreas != actualAreas)
                {
                    Console.WriteLine($"ERROR: areas mismatch for {entry.Test.Label} expected: \"{expectedAreas:F2}\" actual: \"{actualAreas:F2}\"");
                    IsValid = false;
                }
            }
        }

#if !DEBUG
        if(IsValid)
        {
            Console.WriteLine("Profling...");
            for(;;)
            {
                foreach(ShapeTestProfiler entry in testProfilers)
                {
                    Console.Write($"\n--- {entry.Test.Label} ---\n");
                    entry.Profiler.NewTestWave();
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
#endif

        Console.WriteLine("");
    }
}