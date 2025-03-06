using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using Unity.Mathematics;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] RawImage drawingPanel;
    public int CanvasWidth; // �L�����o�X�̉���
    public int CanvasHeight; // �L�����o�X�̏c��
    public Color drawColor; // �y���̐F
    public int brushSize; // �u���V�̑傫��
    public float penAlpha; // �y���̓����x(�v��Ȃ���������Ȃ�)
    Vector2Int? lastPoint = null; // �O��̕`��ʒu
    Stack<Color[]> undoStack; // ���ɖ߂����߂̃X�^�b�N
    Stack<Color[]> redoStack; // ��蒼���̂��߂̃X�^�b�N
    Vector2Int? startPoint = null; // �������[�h�̎n�_
    Vector2Int startPixel; // �~���[�h�A�����`���[�h�̎n�_
    bool isDrawing = false; // �`�撆���ǂ���
    public enum ToolMode
    {
        Pen,
        Fill,
        Line,
        Circle,
        Rectrangle,
    }
    public ToolMode currentMode;

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CanvasWidth = PlayerPrefs.GetInt("Width", 10);
        CanvasHeight = PlayerPrefs.GetInt("Height", 10);

        // Texture2D���쐬
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // �h�b�g�G�������胂�[�hON

        // �X�^�b�N�̏�������
        undoStack = new Stack<Color[]>();
        redoStack = new Stack<Color[]>();

        // �e�N�X�`����������(���œh��Ԃ�)
        Color[] colors = new Color[CanvasWidth * CanvasHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(1, 1, 1, 0);
        }
        texture.SetPixels(colors);
        texture.Apply();

        undoStack.Push(texture.GetPixels());
        drawingPanel.texture = texture;

        // �Q�[���J�n���̓y�����[�h
        currentMode = ToolMode.Pen;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = drawingPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == ToolMode.Pen)
            { 
                if (IsInsideCanvas(localPoint))
                {
                    isDrawing = true;
                }
            }

            if (currentMode == ToolMode.Fill)
            {
                FloodFill(localPoint);
            }

            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
            { 
                if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                {
                    return;
                }
                Vector2Int pixelPos = new Vector2Int(x, y);
                // �n�_��ݒ�
                if (!isDrawing)
                { 
                    startPixel = pixelPos;
                    isDrawing = true;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (currentMode == ToolMode.Pen) 
            {
                DrawAtPoint(localPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
            if (currentMode == ToolMode.Pen)
            {
                if (isDrawing)
                {
                    SaveUndo();
                    isDrawing = false;
                }
            }

            if (currentMode == ToolMode.Line)
            {
                if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                {
                    return;
                }
                Vector2Int pixelPos = new Vector2Int(x, y);
                // ���ڂ̃N���b�N�Ŏn�_��ݒ�
                if (startPoint == null)
                {
                    DrawPoint(pixelPos.x, pixelPos.y);
                    startPoint = pixelPos;
                }
                else
                {
                    DrawLine(startPoint.Value.x, startPoint.Value.y, pixelPos.x, pixelPos.y);
                    startPoint = null;
                    SaveUndo();
                }
            }

            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
            {
                if (isDrawing)
                {
                    if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                    {
                        return;
                    }
                    Vector2Int endPixel = new Vector2Int(x, y);
                    DrawShape(startPixel, endPixel);
                    SaveUndo();
                    isDrawing = false;
                }
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

    private void DrawPoint(int cx, int cy)
    {
        /*
        // �y���̓����x��ݒ�
        Color newColor = drawColor;
        newColor.a = penAlpha;
        Color brushColor = Color.Lerp(Color.white, newColor, newColor.a);
        */

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
        texture.Apply();
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

    private void DrawShape(Vector2Int start, Vector2Int end)
    {
        if (currentMode == ToolMode.Circle)
        {
            DrawCircle(start, end);
        }
        else if (currentMode == ToolMode.Rectrangle)
        {
            DrawRectangle(start, end);
        }
    }

    // Bresenham�̑ȉ~�A���S���Y��
    private void DrawCircle(Vector2Int start, Vector2Int end)
    {
        int centerX = (start.x + end.x) / 2;
        int centerY = (start.y + end.y) / 2;
        int radiusX = Mathf.Abs(centerX - start.x);
        int radiusY = Mathf.Abs(centerY - start.y);

        int x, y;
        float dx, dy, d1, d2;

        x = 0;
        y = radiusY;

        // �����(x����,y���)
        d1 = (radiusY * radiusY) - (radiusX * radiusX * radiusY) + (0.25f * radiusX * radiusX);
        dx = 2 * radiusY * radiusY * x;
        dy = 2 * radiusX * radiusX * y;

        while (dx < dy)
        { 
            DrawPoint(centerX + x, centerY + y);
            DrawPoint(centerX - x, centerY + y);
            DrawPoint(centerX + x, centerY - y);
            DrawPoint(centerX - x, centerY - y);

            x++;
            dx += 2 * radiusY * radiusY;
            if (d1 < 0)
            {
                d1 += dx + radiusY * radiusY;
            }
            else
            {
                y--;
                dy -= 2 * radiusX * radiusX;
                d1 += dx - dy + radiusY * radiusY;
            }
        }
        // �����(x���,y����)
        d2 = ((radiusY * radiusY) * ((x + 0.5f) * (x + 0.5f))) + ((radiusX * radiusX) * ((y - 1) * (y - 1))) - (radiusX * radiusX * radiusY * radiusY);

        while (y >= 0)
        {
            DrawPoint(centerX + x, centerY + y);
            DrawPoint(centerX - x, centerY + y);
            DrawPoint(centerX + x, centerY - y);
            DrawPoint(centerX - x, centerY - y);

            y--;
            dy -= 2 * radiusX * radiusX;
            if (d2 > 0)
            {
                d2 += radiusX * radiusX - dy;
            }
            else
            {
                x++;
                dx += 2 * radiusY * radiusY;
                d2 += dx - dy + radiusX * radiusX;
            }
        }
        texture.Apply();
    }

    private void DrawRectangle(Vector2Int start, Vector2Int end)
    { 
        int xMin = Mathf.Min(start.x, end.x);
        int xMax = Mathf.Max(start.x, end.x);
        int yMin = Mathf.Min(start.y, end.y);
        int yMax = Mathf.Max(start.y, end.y);

        for (int x = xMin; x <= xMax; x++)
        {
            DrawPoint(x, start.y);
            DrawPoint(x, end.y);
        }
        for (int y = yMin; y <= yMax; y++)
        {
            DrawPoint(start.x, y);
            DrawPoint(end.x, y);
        }
        texture.Apply();
    }

    private void SaveUndo()
    {
        undoStack.Push(texture.GetPixels()); // ���݂̏�Ԃ�ۑ�
        redoStack.Clear(); // �V�����`�悵����Redo�����̓N���A
    }

    public void Undo()
    { 
        if (undoStackCount > 1)
        {
            redoStack.Push(undoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot undo");
        }
    }
    public void Redo()
    {
        if (redoStackCount > 0)
        {
            undoStack.Push(redoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot redo");
        }
    }

    // �h�b�g�G�����ׂč폜����
    public void ClearCanvas()
    {
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = new Color(1, 1, 1, 0);
        }
        texture.SetPixels(clearColors);
        texture.Apply();

        SaveUndo();
    }

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }

    // �h��Ԃ��A���S���Y��
    public void FloodFill(Vector2 localPoint)
    {
        // �L�����o�X�O���N���b�N�����ꍇ�͉������Ȃ�
        if (!IsInsideCanvas(localPoint))
        {
            return;
        }

        Rect rect = drawingPanel.rectTransform.rect;

        // ���[�J�����W��Texture2D�̍��W�ɕϊ�
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        Color targetColor = texture.GetPixel(x, y);
        // �N���b�N�����ꏊ�̐F�Ɠh��Ԃ��F�������ꍇ�͉������Ȃ�
        if (targetColor == drawColor)
        {
            return;
        }

        Queue<Vector2Int> pixelQueue = new Queue<Vector2Int>();
        pixelQueue.Enqueue(new Vector2Int(x, y));

        while (pixelQueue.Count > 0)
        {
            Vector2Int pos = pixelQueue.Dequeue();

            if (pos.x < 0 || pos.x >= texture.width || pos.y < 0 || pos.y >= texture.height)
                continue;

            if (texture.GetPixel(pos.x, pos.y) != targetColor)
                continue;

            texture.SetPixel(pos.x, pos.y, drawColor);

            pixelQueue.Enqueue(new Vector2Int(pos.x + 1, pos.y));
            pixelQueue.Enqueue(new Vector2Int(pos.x - 1, pos.y));
            pixelQueue.Enqueue(new Vector2Int(pos.x, pos.y + 1));
            pixelQueue.Enqueue(new Vector2Int(pos.x, pos.y - 1));
        }
        texture.Apply();
        SaveUndo();
    }

    public void ChangeMode(ToolMode mode)
    {
        currentMode = mode;
        isDrawing = false;
    }

    public void ChangeColor(Color color)
    {
        drawColor = color;
    }
}
