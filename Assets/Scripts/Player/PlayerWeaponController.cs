//using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerMovement moveScript;
    //public GameObject playerHolder;
    private Animator daggerAnim;

    public int weaponSelect = 0;

    [Header("Weapon Game Objects")]
    public GameObject daggerObject;
    public GameObject throwingKnifeObject;
    public GameObject bowAndArrowObject;
    //public GameObject throwingSpearObject;
    //public GameObject slingshotObject;
    //public GameObject boomerangObject;

    [Header("Throwing knife rotation corrections")]
    public float knifeXOffset;
    public float knifeYOffset;
    public float knifeZOffset;

    public Transform knifeSpawnPoint;
    public GameObject knifePrefab;
    public float knifeSpeed = 20.0f;
    public float kifeCoolDown = 1.0f;
    public float daggerCoolDown = 0.25f;

    public Transform knifeTransform;

    private bool attackEnabled = true;


    [Header("Weapon animation durations")]
    public float daggerDuration = 0.4f;
    


    // Start is called before the first frame update
    void Start()
    {
        daggerAnim = GameObject.Find("Dagger").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool playerAttack = playerData.isAttacking;
        weaponSelect = playerData.activeWeapon;

        switch(weaponSelect)
        {
            case 1:
                //dagger selected
                if(playerAttack)
                {
                    attackEnabled = false;
                    //Debug.Log("playerAttackTriggered");
                    daggerAnim.SetBool("isIdle", false);  
                    daggerAnim.SetBool("isAttacking", true);  

                    StartCoroutine(attackAnim());
                    StartCoroutine(attackCoolDown(daggerDuration));
                    
                }

                daggerObject.SetActive(true);

                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                break;
            
            case 2:
                //throwing knife
                if(playerAttack && attackEnabled)
                {
                    Quaternion finalRotation = Quaternion.Euler(
                        knifeSpawnPoint.rotation.eulerAngles.x + knifeXOffset,
                        knifeSpawnPoint.rotation.eulerAngles.y + knifeYOffset,
                        knifeSpawnPoint.rotation.eulerAngles.z + knifeZOffset);

                    attackEnabled = false;
                    var knife = Instantiate(knifePrefab, knifeSpawnPoint.position, finalRotation);
                    knife.GetComponent<Rigidbody>().velocity = knifeSpawnPoint.forward * knifeSpeed;
                    StartCoroutine(attackCoolDown(daggerDuration));
                    Destroy(knife, 2);
                }

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

    IEnumerator attackCoolDown(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        attackEnabled = true;
    }

    IEnumerator attackAnim()
    {
        yield return new WaitForSeconds(daggerDuration);

        daggerAnim.SetBool("isIdle", true);  
        daggerAnim.SetBool("isAttacking", false);  
        //gameObject.GetComponent<PlayerMovement>().primaryAttack = false;
        //playerData.isAttacking = false;
    }
}
