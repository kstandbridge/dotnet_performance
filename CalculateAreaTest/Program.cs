using DotNetPerformance;

internal class Program
{
    public enum ShapeType
    {
        Square,
        Rectangle,
        Triangle,
        Circle,

        Count
    }

    private static void Main(string[] args)
    {
        Random random = new Random();

        Console.WriteLine("");

        int shapeCount = 65536;
        bool IsValid = true;

        Console.WriteLine($"Generating {shapeCount} shapes...");
        float expectedAreas = 0.0f;
        BaseShape[] shapes = new BaseShape[shapeCount];
        for(int ShapeIndex = 0; ShapeIndex < shapeCount; ++ShapeIndex)
        {
            ShapeType type = (ShapeType)random.Next(0, (int)ShapeType.Count);
            switch(type)
            {
                case ShapeType.Square:
                {
                    float sideInit = random.NextSingle();
                    shapes[ShapeIndex] = new Square(sideInit);
                } break;

                case ShapeType.Rectangle:
                {
                    float widthInit = random.NextSingle();
                    float heightInit = random.NextSingle();
                    shapes[ShapeIndex] = new Rectangle(widthInit, heightInit);
                } break;

                case ShapeType.Triangle:
                {
                    float baseInit = random.NextSingle();
                    float heightInit = random.NextSingle();
                    shapes[ShapeIndex] = new Triangle(baseInit, heightInit);
                } break;

                case ShapeType.Circle:
                {
                    float radiusInit = random.NextSingle();
                    shapes[ShapeIndex] = new Circle(radiusInit);
                } break;
                
                case ShapeType.Count:
                default:
                {
                    Console.WriteLine($"ERROR: Tried to create invalid shape {type}");
                    IsValid = false;
                } break;
            }
            expectedAreas += shapes[ShapeIndex].Area();
        }

        if(IsValid)
        {
            Console.WriteLine("Validating logic...");
            float actualAreas = 0.0f;
            for(int ShapeIndex = 0; ShapeIndex < shapeCount; ++ShapeIndex)
            {
                actualAreas += shapes[ShapeIndex].Area();
            }
            if(expectedAreas != actualAreas)
            {
                Console.WriteLine("ERROR: areas mismatch for AbstractShapes");
                IsValid = false;
            }
        }

        if(IsValid)
        {
            Console.WriteLine("Profling...");
            // for(;;)
            {
                Profiler profiler = new Profiler();
                profiler.NewTestWave(0, 1);

                Console.Write("\n--- AbstractShapes ---\n");
                while(profiler.IsTesting())
                {
                    profiler.Begin();
                    float totalArea = 0.0f;
                    for(int ShapeIndex = 0; ShapeIndex < shapeCount; ++ShapeIndex)
                    {
                        totalArea += shapes[ShapeIndex].Area();
                    }
                    profiler.End();
                }


            }

        }

        Console.WriteLine("");
    }
    

}