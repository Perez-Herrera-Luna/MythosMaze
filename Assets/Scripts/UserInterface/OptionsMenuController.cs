using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public static OptionsMenuController inst;
    public UserInterfaceManager userInterfaceMgr;

    public PlayerCamera playerCamera;
    public PlayerMovement playerMovement;

    // Options
    public bool cameraTilt = true;
    public bool cameraInvert = false;
    public float cameraSensitivity = 0.1f;
    public float masterVolume = 1f; // TODO: Hook this up to the volume when the audio manager is implemented. Make sure to change range on the slider and the default value.
    public float playerFOV = 85f;

    public UnityEngine.UI.Slider cameraSensitivitySlider;
    public UnityEngine.UI.Slider masterVolumeSlider;
    public UnityEngine.UI.Slider playerFOVSlider;
    public UnityEngine.UI.Toggle cameraTiltToggle;
    public UnityEngine.UI.Toggle cameraInvertToggle;
    public UnityEngine.UI.Toggle fullscreenToggle;

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
    }

    public void UpdateOptions()
    {
        cameraSensitivitySlider.value = cameraSensitivity;
        masterVolumeSlider.value = masterVolume;
        playerFOVSlider.value = playerFOV;
        cameraTiltToggle.isOn = cameraTilt;
        cameraInvertToggle.isOn = cameraInvert;
    }

    public void SetSensitive(float value)
    {
        // cameraSensitivity = value;
        playerCamera.sensitivityMultiplier = value;
    }

    public void SetVolume(float value)
    {
        masterVolume = value;
    }

    public void SetFOV(float value)
    {
        // playerFOV = value;
        playerCamera.SmoothFovChange(value, 0f);
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
