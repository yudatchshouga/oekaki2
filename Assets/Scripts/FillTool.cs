using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class FillTool : MonoBehaviour
{
    public static FillTool instance;
    public Texture2D texture;
    public Color fillColor;
    public bool isFillMode = false;
    private Color[] originalPixels;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalPixels = texture.GetPixels();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 uv;
            if (GetMouseUV(out uv))
            {
                int x = (int)(uv.x * texture.width);
                int y = (int)(uv.y * texture.height);
                Color targetColor = texture.GetPixel(x, y);
                FloodFill(x, y, targetColor);
                texture.Apply();
            }
        }
    }

    bool GetMouseUV(out Vector2 uv)
    {
        uv = Vector2.zero;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Renderer renderer = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
                return false;

            uv = hit.textureCoord;
            return true;
        }
        return false;
    }

    private void FloodFill(int x, int y, Color targetColor)
    { 
        Stack<Vector2> pixels = new Stack<Vector2>();
        pixels.Push(new Vector2(x, y));

        while (pixels.Count > 0)
        {
            Vector2 p = pixels.Pop();
            int px = (int)p.x;
            int py = (int)p.y;

            if (px < 0 || px >= texture.width || py < 0 || py >= texture.height)
                continue;

            Color currentColor = texture.GetPixel(px, py);
            if (currentColor != targetColor || currentColor == fillColor)
                continue;

            texture.SetPixel(px, py, fillColor);

            pixels.Push(new Vector2(px + 1, py));
            pixels.Push(new Vector2(px - 1, py));
            pixels.Push(new Vector2(px, py + 1));
            pixels.Push(new Vector2(px, py - 1));
        }
    }
}