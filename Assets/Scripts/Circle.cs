using UnityEngine;

public class Circle
{
    public float x;
    public float y;
    public float bend;
    public float radius;
    public Complex center;

    public Circle(float bend, float x, float y)
    {
        this.center = new Complex(x, y);
        this.bend = bend;
        this.radius = Mathf.Abs(1f / this.bend);
    }

    public float Distance(Circle other)
    {
        float dx = this.center.a - other.center.a;
        float dy = this.center.b - other.center.b;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public void Draw(LineRenderer lineRenderer, int steps = 32)
    {
        lineRenderer.positionCount = steps;
        for (int i = 0; i < steps; i++)
        {
            float angle = 2f * Mathf.PI * i / steps;
            float pointX = this.center.a + this.radius * Mathf.Cos(angle);
            float pointY = this.center.b + this.radius * Mathf.Sin(angle);
            lineRenderer.SetPosition(i, new Vector3(pointX, pointY, 0));
        }
    }
}