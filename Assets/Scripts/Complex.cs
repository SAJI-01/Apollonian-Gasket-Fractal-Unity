using UnityEngine;

public class Complex
{
    public float a;
    public float b;

    public Complex(float a, float b)
    {
        this.a = a;
        this.b = b;
    }
    
    public Complex add(Complex other)
    {
        return new Complex(this.a + other.a, this.b + other.b);
    }
    
    public Complex sub(Complex other)
    {
        return new Complex(this.a - other.a, this.b - other.b);
    }
    
    public Complex mult(Complex other)
    {
        var a = this.a * other.a - this.b * other.b;
        var b = this.a * other.b + this.b * other.a;
        return new Complex(a, b);
    }
    
    public Complex scale(float scalarValue)
    {
        return new Complex(this.a * scalarValue, this.b * scalarValue);
    }

    public Complex sqrt()
    {
        var magnitude = Mathf.Sqrt(this.a * this.a + this.b * this.b);
        var angle = Mathf.Atan2(this.b, this.a);
        magnitude = Mathf.Sqrt(magnitude);
        angle = angle / 2;
        return new Complex(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
    }
}