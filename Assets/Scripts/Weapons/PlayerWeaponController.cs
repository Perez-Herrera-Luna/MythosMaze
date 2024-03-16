using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public int weaponSelect = 0;

    public GameObject daggerObject;
    public GameObject throwingKnifeObject;
    public GameObject bowAndArrowObject;
    //public GameObject throwingSpearObject;
    //public GameObject slingshotObject;
    //public GameObject boomerangObject;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(weaponSelect)
        {
            case 1:
                //dagger
                daggerObject.SetActive(true);

                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;
            
            case 2:
                //dagger
                throwingKnifeObject.SetActive(true);

                daggerObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;

            case 3:
                //dagger
                bowAndArrowObject.SetActive(true);

                daggerObject.SetActive(false);
                throwingKnifeObject.SetActive(false);
                break;

            default:
                //empty hand
                daggerObject.SetActive(false);
                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;

        }
    }
}
