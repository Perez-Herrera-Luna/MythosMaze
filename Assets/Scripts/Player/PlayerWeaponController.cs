//using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public PlayerMovement moveScript;
    //public GameObject playerHolder;
    private Animator daggerAnim;

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
        daggerAnim = GameObject.Find("Dagger").GetComponent<Animator>();
        //moveScript = gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        bool playerAttack = gameObject.GetComponent<PlayerMovement>().primaryAttack;

        switch(weaponSelect)
        {
            case 1:
                //dagger selected
                if(playerAttack)
                {
                    //Debug.Log("Player attack with dagger!");
                    daggerAnim.SetBool("isIdle", false);  
                    daggerAnim.SetBool("isAttacking", true);  

                    StartCoroutine(attackAnim());
                }   

                daggerObject.SetActive(true);
                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;
            
            case 2:
                //throwing knife
                throwingKnifeObject.SetActive(true);

                daggerObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;

            case 3:
                //bow and arrow
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

    IEnumerator attackAnim()
    {
        yield return new WaitForSeconds(0.6f);

        daggerAnim.SetBool("isIdle", true);  
        daggerAnim.SetBool("isAttacking", false);  
    }
}
