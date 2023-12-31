
public struct StructShape
{
    public ShapeType type;
    public float width;
    public float height;
}

public class StructShapeTest : IShapeTest
{
    private StructShape[] _shapes;
    private int _shapeCount;

    public StructShapeTest(StructShape[] shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "StructShape";

    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int index = 0; index < _shapeCount; ++index)
        {
            StructShape shape = _shapes[index];

            switch(shape.type)
            {
                case ShapeType.Square:      { totalArea += shape.width*shape.height; } break;
                case ShapeType.Rectangle:   { totalArea += shape.width*shape.height; } break;
                case ShapeType.Triangle:    { totalArea += 0.5f * shape.width*shape.height; } break;
                case ShapeType.Circle:      { totalArea += (float)Math.PI*shape.width*shape.height; } break;
            }
        }

        return totalArea;
    }
}

public class StructShapeNoCopyTest : IShapeTest
{
    private StructShape[] _shapes;
    private int _shapeCount;

    public StructShapeNoCopyTest(StructShape[] shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "StructShapeNoCopy";

    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int index = 0; index < _shapeCount; ++index)
        {
            switch(_shapes[index].type)
            {
                case ShapeType.Square:      { totalArea += _shapes[index].width*_shapes[index].height; } break;
                case ShapeType.Rectangle:   { totalArea += _shapes[index].width*_shapes[index].height; } break;
                case ShapeType.Triangle:    { totalArea += 0.5f * _shapes[index].width*_shapes[index].height; } break;
                case ShapeType.Circle:      { totalArea += (float)Math.PI*_shapes[index].width*_shapes[index].height; } break;
            }
        }

        return totalArea;
    }
}
public class StructShapeTableTest : IShapeTest
{
    private StructShape[] _shapes;
    private int _shapeCount;

    public StructShapeTableTest(StructShape[] shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "StructShapeTable";

    private static float[] _table = {1.0f, 1.0f, 0.5f, (float)Math.PI};
    
    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int ShapeIndex = 0; ShapeIndex < _shapeCount; ++ShapeIndex)
        {
            StructShape shape = _shapes[ShapeIndex];
            
            totalArea += _table[(int)shape.type]*shape.width*shape.height;
        }

        return totalArea;
    }
}