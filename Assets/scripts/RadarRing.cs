using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadarRing : MonoBehaviour
{
    private int vertexCount = 10000;
    private float lineWidth = 0.01f;
    private float radius;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = lineWidth;
    }

    public void DrawCircle(float diameter)
    {
        radius = diameter;
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector2 pos = new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}