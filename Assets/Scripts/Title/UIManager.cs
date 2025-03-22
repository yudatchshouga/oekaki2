using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle hostToggle;
    [SerializeField] Toggle guestToggle;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button applyButton;
    [SerializeField] Dropdown resolutionDropdown;

    private Resolution[] resolutions = {
        new Resolution { width = 640, height = 360 },
        new Resolution { width = 854, height = 480 },
        new Resolution { width = 960, height = 540 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    private void Start()
    {
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", 5);
        resolutionDropdown.value = savedIndex;
        SetResolution();
    }

    public void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, false);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    }
}

