using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponObj;
    public Sprite weaponSprite;

    public string weaponName;
    public int weaponLevel;
    public WeaponTypes weaponType;

    public float attackDamage;
    public float defenseDamage;
    public float weaponIntegrity;

    public bool broken;

    // Start is called before the first frame update
    void Start()
    {
        weaponObj.GetComponent<UnityEngine.UI.Image>().sprite = weaponSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            if (weaponIntegrity <= 0)
                broken = true;
        }
    }
}
