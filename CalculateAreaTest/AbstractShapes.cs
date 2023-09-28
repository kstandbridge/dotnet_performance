
public abstract class AbstractShape
{
    public abstract float Area();
}

public class Square : AbstractShape
{
    private float _side;
    public Square(float sideInit) => (_side) = (sideInit);
    public override float Area() => _side*_side;
}

public class Rectangle : AbstractShape
{
    private float _width, _height;
    public Rectangle(float widthInit, float heightInit) => (_width, _height) = (widthInit, heightInit);
    public override float Area() => _width*_height;
}

public class Triangle : AbstractShape
{
    private float _base, _height;
    public Triangle(float baseInit, float heightInit) => (_base, _height) = (baseInit, heightInit);
    public override float Area() => 0.5f*_base*_height;
}

public class Circle : AbstractShape
{
    private float _radius;
    public Circle(float radiusInit) => (_radius) = (radiusInit);
    public override float Area() => (float)Math.PI*_radius*_radius;
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