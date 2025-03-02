using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class FillTool : MonoBehaviour
{
    public static FillTool instance;

    [SerializeField] RawImage DrawerPanel;
    Texture2D texture;
    public Color fillColor;
    public bool isFillMode = false;
    private Color[] originalPixels;

    private void Awake()
    {
        instance = this;
        texture = DrawerPanel.texture as Texture2D;
    }

    void Update()
    {
        if (isFillMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;
                Vector2Int pixelPos = ScreenToTextureCoord(texture, mousePos, DrawerPanel.rectTransform);
                Color targetColor = GetPixelColor(texture, pixelPos);
                if (targetColor != fillColor)
                {
                    FloodFillQueue(texture, pixelPos, targetColor, fillColor);
                }
            }
        }
    }

    // pixelPosition にあるピクセルの色を取得する
    Color GetPixelColor(Texture2D texture, Vector2Int pixelPosition)
    { 
        return texture.GetPixel(pixelPosition.x, pixelPosition.y);
    }

    void FloodFillQueue(Texture2D texture, Vector2Int startPos, Color targetColor, Color fillColor)
    { 
        Queue<Vector2Int> pixels = new Queue<Vector2Int>();
        pixels.Enqueue(startPos);

        while (pixels.Count > 0)
        { 
            Vector2Int pos = pixels.Dequeue();

            if (pos.x < 0 || pos.x >= texture.width || pos.y < 0 || pos.y >= texture.height)
                continue;

            if (texture.GetPixel(pos.x, pos.y) != targetColor)
                continue;

            texture.SetPixel(pos.x, pos.y, fillColor);

            pixels.Enqueue(new Vector2Int(pos.x + 1, pos.y));
            pixels.Enqueue(new Vector2Int(pos.x - 1, pos.y));
            pixels.Enqueue(new Vector2Int(pos.x, pos.y + 1));
            pixels.Enqueue(new Vector2Int(pos.x, pos.y - 1));
        }
        texture.Apply();
    }

    // RawImageのスクリーン座標をTexture2Dのピクセル座標に変換する
    Vector2Int ScreenToTextureCoord(Texture2D texture, Vector2 screenPosition, RectTransform rectTransform)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPos);

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float pixelX = ((localPos.x + width / 2) / width) * texture.width;
        float pixelY = ((localPos.y + height / 2) / height) * texture.height;

        return new Vector2Int(Mathf.FloorToInt(pixelX), Mathf.FloorToInt(pixelY));
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
}