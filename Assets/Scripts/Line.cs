using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;

public class LineDrawing : MonoBehaviourPunCallbacks
{
    public Camera cam;
    public GameObject linePrefab;
    public float lineWidth = 0.1f;
    public Slider penWidthSlider;

    private LineRenderer currentLineRenderer;
    private List<Vector3> points;

    // �`�������̗�����ێ����郊�X�g
    private List<GameObject> lineHistory;
    // Redo �p�ɕێ����郊�X�g
    private List<GameObject> redoHistory;

    public int index = 0;

    // �`�揇���C���N�������g���邽�߂̃J�E���^�[
    private int sortingOrderCounter = 0;

    public ClickChecker clickChecker;

    void Start()
    {
        points = new List<Vector3>();
        penWidthSlider.onValueChanged.AddListener(OnPenWidthChanged);
        lineHistory = new List<GameObject>();
        redoHistory = new List<GameObject>();
    }

    bool isClickedFrame = false;

    void Update()
    {
        // �}�E�X�̍��N���b�N��
        if (Input.GetMouseButtonDown(0))
        {
            isClickedFrame = clickChecker.IsClickedFrame();
            Debug.Log("isClickedFrame: " + isClickedFrame);
            if (!isClickedFrame)
            {
                CreateNewLine();
                Debug.Log("CreateNewLine");
            }
            //CreateNewLine();
        }

        // �}�E�X�̍��N���b�N�������Ă����
        if (Input.GetMouseButton(0) && !isClickedFrame)
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

        // �}�E�X�̍��N���b�N�𗣂�����
        if (Input.GetMouseButtonUp(0))
        {
            currentLineRenderer = null;
            //����
            photonView.RPC("SyncLineData", RpcTarget.Others, points.ToArray(), index, lineWidth);
        }
    }

    void CreateNewLine()
    {
        GameObject lineObject = Instantiate(linePrefab);
        currentLineRenderer = lineObject.GetComponent<LineRenderer>();

        lineWidth = penWidthSlider.value;
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.useWorldSpace = true;
        //�}�e���A���̐ݒ�
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

        // �`�������𗚗��ɒǉ�
        lineHistory.Add(lineObject);
        // Redo �p�̗������N���A
        redoHistory.Clear();

        // Sorting Order �݂̂ŏ����𐧌�
        sortingOrderCounter++;
        currentLineRenderer.sortingOrder = sortingOrderCounter;
    }

    // === ��M��: `RPC` �ō��W���󂯎�� ===
    [PunRPC]
    void SyncLineData(Vector3[] points, int colorIndex, float lineWidth)
    {
        Debug.Log("SyncLineData");
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        if (colorIndex == 0)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        else
        {
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    // �X���C�_�[�̒l���ύX���ꂽ���ɌĂ΂��
    void OnPenWidthChanged(float value)
    {
        if (currentLineRenderer != null)
        {
            currentLineRenderer.startWidth = value;
            currentLineRenderer.endWidth = value;
        }
    }

    // ==== ��߂�iUndo�j ====
    public void UndoLastLine()
    {
        //Debug.Log("UndoLastLine");
        if (lineHistory.Count > 0)
        {
            //Debug.Log("lineHistory.Count > 0");
            Debug.Log(lineHistory.Count);
            // �����̍Ō�̐����擾
            GameObject lastLine = lineHistory[lineHistory.Count - 1];

            // ��\���ɂ��� Redo �p�ɕێ�
            lastLine.SetActive(false);
            redoHistory.Add(lastLine);

            // ��������폜
            lineHistory.RemoveAt(lineHistory.Count - 1);
        }
    }

    // ==== ��i�߂�iRedo�j ====
    public void RedoLastLine()
    {
        if (redoHistory.Count > 0)
        {
            // Redo ���X�g�̍Ō�̃I�u�W�F�N�g���擾
            GameObject lastRedoLine = redoHistory[redoHistory.Count - 1];

            // �ĕ\������ LineHistory �ɖ߂�
            lastRedoLine.SetActive(true);
            lineHistory.Add(lastRedoLine);

            // Redo ���X�g����폜
            redoHistory.RemoveAt(redoHistory.Count - 1);
        }
    }
}