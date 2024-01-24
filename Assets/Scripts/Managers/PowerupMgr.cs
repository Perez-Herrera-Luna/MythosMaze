using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupMgr : MonoBehaviour
{
    public static PowerupMgr inst;

    public List<PowerUp> allPowerups;
    public List<PowerUp> inventoryPowerups;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void addPowerupToInv(PowerUp curr)
    {
        inventoryPowerups.Add(curr);
    }

    void usePowerUp() { }
}
