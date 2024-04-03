using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowAndArrowData", menuName = "Bow Weapon Data")]
public class BowAndArrowData : ScriptableObject
{
    public Transform bowPivot; // The pivot point of the bow

    public bool charging = false;
    public float maxChargeTime = 3f; 
    public float minArrowSpeed = 1f; 
    public float maxArrowSpeed = 60f; 
}
