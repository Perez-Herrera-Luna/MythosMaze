using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMgr : MonoBehaviour
{

    public static WeaponMgr inst;

    public List<Weapon> allWeapons;
    public List<Weapon> currWeapons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Weapon w in currWeapons)
        {
            if (w.broken)
                currWeapons.Remove(w);
        }
    }

    public void addWeaponToInv(Weapon newWeapon)
    {
        // later can add checking for duplicate weapons and such already in inventory
        currWeapons.Add(newWeapon);
    }
}
