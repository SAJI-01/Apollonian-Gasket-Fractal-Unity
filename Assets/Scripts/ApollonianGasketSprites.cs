using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApollonianGasketSprites : MonoBehaviour
{
    private List<Circle> allCircles = new List<Circle>();
    private List<List<Circle>> queue = new List<List<Circle>>();
    private List<GameObject> circleObjects = new List<GameObject>();
    
    [Header("Circle Sprite Settings")]
    [SerializeField] private Sprite circleSprite;
    public Color circleColor = Color.white;
    [SerializeField] private Material circleMaterial;
    
    [Header("Generation Settings")]
    [SerializeField] private float epsilon = 0.1f;
    [SerializeField] private float minRadius = 2f;
    [SerializeField] private float screenWidth = 800f;
    [SerializeField] private float screenHeight = 800f;

    private bool isGenerating = false;
    private bool isComplete = false;
    private Coroutine generationCoroutine;

    void Start()
    {
        CreateNewCircles();
    }

    public void CreateNewCircles()
    {
        if (generationCoroutine != null)
        {
            StopCoroutine(generationCoroutine);
        }

        // Clear existing circle objects
        foreach (var obj in circleObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        circleObjects.Clear();
        allCircles.Clear();
        queue.Clear();
        isComplete = false;

        // Initialize first circle centered on screen
        var c1 = new Circle(-1f / (screenWidth / 2f), screenWidth / 2f, screenHeight / 2f);
        
        // Second circle positioned randomly within the first
        float r2 = UnityEngine.Random.Range(100f, c1.radius / 2f);
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float vx = (c1.radius - r2) * Mathf.Cos(angle);
        float vy = (c1.radius - r2) * Mathf.Sin(angle);
        var c2 = new Circle(1f / r2, screenWidth / 2f + vx, screenHeight / 2f + vy);
        
        // Third circle positioned relative to the first
        float r3 = Mathf.Sqrt(vx * vx + vy * vy);
        float vx2 = -(c1.radius - r3) * Mathf.Cos(angle);
        float vy2 = -(c1.radius - r3) * Mathf.Sin(angle);
        var c3 = new Circle(1f / r3, screenWidth / 2f + vx2, screenHeight / 2f + vy2);

        allCircles.Add(c1);
        allCircles.Add(c2);
        allCircles.Add(c3);
        queue.Add(new List<Circle> { c1, c2, c3 });

        // Draw initial circles
        CreateCircleSprite(c1);
        CreateCircleSprite(c2);
        CreateCircleSprite(c3);

        // Start automatic generation
        generationCoroutine = StartCoroutine(GenerateNextGeneration());
    }

    private void CreateCircleSprite(Circle circle)
    {
        GameObject circleObj = new GameObject($"Circle_{circleObjects.Count}");
        circleObj.transform.SetParent(transform);
        
        // Add SpriteRenderer component
        SpriteRenderer spriteRenderer = circleObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = circleSprite;
        spriteRenderer.color = circleColor;
        spriteRenderer.material = circleMaterial;
        
        // Set position and scale
        circleObj.transform.position = new Vector3(circle.center.a, circle.center.b, 0);
        float diameter = circle.radius * 2;
        circleObj.transform.localScale = new Vector3(diameter, diameter, 1);
        
        circleObjects.Add(circleObj);
    }

    private IEnumerator GenerateNextGeneration()
    {
        while (queue.Count > 0 && !isComplete)
        {
            int allCircleCount = allCircles.Count;
            var nextQueue = new List<List<Circle>>();

            foreach (var triplet in queue)
            {
                var k4 = Descartes(triplet[0], triplet[1], triplet[2]);
                var newCircles = ComplexDescartes(triplet[0], triplet[1], triplet[2], k4);

                foreach (var newCircle in newCircles)
                {
                    if (Validate(newCircle, triplet[0], triplet[1], triplet[2]))
                    {
                        allCircles.Add(newCircle);
                        CreateCircleSprite(newCircle);

                        nextQueue.Add(new List<Circle> { triplet[0], triplet[1], newCircle });
                        nextQueue.Add(new List<Circle> { triplet[0], triplet[2], newCircle });
                        nextQueue.Add(new List<Circle> { triplet[1], triplet[2], newCircle });
                    }
                }
            }

            queue = nextQueue;

            if (allCircles.Count == allCircleCount)
            {
                Debug.Log("Generation complete!");
                isComplete = true;
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private bool Validate(Circle c4, Circle c1, Circle c2, Circle c3)
    {
        // Discard too small circles
        if (c4.radius < minRadius) return false;

        foreach (var other in allCircles)
        {
            float d = c4.Distance(other);
            float radiusDiff = Mathf.Abs(c4.radius - other.radius);
            
            if (d < epsilon && radiusDiff < epsilon)
            {
                return false;
            }
        }

        if (!IsTangent(c4, c1)) return false;
        if (!IsTangent(c4, c2)) return false;
        if (!IsTangent(c4, c3)) return false;

        return true;
    }

    private bool IsTangent(Circle c1, Circle c2)
    {
        float d = c1.Distance(c2);
        float r1 = c1.radius;
        float r2 = c2.radius;
        
        var a = Mathf.Abs(d - (r1 + r2)) < epsilon;
        var b = Mathf.Abs(d - Mathf.Abs(r2 - r1)) < epsilon;
        return a || b;
    }

    private List<Circle> ComplexDescartes(Circle c1, Circle c2, Circle c3, (float, float) k4)
    {
        var k1 = c1.bend;
        var k2 = c2.bend;
        var k3 = c3.bend;
        var z1 = c1.center;
        var z2 = c2.center;
        var z3 = c3.center;
        
        var zk1 = z1.scale(k1);
        var zk2 = z2.scale(k2);
        var zk3 = z3.scale(k3);
        var sum = zk1.add(zk2).add(zk3);

        var root = zk1.mult(zk2).add(zk2.mult(zk3)).add(zk1.mult(zk3));
        root = root.sqrt().scale(2);
        
        var center1 = sum.add(root).scale(1f / k4.Item1);
        var center2 = sum.sub(root).scale(1f / k4.Item1);
        var center3 = sum.add(root).scale(1f / k4.Item2);
        var center4 = sum.sub(root).scale(1f / k4.Item2);

        return new List<Circle>
        {
            new Circle(k4.Item1, center1.a, center1.b),
            new Circle(k4.Item1, center2.a, center2.b),
            new Circle(k4.Item2, center3.a, center3.b),
            new Circle(k4.Item2, center4.a, center4.b)
        };
    }

    private (float, float) Descartes(Circle c1, Circle c2, Circle c3)
    {
        float k1 = c1.bend;
        float k2 = c2.bend;
        float k3 = c3.bend;

        float sum = k1 + k2 + k3;
        float root = 2 * Mathf.Sqrt(k1 * k2 + k2 * k3 + k1 * k3);
        
        return (sum + root, sum - root);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CreateNewCircles();
        }
    }
} 