using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMgr : MonoBehaviour
{
    public static InventoryMgr inst;

    private WeaponMgr weaponManager;
    private ArmorMgr armorManager;
    private PowerupMgr powerupManager;

    // Start is called before the first frame update
    void Start()
    {
        weaponManager = WeaponMgr.inst;
        armorManager = ArmorMgr.inst;
        powerupManager = PowerupMgr.inst;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
