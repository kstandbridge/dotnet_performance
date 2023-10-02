
public class DataShapes
{
    public ShapeType[] Type;
    public float[] Width;
    public float[] Height;
    public int Count;

    public DataShapes(int count)
    {
        Type = new ShapeType[count];
        Width = new float[count];
        Height = new float[count];
        Count = count;
    }

    public void Set(int index, ShapeType type, float width, float height)
    {
        Type[index] = type;
        Width[index] = width;
        if ((type == ShapeType.Square) || 
            (type == ShapeType.Circle))
        {
            Height[index] = width;
        }
        else
        {
            Height[index] = height;
        }
    }
}

public class DataShapeTest : IShapeTest
{
    private DataShapes _shapes;
    private int _shapeCount;

    public DataShapeTest(DataShapes shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "DataShape";

    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int index = 0; index < _shapeCount; ++index)
        {
            switch(_shapes.Type[index])
            {
                case ShapeType.Square:      { totalArea += _shapes.Width[index]*_shapes.Height[index]; } break;
                case ShapeType.Rectangle:   { totalArea += _shapes.Width[index]*_shapes.Height[index]; } break;
                case ShapeType.Triangle:    { totalArea += 0.5f * _shapes.Width[index]*_shapes.Height[index]; } break;
                case ShapeType.Circle:      { totalArea += (float)Math.PI*_shapes.Width[index]*_shapes.Height[index]; } break;
            }
        }

        return totalArea;
    }
}

public class DataShapeTableTest : IShapeTest
{
    private DataShapes _shapes;
    private int _shapeCount;

    public DataShapeTableTest(DataShapes shapes, int shapeCount)
    {
        _shapes = shapes;
        _shapeCount = shapeCount;
    }

    public string Label => "DataShapeTable";

    private static float[] _table = {1.0f, 1.0f, 0.5f, (float)Math.PI};
    
    public float CalulateAreas()
    {
        float totalArea = 0.0f;

        for(int index = 0; index < _shapeCount; ++index)
        {
            totalArea += _table[(int)_shapes.Type[index]]*_shapes.Width[index]*_shapes.Height[index];
        }

        return totalArea;
    }
}