using UnityEngine;
using System.Collections.Generic;

public class LineDrawing : MonoBehaviour
{
    public Camera cam;
    public GameObject linePrefab;
    public float lineWidth = 0.1f;

    public LineRenderer currentLineRenderer;
    private List<Vector3> points;

    public int index = 0;

    void Start()
    {
        points = new List<Vector3>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateNewLine();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePosition) > 0.1f)
            {
                points.Add(mousePosition);
                currentLineRenderer.positionCount = points.Count;
                currentLineRenderer.SetPosition(points.Count - 1, mousePosition);
            }
        }
    }

    void CreateNewLine()
    {
        GameObject lineObject = Instantiate(linePrefab);
        currentLineRenderer = lineObject.GetComponent<LineRenderer>();
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.useWorldSpace = true;
        //É}ÉeÉäÉAÉãÇÃê›íË
        currentLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        if (index == 0)
        {
            currentLineRenderer.startColor = Color.red;
            currentLineRenderer.endColor = Color.red;
        } else
        {
            currentLineRenderer.startColor = Color.yellow;
            currentLineRenderer.endColor = Color.yellow;
        }
        points.Clear();
    }
}