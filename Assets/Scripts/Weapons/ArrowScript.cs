using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.rotation = transform.direction;
    }
}
