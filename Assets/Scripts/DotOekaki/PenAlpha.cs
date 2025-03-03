using UnityEngine;
using UnityEngine.UI;

public class PenAlpha : MonoBehaviour
{
    [SerializeField] Slider penAlphaSlider;

    private void Start()
    {
        penAlphaSlider.minValue = 0;
        penAlphaSlider.maxValue = 1;
        penAlphaSlider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        DrawingManager.instance.penAlpha = value;
    }
}
