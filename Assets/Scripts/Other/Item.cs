using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.CompareTag("player"))
        {
            Debug.Log("player picked up quest item");
            QuestManager.inst.ItemPickedUp(itemName);

            Destroy(gameObject);
        }
    }
}
