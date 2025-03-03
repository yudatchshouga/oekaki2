using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] RawImage drawingPanel;
    public int CanvasWidth; // �L�����o�X�̉���
    public int CanvasHeight; // �L�����o�X�̏c��
    public Color drawColor; // �y���̐F
    public int brushSize; // �u���V�̑傫��
    Vector2Int? lastPoint = null; // �O��̕`��ʒu
    Stack<Color[]> undoStacks; // ���ɖ߂����߂̃X�^�b�N
    Stack<Color[]> redoStacks; // ��蒼���̂��߂̃X�^�b�N

    public int undoStacksCount { get { return undoStacks.Count; } }
    public int redoStacksCount { get { return redoStacks.Count; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Texture2D���쐬
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        // �X�^�b�N�̏�������
        undoStacks = new Stack<Color[]>();
        redoStacks = new Stack<Color[]>();

        // �e�N�X�`����������(���œh��Ԃ�)
        ClearCanvas();
        undoStacks.Push(texture.GetPixels());
        drawingPanel.texture = texture;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);
        
        if (Input.GetMouseButton(0))
        {
            DrawAtPoint(localPoint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
            if (IsInsideCanvas(localPoint))
            {
                SaveUndo();
            }
        }
    }

    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;

        // ���[�J�����W��Texture2D�̍��W�ɕϊ�
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
        {
            if (lastPoint.HasValue)
            {
                DrawLine(lastPoint.Value.x, lastPoint.Value.y, x, y); // 2��ڈȍ~�̕`��
            }
            else
            {
                DrawPoint(x, y); // �ŏ��̕`��
            }

            lastPoint = new Vector2Int(x, y);
            texture.Apply();
        }
    }

    // �ŏ��̃N���b�N�̕`��
    private void DrawPoint(int cx, int cy)
    { 
        if (brushSize == 1)
        {
            texture.SetPixel(cx, cy, drawColor);
        }
        // �u���V�̑傫���������̏ꍇ�Ɗ�̏ꍇ�ŏ����𕪂���
        else if (brushSize % 2 == 0)
        {
            int halfSize = brushSize / 2;
            for (int dx = -halfSize + 1; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize + 1; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
        else if (brushSize % 2 == 1)
        {
            int halfSize = brushSize / 2;
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
    }

    // Bresenham�̒����A���S���Y��
    private void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            DrawPoint(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    private void SaveUndo()
    {
        undoStacks.Push(texture.GetPixels()); // ���݂̏�Ԃ�ۑ�
        redoStacks.Clear(); // �V�����`�悵����Redo�����̓N���A
    }

    public void Undo()
    { 
        if (undoStacksCount > 1)
        {
            redoStacks.Push(undoStacks.Pop());
            texture.SetPixels(undoStacks.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot undo");
        }
    }
    public void Redo()
    {
        if (redoStacksCount > 0)
        {
            undoStacks.Push(redoStacks.Pop());
            texture.SetPixels(undoStacks.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot redo");
        }
    }

    // �h�b�g�G�����ׂč폜����(Undo, Redo�͂ł��Ȃ��Ȃ�)
    public void ClearCanvas()
    {
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        texture.SetPixels(clearColors);
        texture.Apply();

        if (undoStacks.Count > 0)
        {
            undoStacks.Clear();
            undoStacks.Push(texture.GetPixels());
        }

        if (redoStacks.Count > 0)
        {
            redoStacks.Clear();
        }
    }

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }
}
