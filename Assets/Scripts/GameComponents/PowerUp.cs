using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject powerupObj;
    public Sprite powerupSprite;

    public string powerupName;
    public int powerupLevel;  // the earliest game level it can appear in
    public PowerupTypes powerupType;

    // Start is called before the first frame update
    void Start()
    {
        powerupObj.GetComponent<UnityEngine.UI.Image>().sprite = powerupSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
