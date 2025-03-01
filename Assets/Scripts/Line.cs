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

    // 描いた線の履歴を保持するリスト
    private List<GameObject> lineHistory;
    // Redo 用に保持するリスト
    private List<GameObject> redoHistory;

    public int index = 0;

    // 描画順をインクリメントするためのカウンター
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
        // マウスの左クリック時
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

        // マウスの左クリックを押している間
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

        // マウスの左クリックを離した時
        if (Input.GetMouseButtonUp(0))
        {
            currentLineRenderer = null;
            //同期
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
        //マテリアルの設定
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

        // 描いた線を履歴に追加
        lineHistory.Add(lineObject);
        // Redo 用の履歴をクリア
        redoHistory.Clear();

        // Sorting Order のみで順序を制御
        sortingOrderCounter++;
        currentLineRenderer.sortingOrder = sortingOrderCounter;
    }

    // === 受信側: `RPC` で座標を受け取る ===
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

    // スライダーの値が変更された時に呼ばれる
    void OnPenWidthChanged(float value)
    {
        if (currentLineRenderer != null)
        {
            currentLineRenderer.startWidth = value;
            currentLineRenderer.endWidth = value;
        }
    }

    // ==== 一つ戻る（Undo） ====
    public void UndoLastLine()
    {
        //Debug.Log("UndoLastLine");
        if (lineHistory.Count > 0)
        {
            //Debug.Log("lineHistory.Count > 0");
            Debug.Log(lineHistory.Count);
            // 履歴の最後の線を取得
            GameObject lastLine = lineHistory[lineHistory.Count - 1];

            // 非表示にして Redo 用に保持
            lastLine.SetActive(false);
            redoHistory.Add(lastLine);

            // 履歴から削除
            lineHistory.RemoveAt(lineHistory.Count - 1);
        }
    }

    // ==== 一つ進める（Redo） ====
    public void RedoLastLine()
    {
        if (redoHistory.Count > 0)
        {
            // Redo リストの最後のオブジェクトを取得
            GameObject lastRedoLine = redoHistory[redoHistory.Count - 1];

            // 再表示して LineHistory に戻す
            lastRedoLine.SetActive(true);
            lineHistory.Add(lastRedoLine);

            // Redo リストから削除
            redoHistory.RemoveAt(redoHistory.Count - 1);
        }
    }
}