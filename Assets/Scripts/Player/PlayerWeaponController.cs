//using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    public PlayerData playerData;
    public BowAndArrowData bowData;
    public PlayerMovement moveScript;
    //public GameObject playerHolder;
    private Animator daggerAnim;

    private Animator bowAnim;
    public int weaponSelect = 0;

    [Header("Weapon Game Objects")]
    public GameObject daggerObject;
    public GameObject throwingKnifeObject;
    public GameObject bowAndArrowObject;
    public GameObject arrowPrefab;
    //public GameObject throwingSpearObject;
    //public GameObject slingshotObject;
    //public GameObject boomerangObject;

    [Header("Throwing knife rotation corrections")]
    public float knifeXOffset;
    public float knifeYOffset;
    public float knifeZOffset;

    public Transform knifeSpawnPoint;
    public Transform arrowFirePoint;

    public GameObject knifePrefab;
    public float knifeSpeed = 20.0f;
    public float kifeCoolDown = 1.0f;
    public float daggerCoolDown = 0.25f;

    private float bowChargeTime = 0f;

    public float chargeEndTime;
    private float chargeStartTime;

    public bool bowCharging = false;

    public Transform knifeTransform;


    private bool attackEnabled = true;


    [Header("Weapon animation durations")]
    public float daggerDuration = 0.4f;
    public float bowDuration = 3.0f;

    //private InputAction attackAction;
    //private InputActionAsset playerControls;


    // Start is called before the first frame update
    void Start()
    {
        daggerAnim = GameObject.Find("Dagger").GetComponent<Animator>();
        bowAnim = GameObject.Find("Bow and Arrow - Animated").GetComponent<Animator>();
    }

    private void Awake()
    {
        //attackAction = playerControls.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        bool playerAttack = playerData.isAttacking;
        weaponSelect = playerData.activeWeapon;
        Debug.Log(weaponSelect);

        switch(weaponSelect)
        {
            case 1:
                //dagger selected
                if(playerData.isMoving)
                {
                    daggerAnim.SetBool("isIdle", false);
                    daggerAnim.SetBool("isWalking", true);
                    //StartCoroutine(walkAnim());
                }
                // else
                // {
                //     daggerAnim.SetBool("isWalking", false);
                // }

                if(!playerAttack && !playerData.isMoving)
                {
                    daggerAnim.SetBool("isWalking", false);  
                    daggerAnim.SetBool("isAttacking", false);  
                    daggerAnim.SetBool("isIdle", true);
                }

                if(playerAttack)
                {
                    attackEnabled = false;
                    //Debug.Log("playerAttackTriggered");
                    daggerAnim.SetBool("isIdle", false);  
                    daggerAnim.SetBool("isWalking", false);
                    daggerAnim.SetBool("isAttacking", true);  

                    StartCoroutine(attackAnim());
                    StartCoroutine(attackCoolDown(daggerDuration));
                    
                }

                daggerObject.SetActive(true);

                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                arrowPrefab.SetActive(false);
                break;
            
            case 2:
                //throwing knife
                if(playerAttack && attackEnabled)
                {
                    throwingKnifeObject.GetComponent<Renderer>().enabled = false;
                    Quaternion finalRotation = Quaternion.Euler(
                        knifeSpawnPoint.rotation.eulerAngles.x + knifeXOffset,
                        knifeSpawnPoint.rotation.eulerAngles.y + knifeYOffset,
                        knifeSpawnPoint.rotation.eulerAngles.z + knifeZOffset);

                    attackEnabled = false;
                    var knife = Instantiate(knifePrefab, knifeSpawnPoint.position, finalRotation);
                    knife.GetComponent<Rigidbody>().velocity = knifeSpawnPoint.forward * knifeSpeed;
                    StartCoroutine(attackCoolDown(2));
                    
                    Destroy(knife, 2);
                }
                else
                {
                    throwingKnifeObject.GetComponent<Renderer>().enabled = true;
                }

                throwingKnifeObject.SetActive(true);
                daggerObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                arrowPrefab.SetActive(false);
                break;

            case 3:
                //bow and arrow
                if(playerData.isMoving)
                {
                    bowAnim.SetBool("isIdle", false);
                    bowAnim.SetBool("isWalking", true);
                    //StartCoroutine(walkAnim());
                }

                /*if(!playerAttack && !playerData.isMoving && !bowCharging)
                {
                    bowAnim.SetBool("isWalking", false);  
                    bowAnim.SetBool("isCharging", false);  
                    bowAnim.SetBool("isIdle", true);
                }

                if(Input.GetKeyDown(KeyCode.Mouse0) && attackEnabled)
                {
                    chargeStartTime = Time.time;
                    attackEnabled = false;
                    bowCharging = true;
                    bowAnim.SetBool("isIdle", false);
                    bowAnim.SetBool("isWalking", false);
                    bowAnim.SetBool("isCharging", true);

                    //StartCoroutine(chargeCounter())
                }

                if(bowCharging && !playerAttack)
                {
                    chargeEndTime = Time.time - chargeStartTime;
                    Debug.Log(chargeEndTime);
                    bowCharging = false;
                    bowAnim.SetBool("isCharging", false);
                    attackEnabled = true;
                }*/

                //detect key down
                if(Input.GetMouseButtonDown(0))
                {
                    bowData.charging = true;
                    bowChargeTime = 0f;

                    bowAnim.SetBool("isCharging", true);
                }
                else if(Input.GetMouseButtonUp(0) && bowData.charging)
                {
                    bowData.charging = false;
                    bowAnim.SetBool("isCharging", false);
                    fireArrow();
                }

                if(bowData.charging)
                {
                    bowChargeTime += Time.deltaTime;
                }

                bowAndArrowObject.SetActive(true);
                arrowPrefab.SetActive(false);
                daggerObject.SetActive(false);
                throwingKnifeObject.SetActive(false);
                break;

            default:
                //empty hand
                daggerObject.SetActive(false);
                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                arrowPrefab.SetActive(false);
                break;

        }

    }

    void fireArrow()
    {
        arrowPrefab.SetActive(true);
        // Instantiate and launch the arrow based on chargeTime
        GameObject arrow = Instantiate(arrowPrefab, arrowFirePoint.position, arrowFirePoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        // Adjust arrow velocity based on chargeTime
        rb.velocity = arrowFirePoint.forward * (bowData.minArrowSpeed + (bowData.maxArrowSpeed - bowData.minArrowSpeed) * (bowChargeTime / bowData.maxChargeTime));

        arrow.transform.forward = Vector3.Slerp(arrow.transform.forward, arrow.GetComponent<Rigidbody>().velocity.normalized, Time.deltaTime);

        Destroy(arrow, 2);
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


    IEnumerator walkAnim()
    {
        yield return new WaitForSeconds(1);

        daggerAnim.SetBool("isIdle", true);  
        daggerAnim.SetBool("isWalking", false);  
    }
}
