using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class OptionsMenuController : MonoBehaviour
{
    public static OptionsMenuController inst;
    public UserInterfaceManager userInterfaceMgr;

    public PlayerCamera playerCamera;
    public PlayerMovement playerMovement;
    public AudioMixer audioMixer;

    // Options
    public bool cameraTilt = true;
    public bool cameraInvert = false;
    public float cameraSensitivity = 0.1f;
    public float masterVolume = 1f;
    public float playerFOV = 85f;

    public UnityEngine.UI.Slider cameraSensitivitySlider;
    public UnityEngine.UI.Slider masterVolumeSlider;
    public UnityEngine.UI.Slider playerFOVSlider;
    public UnityEngine.UI.Toggle cameraTiltToggle;
    public UnityEngine.UI.Toggle cameraInvertToggle;
    public UnityEngine.UI.Toggle fullscreenToggle;

    public TMP_Text cameraSensitivityText;
    public TMP_Text masterVolumeText;
    public TMP_Text playerFOVText;

    void Start()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(this);
        }

        playerCamera = GameObject.Find("Player Camera").GetComponent<PlayerCamera>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        cameraSensitivitySlider = GameObject.Find("CameraSensitivitySlider").GetComponent<UnityEngine.UI.Slider>();
        masterVolumeSlider = GameObject.Find("VolumeSlider").GetComponent<UnityEngine.UI.Slider>();
        playerFOVSlider = GameObject.Find("PlayerFOVSlider").GetComponent<UnityEngine.UI.Slider>();
        cameraTiltToggle = GameObject.Find("CameraTiltToggle").GetComponent<UnityEngine.UI.Toggle>();
        cameraInvertToggle = GameObject.Find("CameraInvertToggle").GetComponent<UnityEngine.UI.Toggle>();
        fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<UnityEngine.UI.Toggle>();

        cameraSensitivityText = GameObject.Find("SensitivityText").GetComponent<TMP_Text>();
        masterVolumeText = GameObject.Find("VolumeText").GetComponent<TMP_Text>();
        playerFOVText = GameObject.Find("FOVText").GetComponent<TMP_Text>();
        cameraSensitivityText.text = null;
        masterVolumeText.text = null;
        playerFOVText.text = null;

        UpdateOptions();
    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void BackButton()
    {
        if(GameManager.inst.isGamePaused)
        {
            userInterfaceMgr.OptionsMenuBackPaused();
        }
        else
        {
            userInterfaceMgr.MainMenu();
        }
    }

    public void ResetBackToDefault()
    {
        cameraSensitivity = 0.1f;
        masterVolume = 1f;
        playerFOV = 85f;
        cameraTilt = true;
        cameraInvert = false;

        UpdateOptions();

        AudioManager.inst.PlayMenuInteraction();
    }

    public void UpdateOptions()
    {
        cameraSensitivitySlider.value = cameraSensitivity;
        masterVolumeSlider.value = masterVolume;
        playerFOVSlider.value = playerFOV;
        cameraTiltToggle.isOn = cameraTilt;
        cameraInvertToggle.isOn = cameraInvert;

        cameraSensitivityText.text = cameraSensitivity.ToString();
        masterVolumeText.text = masterVolume.ToString();
        playerFOVText.text = playerFOV.ToString();
    }

    public void SetSensitive(float value)
    {
        // cameraSensitivity = value;
        playerCamera.sensitivityMultiplier = value;

        cameraSensitivityText.text = value.ToString("0.00");
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);

        // Map slider value from -80 to 0 to 0 to 1
        float transformedValue = (value + 80) / 80;

        masterVolumeText.text = transformedValue.ToString("0.00");
    }

    public void SetFOV(float value)
    {
        playerCamera.SetDefaultFOV(value);

        playerFOVText.text = value.ToString("0");
    }

    public void SetTilt(bool value)
    {
        // cameraTilt = value;
        playerMovement.cameraTilt = value;
    }

    public void SetInvert(bool value)
    {
        // cameraInvert = value;
        playerCamera.invertedCamera = value;
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
