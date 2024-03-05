using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public EnemyData monster;
    public bool hasWeapon;
    public WeaponData weapon;

    [Header("Enemy Stats")]
    //enemy type
    public string enemyType = "Basic_Melee"; //Enemy type: Basic_Melee, Basic_Ranged, Sniper_Ranged, Shotgun_Ranged, 
                             //Predictive_Ranged, Winged_Melee, Winged_Ranged, Shielded_Ranged, Bard
    public float moveSpeed; //enemy's move speed
    public float health;

    //Attack
    public float attackCoolDown; //time between attacks
    bool hasAttacked;   //true if currently executing an attack

    //states
    public float sightRange, attackRange;
    bool playerInSight, playerAttackable;

    public float wanderSpeed; // enemy's wandering speed
    public float chaseSpeed; // enemy's run speed

    public float enemyTurnSpeed; // how fast the enemy can turn

    public float groundDrag; // enemy's drag when on the ground
    public float airMultiplier; //enemy's speed multiplier in air
    public float roamRange; //range from current position the walkpoints/flypoints can be set to
    public float flyingHeightMax; // maximum height that a flying enemy can travel to.

    [Header("Transforms")]
    public Transform player;
    public Transform orientation;

    [Header("Ground Detection")]
    public float enemyHeight; // enemy's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    public LayerMask whatIsPlayer; //Layermask for player detection

    Vector3 moveDirection;
    Rigidbody rb;

    //wandering
    public Vector3 walkPoint; //point on the ground to walk to
    bool pointChosen;   //true if a new walkpoint is set

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
        if(enemyType == "Winged_Melee" || enemyType == "Winged_Ranged")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //raycasting for ground detection

        isGrounded = Physics.Raycast(transform.position, Vector3.down, enemyHeight * 0.5f + 0.2f, whatIsGround);

        //apply drag
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        
        //check if player is within sight/attack range

        float distance = Vector3.Distance (transform.position, player.position);

        if(distance < sightRange && distance > attackRange)
        {
            playerInSight = true;
            playerAttackable = false;
        }
        if(distance < sightRange && distance < attackRange)
        {
            playerAttackable = true;
            playerInSight = true;
        }
        if(distance > sightRange)
        {
            playerInSight = false;
            playerAttackable = false;
        }

        if(!playerInSight && !playerAttackable)
        {
            wandering();
        }
        if(playerInSight && !playerAttackable)
        {
            chasing();
        }
        if(playerAttackable)
        {
            attacking();
        }
        // if(hasAttacked)
        // {
        //     coolDown();
        // }
    }

    private void coolDown()
    {
        if(enemyType == "Winged_Melee")
        {
            //attack code
            Vector3 backOff = player.position;
            backOff.y += 5;
            moveEnemy(backOff, 1);
        }

        if(enemyType == "Baic_Melee")
        {
            moveEnemy(transform.position, 2); //stop enemy movement
            transform.LookAt(player); // have enemy face the player
        }

        hasAttacked = false;
    }

    private void wandering()
    {
        Debug.Log("enemy wandering");

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
        float randomZ = Random.Range(-roamRange, roamRange);
        float randomX = Random.Range(-roamRange, roamRange);
        float randomY = Random.Range(flyingHeightMax-1, flyingHeightMax);

        walkPoint = new Vector3();

        switch(enemyType)
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
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && (enemyType != "Winged_Melee" || enemyType != "Winged_Ranged"))
        {
            pointChosen = true;
        }
    }

    private void chasing()
    {
        Debug.Log("enemy chasing");
        Vector3 target;
        switch(enemyType)
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

                if(target.y >= flyingHeightMax)
                {
                    target.y = flyingHeightMax - (float)0.5;
                }
                break;

            case "Winged_Ranged":
                target.x = player.position.x;
                target.z = player.position.z;
                target.y = transform.position.y;
                
                if(target.y >= flyingHeightMax)
                {
                    target.y = flyingHeightMax - (float)0.5;
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
        
        Debug.Log("enemy chasing");
        moveEnemy(target, 1);
    }

    private void attacking()
    {
        Debug.Log("enemy attacking");
        if(!hasAttacked)
        {
            //attack code
            switch(enemyType)
            {
                case "Winged_Melee":
                    Vector3 backOff = new Vector3(transform.position.x, transform.position.y+5, transform.position.z);
                    moveEnemy(backOff, 1);
                    break;

                case "Winged_Ranged":
        
                    break;

                case "Sniper_Ranged":
                    
                    break;

                default:
                    
                    break;
            }

            //finish attack state
            hasAttacked = true;
        }
    }

    private void moveEnemy(Vector3 target, int mode)
    {
        float moveSpeed;
        if(mode == 0)
        {
            moveSpeed = wanderSpeed;
        }
        else if(mode == 1)
        {
            moveSpeed = chaseSpeed;
        }
        else
        {
            moveSpeed = 0;
        }

        // Apply force to the enemy. Variable airMultiplier is used to reduce the enemy's speed in the air
        if (isGrounded)
        {
            //rotate to look at the target
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), enemyTurnSpeed * Time.deltaTime);

            //move towards the target
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

        }
        else if (!isGrounded)
        {
             //rotate to look at the target
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), enemyTurnSpeed * Time.deltaTime);
            
            //move towards the target
            transform.position += transform.forward * Time.deltaTime * moveSpeed * airMultiplier;
        }
    }
}