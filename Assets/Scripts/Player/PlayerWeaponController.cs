//using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    public Camera fpsCam;
    public BowAndArrowData bowData;
    public PlayerMovement moveScript;
    //public GameObject playerHolder;
    private Animator daggerAnim;

    private Animator bowAnim;
    public int weaponSelect = 0;

    // for now just manually set
    public float[] weaponDamage = new float[] { 5.0f, 2.0f, 5.0f };

    [Header("Weapon Game Objects")]
    public GameObject daggerObject;
    public GameObject throwingKnifeObject;
    public GameObject throwingKnifePlaceHolder;
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
    public float daggerCoolDown = 0.25f;
    private float bowChargeTime = 0f;

    public float weaponCooldown = 2.0f;
    public float weaponSpeed = 2.0f;

    public float chargeEndTime;
    private float chargeStartTime;

    public bool bowCharging = false;

    public Transform knifeTransform;

    private bool attackEnabled = true;
    private bool bowEnabled = true;
    private bool playerAttacking = false;


    [Header("Weapon animation durations")]
    public float daggerDuration = 1.0f;
    public float bowDuration = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        daggerAnim = GameObject.Find("Dagger").GetComponent<Animator>();
        bowAnim = GameObject.Find("Bow and Arrow - Animated").GetComponent<Animator>();
        moveScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //attacking
        //if(attackAction.triggered)
        if(InputManager.instance.AttackInput && attackEnabled)
        {    
            //primaryAttack = true; 
            playerAttacking = true;
            //StartCoroutine(attackDelay());
        }

        //bool playerAttack = playerData.isAttacking;
        weaponSelect = moveScript.weaponSelected;
        //Debug.Log(weaponSelect);

        switch(weaponSelect)
        {
            case 1:
                //Debug.Log("weapon 1 selected");
                if(!playerAttacking && !moveScript.isMoving)
                {
                    daggerAnim.SetBool("isWalking", false);   
                    daggerAnim.SetBool("isIdle", true);
                    daggerAnim.SetBool("isAttacking", false);  
                }

                if(!playerAttacking && moveScript.isMoving)
                {
                    daggerAnim.SetBool("isWalking", true);   
                    daggerAnim.SetBool("isIdle", false);
                    daggerAnim.SetBool("isAttacking", false);  
                }

                if(playerAttacking && attackEnabled)
                {
                    attackEnabled = false;
                    daggerAnim.SetBool("isIdle", false);  
                    daggerAnim.SetBool("isWalking", false);
                    daggerAnim.SetBool("isAttacking", true);  

                    //StartCoroutine(attackAnim(daggerDuration));
                    //StartCoroutine(attackCoolDown(daggerDuration));
                    StartCoroutine(daggerAnimDuration(0.5f));
                    StartCoroutine(attackCoolDown(0.5f));
                }
                daggerObject.SetActive(true);

                throwingKnifePlaceHolder.SetActive(false);  
                throwingKnifeObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                arrowPrefab.SetActive(false);
     
                break;
            
            case 2:
                //throwing knife
                if(playerAttacking && attackEnabled)
                {
                    throwingKnifePlaceHolder.SetActive(false);    
                    Quaternion finalRotation = Quaternion.Euler(
                        knifeSpawnPoint.rotation.eulerAngles.x + knifeXOffset,
                        knifeSpawnPoint.rotation.eulerAngles.y + knifeYOffset,
                        knifeSpawnPoint.rotation.eulerAngles.z + knifeZOffset);

                    attackEnabled = false;
                    var knife = Instantiate(knifePrefab, knifeSpawnPoint.position, finalRotation);
                    knife.GetComponent<Rigidbody>().velocity = knifeSpawnPoint.forward * knifeSpeed;
                    StartCoroutine(attackCoolDown(weaponCooldown));
                    
                    Destroy(knife, 2);
                }
                else
                {
                    throwingKnifePlaceHolder.SetActive(true);    
                }

                //throwingKnifePlaceHolder.SetActive(true);    
                throwingKnifeObject.SetActive(true);
                daggerObject.SetActive(false);
                bowAndArrowObject.SetActive(false);
                arrowPrefab.SetActive(false);
                break;

            case 3:
                //bow and arrow
                if(moveScript.isMoving && bowEnabled)
                {
                    bowAnim.SetBool("isIdle", false);
                    bowAnim.SetBool("isWalking", true);
                }
                
                if(!moveScript.isMoving && bowEnabled)
                {
                    bowAnim.SetBool("isIdle", true);
                    bowAnim.SetBool("isWalking", false);
                }

                if(!bowEnabled)
                {
                    bowAnim.SetBool("isIdle", false);
                    bowAnim.SetBool("isWalking", false);
                }
                
                //detect key down
                if(Input.GetMouseButtonDown(0) && bowEnabled)
                {
                    bowData.charging = true;
                    bowChargeTime = 0f;

                    bowAnim.SetBool("isCharging", true);
                }
                else if(Input.GetMouseButtonUp(0) && bowData.charging && bowEnabled) //arrow launched
                { 
                    bowData.charging = false;
                    bowAnim.SetBool("isCharging", false);

                    //arrowPrefab.SetActive(false);
                    fireArrow();

                    bowEnabled = false;
                    StartCoroutine(bowCoolDown());
                }

                if(bowData.charging)
                {
                    bowChargeTime += Time.deltaTime;
                }

                bowAndArrowObject.SetActive(true);
                arrowPrefab.SetActive(true);
                throwingKnifePlaceHolder.SetActive(false);  

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

    public void throwKnife()
    {

    }

    // Weapon Powerup Function
    public void buffWeapons(float amount)
    {
        weaponCooldown = weaponCooldown - amount;
        daggerAnim.speed = daggerAnim.speed + amount;
        Debug.Log("buff weapons: " + amount);
    }

/*
    IEnumerator attackDelay()
    {
        //Debug.Log("Delay start");
        yield return new WaitForSeconds(2.0f);
        //Debug.Log("Delay end");
        moveScript.isAttacking = false;
        playerAttacking = false;
    }
    */

    IEnumerator daggerAnimDuration(float dur)
    {
        yield return new WaitForSeconds(dur);
        daggerAnim.SetBool("isAttacking", false);  
    }

    void fireArrow()
    {
        float arrowMultiplier;
        // Adjust arrow velocity based on chargeTime

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            arrowMultiplier = 2.0f;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
            arrowMultiplier = 1.0f;
        }

        Vector3 direction = (targetPoint - arrowFirePoint.position)/20;

        // Instantiate and launch the arrow based on chargeTime
        GameObject arrow = Instantiate(arrowPrefab, arrowFirePoint.position, arrowFirePoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        Vector3 arrowVelocity = direction * (bowData.minArrowSpeed + (bowData.maxArrowSpeed - bowData.minArrowSpeed) * (bowChargeTime / bowData.maxChargeTime));

        // Adjust arrow rotation to align with game world's forward direction
        arrow.transform.forward = direction;

        // Apply velocity to the arrow
        rb.velocity = arrowVelocity * arrowMultiplier;

        // Rotate the arrow to align with its velocity
        if (rb.velocity != Vector3.zero)
        {
            arrow.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        Destroy(arrow, bowData.arrowDuration);
    }

    IEnumerator bowCoolDown()
    {
        yield return new WaitForSeconds(bowData.coolDown);
        bowEnabled = true;
    }

    IEnumerator attackCoolDown(float delayTime)
    {
        UserInterfaceManager.inst.DisplayAttackIndicator(delayTime);
        yield return new WaitForSeconds(delayTime);
        attackEnabled = true;
        playerAttacking = false;
        //playerData.isAttacking = false;
    }

    /*
    IEnumerator attackAnim(float dur)
    {
        yield return new WaitForSeconds(dur);

        daggerAnim.SetBool("isIdle", true);  
        daggerAnim.SetBool("isAttacking", false);  
        //gameObject.GetComponent<PlayerMovement>().primaryAttack = false;
        //playerData.isAttacking = false;
    }
    */
}
