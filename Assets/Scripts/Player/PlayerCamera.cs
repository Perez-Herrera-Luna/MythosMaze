using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerCamera : MonoBehaviour
{
    public float sensitivityMultiplier = 0.1f; // Sensitivity multiplier

    public Transform orientation;

    float xRotation;
    float yRotation;
    float zRotation;
    float defaultZ_Rotation = 0;
    float tiltAngle = 2f;
    float rotationDuration = 0.30f;
    string previousTiltDirection;
    public bool invertedCamera = false;

    private IEnumerator FOV_Change_Coroutine;
    private IEnumerator Tilt_Coroutine;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        zRotation = defaultZ_Rotation;
    }

    void Update()
    {
        float mouseX = InputManager.instance.CameraInput.x * sensitivityMultiplier;
        float mouseY = InputManager.instance.CameraInput.y * sensitivityMultiplier;

        if (invertedCamera)
        {
            // mouseX = -mouseX;
            mouseY = -mouseY;
        }

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents the player from looking too far up or down

        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void SmoothFovChange(float targetFov, float duration)
    {
        float targetHorizontalFov = Camera.HorizontalToVerticalFieldOfView(targetFov, Camera.main.aspect);

        if (FOV_Change_Coroutine != null)
        {
            StopCoroutine(FOV_Change_Coroutine);
        }

        FOV_Change_Coroutine = SmoothFovChangeCoroutine(targetHorizontalFov, duration);
        StartCoroutine(FOV_Change_Coroutine);
    }

    IEnumerator SmoothFovChangeCoroutine(float targetFov, float duration)
    {
        if (duration <= 0)
        {
            Camera.main.fieldOfView = targetFov;
            yield break;
        }

        float startFov = Camera.main.fieldOfView;
        float time = 0;

        while (time < duration)
        {
            Camera.main.fieldOfView = Mathf.Lerp(startFov, targetFov, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        Camera.main.fieldOfView = targetFov;
    }

    public void applyTilt(string direction)
    {
        if(direction == previousTiltDirection)
        {
            return;
        }

        float targetZRotation;
        if(direction == "left")
        {
            targetZRotation = defaultZ_Rotation + tiltAngle;
        }
        else if(direction == "right")
        {
            targetZRotation = defaultZ_Rotation - tiltAngle;
        }
        else if(direction == "reset")
        {
            targetZRotation = defaultZ_Rotation;
        }
        else
        {
            targetZRotation = defaultZ_Rotation;
        }

        if (Tilt_Coroutine != null)
        {
            StopCoroutine(Tilt_Coroutine);
        }

        Tilt_Coroutine = SmoothCameraTilt(targetZRotation);
        StartCoroutine(Tilt_Coroutine);
        previousTiltDirection = direction;
    }

    IEnumerator SmoothCameraTilt(float targetZRotation)
    {
        
        float startZRotation = zRotation;
        float time = 0;
        
        while (targetZRotation != zRotation)
        {
            zRotation = Mathf.Lerp(startZRotation, targetZRotation, time / rotationDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void InvertCamera(bool invert)
    {
        invertedCamera = invert;
    }
}
