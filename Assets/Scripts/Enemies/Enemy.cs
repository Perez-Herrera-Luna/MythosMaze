using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data Files")]
    public EnemyData enemy;
    public WeaponData weapon;
    public PlayerData playerData;

    [Header("Transforms")]
    public Transform player;
    public Transform orientation;

    [Header("Ground Detection")]
    public float enemyHeight; // enemy's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    [Header("Player Detection")]
    public GameObject playerObject;
    public LayerMask whatIsPlayer; //Layermask for player detection

    [Header("Enemy Physics")]
    Rigidbody rb;

    [Header("Attacking")]
    public bool isAttacking;

    //wandering
    private Vector3 moveDirection;
    private Vector3 walkPoint; //point on the ground to walk to
    private bool pointChosen;   //true if a new walkpoint is set

    //player attacking
    public bool invulnerable = false;

    //animation
    private Animator skeletonAnim;

    private string animType;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //disable gravity if flight enemy
        if(enemy.canFly)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        enemy.health = enemy.maxHealth;
        enemy.hasAttacked = false;
        enemy.attackDamage = enemy.maxAttackDamage;

        //animation setup
        skeletonAnim = GameObject.Find("Skeleton_Enemy").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //check enemy health
        checkHealth();
        animate(animType);

        //raycasting for ground detection
        isGrounded = Physics.Raycast(transform.position, Vector3.down, enemyHeight * 0.5f + 0.2f, whatIsGround);

        //apply drag
        if(isGrounded)
        {
            rb.drag = enemy.groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        
        //check if player is within sight/attack range

        float distance = Vector3.Distance (transform.position, player.position);

        if(distance < enemy.sightRange && distance > enemy.attackRange)
        {
            enemy.playerInSight = true;
            enemy.playerAttackable = false;
        }
        if(distance < enemy.sightRange && distance < enemy.attackRange)
        {
            enemy.playerAttackable = true;
            enemy.playerInSight = true;
        }
        if(distance > enemy.sightRange)
        {
            enemy.playerInSight = false;
            enemy.playerAttackable = false;
        }

        if(!enemy.playerInSight && !enemy.playerAttackable)
        {
            animType = "wander";
            wandering();
        }
        if(enemy.playerInSight && !enemy.playerAttackable)
        {
            animType = "chase";
            chasing();
        }
        if(enemy.playerAttackable)
        {
            
            attacking();
        }
        
        isAttacking = enemy.hasAttacked;
        if(isAttacking)
        {
            animType = "attack";
        }

        if(enemy.health <= 0)
        {
            animType = "dead";
        }
    }

    

    private void wandering()
    {
        
        //determine what point to walk to
        if(!pointChosen)
        {
            searchForPoint();
        }
        else
        {
            moveEnemy(walkPoint, 0);
        }

        //check if enemy reached desired point
        Vector3 distanceToPoint = transform.position - walkPoint;
        if(distanceToPoint.magnitude < 1f)
        {
            pointChosen = false; //search for a new point now
        }
    }

    private void searchForPoint()
    {
        //create a random point within wandering range
        float randomZ = Random.Range(-enemy.roamRange, enemy.roamRange);
        float randomX = Random.Range(-enemy.roamRange, enemy.roamRange);
        float randomY = Random.Range(enemy.flyingHeightMax-1, enemy.flyingHeightMax);

        walkPoint = new Vector3();

        switch(enemy.name)
        {
            case "Winged_Melee":
                walkPoint.x = transform.position.x + randomX;
                walkPoint.y = transform.position.y + randomY;
                walkPoint.z = transform.position.z + randomZ;
                pointChosen = true;
                break;

            case "Winged_Ranged":
                walkPoint.x = transform.position.x + randomX;
                walkPoint.y = transform.position.y + randomY;
                walkPoint.z = transform.position.z + randomZ;
                pointChosen = true;
                break;

            case "Sniper_Ranged":
                walkPoint.x = transform.position.x + randomX;
                walkPoint.y = transform.position.y;
                walkPoint.z = transform.position.z + randomZ;
                break;

            default:
                walkPoint.x = transform.position.x + randomX;
                walkPoint.y = transform.position.y;
                walkPoint.z = transform.position.z + randomZ;
                break;
        }

        //check if the point chosen is valid
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && (enemy.name != "Winged_Melee" || enemy.name != "Winged_Ranged"))
        {
            pointChosen = true;
        }
    }

    private void chasing()
    {
        Vector3 target;
        switch(enemy.name)
        {
            case "Winged_Melee":

                if(Mathf.Abs(transform.position.x - player.position.x) > 0.5 && Mathf.Abs(transform.position.z - player.position.z) > 0.5)
                {
                    target.x = player.position.x;
                    target.z = player.position.z;
                    target.y = transform.position.y;
                }
                else
                {
                    target.x = player.position.x;
                    target.z = player.position.z;
                    target.y = player.position.y;
                }

                if(target.y >= enemy.flyingHeightMax)
                {
                    target.y = enemy.flyingHeightMax - (float)0.5;
                }
                break;

            case "Winged_Ranged":
                target.x = player.position.x;
                target.z = player.position.z;
                target.y = transform.position.y;
                
                if(target.y >= enemy.flyingHeightMax)
                {
                    target.y = enemy.flyingHeightMax - (float)0.5;
                }
                break;

            case "Sniper_Ranged":
                target.x = (transform.position.x - player.position.x) * 2;
                target.z = (transform.position.z - player.position.z) * 2;
                target.y = player.position.y;
                break;

            default:
                target = player.position;
                break;
        }
        
        moveEnemy(target, 1);
    }

    private void attacking()
    {
        if(!enemy.hasAttacked)
        {
            Debug.Log("Enemy Attacking!");
            enemy.hasAttacked = true;
            Debug.Log("setting hasAttacked to true");
            StartCoroutine(coolDownTimer(enemy.attackSpeed));
        }
        else
        {
            Vector3 backOff = player.position;
            if(enemy.name == "Winged_Melee")
            {
                //attack code
                backOff.y += 5;
                moveEnemy(backOff, 1);
            }

            if(enemy.name == "Skeleton")
            {
                moveEnemy(transform.position, 2); //stop enemy movement
                transform.LookAt(player); // have enemy face the player
                //transform.rotation = Quaternion.identity;
            }
        }
         
    }

    private void moveEnemy(Vector3 target, int mode)
    {
        float moveSpeed;
        if(mode == 0)
        {
            moveSpeed = enemy.wanderSpeed;
        }
        else if(mode == 1)
        {  
            moveSpeed = enemy.chaseSpeed;
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

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemy.turnSpeed * Time.deltaTime);

            //move towards the target
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

        }
        else if (!isGrounded)
        {
             //rotate to look at the target
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), enemy.turnSpeed * Time.deltaTime);
            
            //move towards the target
            transform.position += transform.forward * Time.deltaTime * moveSpeed * enemy.airMultiplier;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("playerWeapon"))
        {
            Debug.Log("player attacking: " + playerData.isAttacking);
            //Debug.Log("Enemy hit!");
            //StartCoroutine(OnHit(4));
            //player_script = other.GetComponent<Weapon>();
            if(playerData.isAttacking)
            {
                Debug.Log("Enemy hit!");

                switch(playerData.activeWeapon)
                {
                    case 1:
                        //dagger
                        StartCoroutine(OnHit(5));
                        Object.Destroy(other);
                        break;
                        
                    case 2:
                        //throwing knife
                        StartCoroutine(OnHit(2));
                        break;

                    case 3:
                        //bow and arrow
                        StartCoroutine(OnHit(5));
                        Object.Destroy(other);
                        break;

                    default:
                        //no weapon
                        break;
                }
            }
            else
            {
                if(playerData.activeWeapon == 3)
                {
                    StartCoroutine(OnHit(5));
                    Object.Destroy(other.gameObject);
                }
            }
            
        }
    }

    IEnumerator coolDownTimer(float cd)
    {
        yield return new WaitForSeconds(cd);
        enemy.hasAttacked = false;
        Debug.Log("cooldown finished");
        Debug.Log("setting hasAttacked to false");
        //animType = "chase";
    }

    IEnumerator OnHit(int damage)
    {
        if(!invulnerable)
        {
            invulnerable = true;
            enemy.health -= damage;
            //Debug.Log("Enemy Health: " + enemy.health);
        }
        else
        {
            StartCoroutine(hitDelay());
        }
      
        //gameMgr.DisplayDamage();

        yield return new WaitForSeconds(2);

        //gameMgr.HideDamage();
    }

    IEnumerator hitDelay()
    {
        yield return new WaitForSeconds(0.25f);
        invulnerable = false;
    }

    private void checkHealth()
    {
        if(enemy.health <= 0)
        {
            animType = "dead";
            enemy.health = 0;
            enemy.attackDamage = 0;
            StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        yield return new WaitForSeconds(1f);
        Object.Destroy(this.gameObject);
    }

    private void animate(string animType)
    {
        
        switch(enemy.name)
        {
            case "Skeleton":
                
                if(animType == "wander")
                {
                    skeletonAnim.SetBool("isWandering", true);
                    skeletonAnim.SetBool("isChasing", false);
                    skeletonAnim.SetBool("isAttacking", false);
                }
                else if(animType == "chase")
                {
                    skeletonAnim.SetBool("isWandering", false);
                    skeletonAnim.SetBool("isChasing", true);
                    skeletonAnim.SetBool("isAttacking", false);
                }
                else if(animType == "attack")
                {
                    skeletonAnim.SetBool("isWandering", false);
                    skeletonAnim.SetBool("isChasing", false);
                    skeletonAnim.SetBool("isAttacking", true);
                }
                else if(animType == "dead")
                {
                    skeletonAnim.SetBool("isWandering", false);
                    skeletonAnim.SetBool("isChasing", false);
                    skeletonAnim.SetBool("isAttacking", false);
                    skeletonAnim.SetBool("isDead", true);
                }
                break;

        }
    }
}