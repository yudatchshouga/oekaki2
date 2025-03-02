using UnityEngine;

public class ScreenColorPicker : MonoBehaviour
{
    public Camera targetCamera; // �F���擾���邽�߂̃^�[�Q�b�g�J����
    private Texture2D screenTexture;

    void Start()
    {
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            CaptureScreenAndGetColor(mousePosition);
        }
    }

    private void CaptureScreenAndGetColor(Vector2 screenPosition)
    {
        // �J��������X�N���[���V���b�g���L���v�`��
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        // �X�N���[���V���b�g��Texture2D�ɃR�s�[
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        // �e�N�X�`���̃s�N�Z���J���[���擾
        Vector2Int pixelPosition = new Vector2Int((int)screenPosition.x, (int)screenPosition.y);
        Color pixelColor = screenTexture.GetPixel(pixelPosition.x, pixelPosition.y);
        Debug.Log("���̐F���N���b�N������: " + pixelColor);

        // ���\�[�X�����
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }
}
