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
            QuestManager.inst.ItemPickedUp(itemName);

            Destroy(gameObject);
        }
    }
}
