
public abstract class BaseShape
{
    public abstract float Area();
}

public class Square : BaseShape
{
    private float _side;
    public Square(float sideInit) => (_side) = (sideInit);
    public override float Area() => _side*_side;
}

public class Rectangle : BaseShape
{
    private float _width, _height;
    public Rectangle(float widthInit, float heightInit) => (_width, _height) = (widthInit, heightInit);
    public override float Area() => _width*_height;
}

public class Triangle : BaseShape
{
    private float _base, _height;
    public Triangle(float baseInit, float heightInit) => (_base, _height) = (baseInit, heightInit);
    public override float Area() => 0.5f*_base*_height;
}

public class Circle : BaseShape
{
    private float _radius;
    public Circle(float radiusInit) => (_radius) = (radiusInit);
    public override float Area() => (float)Math.PI*_radius*_radius;
}
