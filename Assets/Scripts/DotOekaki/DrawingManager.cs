using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using Unity.Mathematics;
using Photon.Pun;
using Photon.Realtime;

public class DrawingManager : MonoBehaviourPunCallbacks
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] GameObject drawField;
    [SerializeField] RawImage drawingPanel;
    [SerializeField] Text answer;
    public int CanvasWidth; // �L�����o�X�̉���
    public int CanvasHeight; // �L�����o�X�̏c��
    public Color drawColor; // �y���̐F
    public int brushSize; // �u���V�̑傫��
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
        Spoit,
        Line,
        Circle,
        Rectrangle,
    }
    public ToolMode currentMode;

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }

    Dictionary<int, Vector2Int?> lastPoints = new Dictionary<int, Vector2Int?>();
    Dictionary<int, Color> playerColors = new Dictionary<int, Color>(); // �v���C���[���Ƃ̐F�ݒ�
    Dictionary<int, int> playerPenSizes = new Dictionary<int, int>(); // �v���C���[���Ƃ̃y���T�C�Y�ݒ�


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log("DrawingManager Start");
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

        drawer = new DrawingUtils(texture, drawColor, brushSize);

        photonView.RPC("SetDrawFieldSize", RpcTarget.All);

        string role = PlayerPrefs.GetString("role", "answerer");
        Debug.Log(role);
        if (role == "answerer")
        {
            answer.text = "����͂Ȃ�ł��傤�H";
        }
        else
        {
            answer.text = "����F���N�o���X";
        }
    }

    // �`��̈�̃T�C�Y��ݒ�
    [PunRPC]
    private void SetDrawFieldSize()
    {
        RectTransform rectTransform = drawField.GetComponent<RectTransform>();
        float aspectRatio = (float)CanvasWidth / CanvasHeight;

        if (aspectRatio > 1)
        {
            rectTransform.sizeDelta = new Vector2(900, 900 / aspectRatio);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(900 * aspectRatio, 900);
        }
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = drawingPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        // �y���c�[��
        if (currentMode == ToolMode.Pen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsInsideCanvas(localPoint))
                {
                    isDrawing = true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (isDrawing)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (IsInsideCanvas(localPoint))
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawAtPointRPC", RpcTarget.All, actorNumber, x, y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                        }
                    }
                    else
                    {
                        DrawAtPoint(localPoint);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastPoint = null;
                if (isDrawing)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        photonView.RPC("ResetLastPoint", RpcTarget.All, actorNumber);
                    }
                    SaveUndo();
                    isDrawing = false;
                }
            }
        }

        // �X�|�C�g�c�[��
        else if (currentMode == ToolMode.Spoit)
        {
            Debug.Log("Spoit");
            /*
            if (Input.GetMouseButtonDown(0))
            {
                drawColor = drawer.SpoitColor(localPoint);
                drawer = new DrawingUtils(texture, drawColor, brushSize);
            }
            */
        }

        // �h��Ԃ��c�[��
        else if (currentMode == ToolMode.Fill)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentMode == ToolMode.Fill)
                {
                    if (!IsInsideCanvas(localPoint))
                    {
                        return;
                    }
                    if (PhotonNetwork.InRoom)
                    {
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        photonView.RPC("FloodFillRPC", RpcTarget.All, actorNumber, localPoint, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                    }
                    else
                    {
                        FloodFill(localPoint);
                    }
                }
            }
        }

        // �����c�[��
        else if (currentMode == ToolMode.Line)
        {
            if (Input.GetMouseButtonUp(0))
            {
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
                        startPoint = pixelPos;
                    }
                    else
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawShapeRPC", RpcTarget.All, actorNumber, startPoint.Value.x, startPoint.Value.y, pixelPos.x, pixelPos.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                        }
                        else
                        {
                            DrawShape(startPoint.Value, pixelPos);
                        }
                        startPoint = null;
                        SaveUndo();
                    }
                }
            }
        }

        // �~�A�����`�c�[��
        else if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // �n�_�̐ݒ�
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
            if (Input.GetMouseButtonUp(0))
            {
                if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
                {
                    if (isDrawing)
                    {
                        if (!IsInsideCanvas(localPoint))
                        {
                            isDrawing = false;
                            return;
                        }
                        Vector2Int endPixel = new Vector2Int(x, y);
                        if (PhotonNetwork.InRoom)
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawShapeRPC", RpcTarget.All, actorNumber, startPoint.Value.x, startPoint.Value.y, endPixel.x, endPixel.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                        }
                        else
                        {
                            DrawShape(startPixel, endPixel);
                        }
                        SaveUndo();
                        isDrawing = false;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void DrawAtPointRPC(int actorNumber, int x, int y, float r, float g, float b, float a, int size)
    {
        Vector2Int point = new Vector2Int(x, y);
        Color color = new Color(r, g, b, a);

        // �v���C���[�̐ݒ���X�V
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        if (!lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
        }

        // �ꎞ�I�ɂ��̃v���C���[�ݒ��DrawingUtils���g��
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        if (IsInsideCanvas(new Vector2(x, y)))
        {
            if (lastPoints[actorNumber].HasValue)
            {
                tempDrawer.DrawLine(lastPoints[actorNumber].Value, point); // 2��ڈȍ~�̕`��
            }
            else
            {
                tempDrawer.DrawPoint(point); // �ŏ��̕`��
            }
            lastPoints[actorNumber] = point;
        }
        texture.Apply();
    }

    [PunRPC]
    private void ResetLastPoint(int actorNumber)
    {
        if (lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
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

    public void UndoButton()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("Undo", RpcTarget.All);
        }
        else
        {
            Undo();
        }
    }
    public void RedoButton() 
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("Redo", RpcTarget.All);
        }
        else
        {
            Redo();
        }
    }

    [PunRPC]
    private void Undo()
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
    [PunRPC]
    private void Redo()
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("ClearCanvas", RpcTarget.All);
        }
        else
        {
            ClearCanvas();
        }
        SaveUndo();
    }

    [PunRPC]
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

    private void FloodFill(Vector2 localPoint)
    {
        drawer.FloodFill(drawingPanel, localPoint);
        texture.Apply();
        SaveUndo();
    }

    [PunRPC]
    private void FloodFillRPC(int actorNumber, Vector2 localPoint, float r, float g, float b, float a, int size)
    {
        Color color = new Color(r, g, b, a);

        // �v���C���[�̐ݒ���X�V
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        // �ꎞ�I�ɂ��̃v���C���[�ݒ��DrawingUtils���g��
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        tempDrawer.FloodFill(drawingPanel, localPoint);
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
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }

    public void OnValueChangedBrushSize(Slider slider)
    {
        brushSize = (int)slider.value;
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }

    // �e�L�X�g���Z�b�g
    public void SetAnswerText(string text)
    {
        answer.text = text;
    }

    // RPC�ŕ`�悷�邽�߂̃��\�b�h(Vector2Int�^���g���Ȃ�����)
    [PunRPC]
    private void DrawShapeRPC(int actorNumber, int startX, int startY, int endX, int endY, float r, float g, float b, float a, int size)
    {
        Color color = new Color(r, g, b, a);

        // �v���C���[�̐ݒ���X�V
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        // �ꎞ�I�ɂ��̃v���C���[�ݒ��DrawingUtils���g��
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        DrawShape(new Vector2Int(startX, startY), new Vector2Int(endX, endY));
    }
}
