using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerCamera : MonoBehaviour
{
    public float sensitivityX = 3f; // X-axis sensitivity
    public float sensitivityY = 3f; // Y-axis sensitivity

    public Transform orientation;

    float xRotation;
    float yRotation;

    void Start()
    {
        // Locks the cursor to the center of the screen and makes it invisible
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents the player from looking too far up or down

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void SmoothFovChange(float targetFov, float duration)
    {
        float targetHorizontalFov = Camera.HorizontalToVerticalFieldOfView(targetFov, Camera.main.aspect);

        StopAllCoroutines();
        StartCoroutine(SmoothFovChangeCoroutine(targetHorizontalFov, duration));
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
}
