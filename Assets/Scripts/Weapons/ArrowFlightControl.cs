using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFlightControl : MonoBehaviour
{
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            // Negate the velocity vector before passing it to Quaternion.LookRotation
            this.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
}
