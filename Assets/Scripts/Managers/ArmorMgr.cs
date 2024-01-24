using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorMgr : MonoBehaviour
{
    public static ArmorMgr inst;
    private void Awake()
    {
        inst = this;
    }


    public List<Armor> allArmorPieces;
    public List<Armor> equipedArmor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Armor piece in equipedArmor)
        {
            if (piece.broken)
                equipedArmor.Remove(piece);
        }
    }

    public void equipArmor(Armor armorPiece)
    {
        // later can add checking for duplicate weapons and such already in inventory
        equipedArmor.Add(armorPiece);
    }
}
