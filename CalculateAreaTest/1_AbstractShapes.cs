
public abstract class AbstractShape
{
    public float Width { get; set; }
    public float Height { get; set; }
    public virtual float Area() => Width*Height;

    public AbstractShape(float width, float height)
    {
        Width = width;
        Height = height;
    }
}

public class Square : AbstractShape
{
    public Square(float side) : base(side, side)
    {
    }
}

public class Rectangle : AbstractShape
{
    public Rectangle(float width, float height) : base(width, height)
    {
    }
}

public class Triangle : AbstractShape
{
    public Triangle(float width, float height) : base(width, height)
    {
    }

    public override float Area() => 0.5f*Width*Height;
}

public class Circle : AbstractShape
{
    public Circle(float radius) : base(radius, radius)
    {
    }

    public override float Area() => (float)Math.PI*Width*Height;
}

public class AbstractShapeTest : IShapeTest
{
    private AbstractShape[] _shapes;
    private int _shapeCount;

    public AbstractShapeTest(AbstractShape[] shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "AbstractShape";

    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int ShapeIndex = 0; ShapeIndex < _shapeCount; ++ShapeIndex)
        {
            totalArea += _shapes[ShapeIndex].Area();
        }

        return totalArea;
    }
}
