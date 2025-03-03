using UnityEngine;
using UnityEngine.UI;

public class PenWidth : MonoBehaviour
{
    [SerializeField] private Slider penWidthSlider;

    private void Start()
    {
        penWidthSlider.value = DrawingManager.instance.brushSize;

        penWidthSlider.wholeNumbers = true; // スライダーの値を整数にする
        penWidthSlider.minValue = 1;
        penWidthSlider.maxValue = 7;
        penWidthSlider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        DrawingManager.instance.brushSize = intValue;
    }
}
