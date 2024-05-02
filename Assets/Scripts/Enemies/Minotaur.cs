using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MonoBehaviour
{
    public EnemyData enemy;
    
    [Header("Transforms")]
    public Transform player;
    public Transform orientation;

    [Header("Ground Detection")]
    public float enemyHeight; // enemy's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    [Header("Player Detection")]
    public GameObject playerObject;
    private PlayerManager playerMgr;
    public LayerMask whatIsPlayer; //Layermask for player detection

    [Header("Enemy Physics")]
    Rigidbody rb;

    [Header("Attacking")]
    public bool isAttacking = false;
    public bool isCharging = false;
    public bool executedAttack;
    private bool attackLock = false;
    public float attackRange = 1.0f;
    private bool playerAttackable = false;
    private bool invulnerable = false;

    [Header("Death")]
    public bool enemyDead = false;
    private bool triggerDeath = false;

    //player attacks
    public int playerAttackCounter = 1;
    public int pillarDestroyedCount = 0;

    //animation
    private Animator Anim;
    private string animType;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isAttacking = false;
        executedAttack = false;
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        playerMgr = player.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        checkPillarCount();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, enemyHeight * 0.5f + 0.2f, whatIsGround);
        rb.drag = 1;

        //minotaur states

        float distance = Vector3.Distance (transform.position, player.position);
        if(!enemyDead)
        {
            animate(animType);

            if(distance < attackRange)
            {
                playerAttackable = true;
            }
            else
            {
                playerAttackable = false;
            }

            if (!playerAttackable)
            {
                animType = "chase";
                chasing();
            }
            else
            {
                if(playerAttackCounter % 5 == 0)
                {
                    charging();
                }
                else
                {
                    attacking();
                }
                
                if (!executedAttack && !attackLock)
                {
                    attackLock = true;
                    StartCoroutine(executingAttack(0.7f));
                    StartCoroutine(stoppingAttack(0.8f));
                }
            }
        }
        else
        {
            moveEnemy(transform.position, 2);
        }
    }

    private void moveEnemy(Vector3 target, int mode)
    {
        float moveSpeed;
        if(mode == 0)
        {
            moveSpeed = 2;
        }
        else if(mode == 1)
        {  
            moveSpeed = 3;
        }
        else
        {
            moveSpeed = 0;
        }

        // Apply force to the enemy. Variable airMultiplier is used to reduce the enemy's speed in the air
        if (isGrounded && target != transform.position)
        {
            //rotate to look at the target
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), enemy.turnSpeed * Time.deltaTime);
            Vector3 direction = (target - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            if(mode == 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            }

            //move towards the target
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

        }
    }

    void charging()
    {
        animType = "charge";
        isCharging = true;
        StartCoroutine(chargeTimer());
    }

    IEnumerator chargeTimer()
    {
        yield return new WaitForSeconds(5);
        isCharging = false;
        animType = "chase";
    }

    IEnumerator executingAttack(float dur)
    {
        yield return new WaitForSeconds(dur);
        executedAttack = true;
        // Debug.Log("Attack Executed");
    }

    IEnumerator stoppingAttack(float dur)
    {
        yield return new WaitForSeconds(dur);
        executedAttack = false;
        // Debug.Log("Attack finished");
        attackLock = false;
    }

    private void chasing()
    {
        executedAttack = false;
        Vector3 target = player.position;     
        moveEnemy(target, 1);
    }

    private void attacking()
    {
        if(!isAttacking)
        {
            Debug.Log("Enemy Attacking!");
            isAttacking = true;
            //Debug.Log("setting hasAttacked to true");
            
        }
        else
        {
            Vector3 backOff = player.position;
            
            moveEnemy(transform.position, 2); //stop enemy movement
            transform.LookAt(player); // have enemy face the player
            animate("attack");
            //transform.rotation = Quaternion.identity;
        }  
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("playerWeapon"))
        {
            // Debug.Log("player attacking: " + playerData.isAttacking);
            //Debug.Log("Enemy hit!");
            //StartCoroutine(OnHit(4));
            //player_script = other.GetComponent<Weapon>();
            if(playerMgr.IsAttacking)
            {
                Debug.Log("Player attacking!");
                playerAttackCounter += 1;
            }
            else
            {
                if(playerMgr.ActiveWeapon == 3)
                {
                    playerAttackCounter += 1;
                }
            }
            StartCoroutine(OnHit());
        }
    }

    IEnumerator OnHit()
    {
        Debug.Log("enemy hit");
        if(!invulnerable)
        {
            StartCoroutine(hitDelay());
        }
        yield return new WaitForSeconds(2);
    }
    IEnumerator hitDelay()
    {
        yield return new WaitForSeconds(1f);
        invulnerable = false;
    }

    void checkPillarCount()
    {
        if(pillarDestroyedCount >= 4)
        {
            //win game
        }
    }

    private void animate(string animType)
    {
        if(animType == "chase")
        {
            Anim.SetBool("isWalking", true);
            Anim.SetBool("isAttacking", false);
            Anim.SetBool("isCharging", false);
        }

        if(animType == "attack")
        {
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isCharging", false);
        }

        if(animType == "charge")
        {
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
            Anim.SetBool("isCharging", true);
        }

        if(animType == "dead")
        {
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
            Anim.SetBool("isCharging", false);
            Anim.SetBool("isDead", true);
        }
    }
}
