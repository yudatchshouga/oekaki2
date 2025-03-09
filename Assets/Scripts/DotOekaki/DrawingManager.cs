using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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
    DrawingUtils drawer;
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
        CanvasWidth = PlayerPrefs.GetInt("Width", 50);
        CanvasHeight = PlayerPrefs.GetInt("Height", 50);

        // Texture2D���쐬
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // �h�b�g�G�������胂�[�h

        // �X�^�b�N�̏�������
        undoStack = new Stack<Color[]>();
        redoStack = new Stack<Color[]>();

        // �e�N�X�`����������
        ClearCanvas();

        undoStack.Push(texture.GetPixels());
        drawingPanel.texture = texture;

        // �Q�[���J�n���̓y�����[�h
        currentMode = ToolMode.Pen;
    }

    private void Update()
    {
        /*
        // �y���̓����x��ݒ�
        Color newColor = drawColor;
        newColor.a = penAlpha;
        Color brushColor = Color.Lerp(Color.white, newColor, newColor.a);
        DrawingUtils.DrawPoint(texture, x, y, brushSize, brushColor);
        */

        drawer = new DrawingUtils(texture, drawColor, brushSize);

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

            // �h��Ԃ��c�[��
            if (currentMode == ToolMode.Fill)
            {
                FloodFill(localPoint);
            }

            // �~�A�����`�c�[���̎n�_�̐ݒ�
            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
            { 
                if (!IsInsideCanvas(localPoint))
                {
                    return;
                }
                Vector2Int pixelPos = new Vector2Int(x, y);
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

            // �����c�[��
            if (currentMode == ToolMode.Line)
            {
                if (!IsInsideCanvas(localPoint))
                {
                    return;
                }
                Vector2Int pixelPos = new Vector2Int(x, y);
                // ���ڂ̃N���b�N�Ŏn�_��ݒ�
                if (startPoint == null)
                {
                    drawer.DrawPoint(pixelPos);
                    texture.Apply();
                    startPoint = pixelPos;
                }
                else
                {
                    DrawShape(startPoint.Value, pixelPos);
                    startPoint = null;
                    SaveUndo();
                }
            }

            // �~�A�����`�c�[��
            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
            {
                if (isDrawing)
                {
                    if (!IsInsideCanvas(localPoint))
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

        if (IsInsideCanvas(localPoint))
        {
            if (lastPoint.HasValue)
            {
                drawer.DrawLine(lastPoint.Value, new Vector2Int(x, y)); // 2��ڈȍ~�̕`��
            }
            else
            {
                drawer.DrawPoint(new Vector2Int(x, y)); // �ŏ��̕`��
            }

            lastPoint = new Vector2Int(x, y);
        }
        texture.Apply();
    }

    private void DrawShape(Vector2Int start, Vector2Int end)
    {
        if (currentMode == ToolMode.Line)
        {
            drawer.DrawLine(start, end);
        }
        else if (currentMode == ToolMode.Circle)
        {
            drawer.DrawCircle(start, end);
        }
        else if (currentMode == ToolMode.Rectrangle)
        {
            drawer.DrawRectangle(start, end);
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

    public void AllClear()
    {
        ClearCanvas();
        SaveUndo();
    }

    private void ClearCanvas()
    {
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = new Color(1, 1, 1, 0);
        }
        texture.SetPixels(clearColors);
        texture.Apply();
    }

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }

    // �h��Ԃ�
    private void FloodFill(Vector2 localPoint)
    {
        // �L�����o�X�O���N���b�N�����ꍇ�͉������Ȃ�
        if (!IsInsideCanvas(localPoint))
        {
            return;
        }

        drawer.FloddFill(drawingPanel, localPoint);
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
