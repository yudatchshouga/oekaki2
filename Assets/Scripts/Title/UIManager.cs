using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle hostToggle;
    [SerializeField] Toggle guestToggle;
    [SerializeField] GameObject passwordMaching;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button applyButton;

    private void Start()
    {
        hostToggle.interactable = false;
        guestToggle.interactable = false;
        passwordInputField.interactable = false;
        applyButton.interactable = false;
        passwordMaching.SetActive(false);
    }

    public void OnClickPasswordMatchingButton()
    { 
        hostToggle.interactable = true;
        guestToggle.interactable = true;
        passwordInputField.interactable = true;
        applyButton.interactable = true;
    }

    public void OnClickHostToggle()
    {
        passwordMaching.SetActive(true);
    }
}
