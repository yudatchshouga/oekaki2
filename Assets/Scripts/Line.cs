using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LineDrawing : MonoBehaviour
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

    

    void Update()
    {
        bool isOutside = clickChecker.IsClickedOutsidePanel();
        // マウスの左クリック時
        if (Input.GetMouseButtonDown(0))
        {
            //isOutside = clickChecker.IsClickedOutsidePanel();
            Debug.Log("LineManager 側: Panel の外側をクリックしたか: " + isOutside);
            if (isOutside)
            {
                CreateNewLine();
                Debug.Log("CreateNewLine");
            }
            //CreateNewLine();
        }

        // マウスの左クリックを押している間
        if (Input.GetMouseButton(0) && isOutside)
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

        float lineWidth = penWidthSlider.value;
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

        // === 修正点: Sorting Order のみで順序を制御 ===
        sortingOrderCounter++;
        currentLineRenderer.sortingOrder = sortingOrderCounter;
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