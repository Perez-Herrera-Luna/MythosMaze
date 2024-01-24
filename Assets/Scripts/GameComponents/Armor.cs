using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    public GameObject armorObj;
    public Sprite armorSprite;

    public string armorName;
    public int armorLevel;  // the earliest game level it can appear in
    public ArmorTypes armorType;

    public float defenseDamage;
    public float armorIntegrity;

    public bool broken;

    // Start is called before the first frame update
    void Start()
    {
        armorObj.GetComponent<UnityEngine.UI.Image>().sprite = armorSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            if (armorIntegrity <= 0)
                broken = true;
        }
    }
}
