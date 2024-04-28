using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public UserInterfaceManager userInterfaceMgr;
    public OptionsMenuController inst;

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

    void Start()
    {
        inst = this;

        // cameraSensitivitySlider = GameObject.Find("CameraSensitivitySlider").GetComponent<UnityEngine.UI.Slider>();
        // masterVolumeSlider = GameObject.Find("MasterVolumeSlider").GetComponent<UnityEngine.UI.Slider>();
        // playerFOVSlider = GameObject.Find("PlayerFOVSlider").GetComponent<UnityEngine.UI.Slider>();
        // cameraTiltToggle = GameObject.Find("CameraTiltToggle").GetComponent<UnityEngine.UI.Toggle>();
        // cameraInvertToggle = GameObject.Find("CameraInvertToggle");
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
}
