using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerCamera : MonoBehaviour
{
    public float sensitvityX; // X-axis sensitivity
    public float sensitvityY; // Y-axis sensitivity

    public Transform orientation;

    float xRotation;
    float yRotation;

    void Start()
    {
        // Locks the cursor to the center of the screen and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitvityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitvityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents the player from looking too far up or down

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
