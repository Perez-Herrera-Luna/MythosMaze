using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterData monster;
    public bool hasWeapon;
    public WeaponData weapon;

    [Header("Enemy Stats")]
    //enemy type
    public string enemyType; //Enemy type: Basic_Melee, Basic_Ranged, Sniper_Ranged, Shotgun_Ranged, 
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
        if(playerInSight && playerAttackable)
        {
            attacking();
        }
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

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //check if the point chosen is valid
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            pointChosen = true;
        }
    }

    private void chasing()
    {
        Vector3 target;
        if(enemyType == "Winged_Melee" || enemyType == "Winged_Ranged")
        {
            
            target.x = player.position.x;
            target.z = player.position.z;
            target.y = transform.position.y;
        }
        else if (enemyType == "Sniper_Ranged")
        {
            target.x = (transform.position.x - player.position.x) * 2;
            target.z = (transform.position.z - player.position.z) * 2;
            target.y = player.position.y;
        }
        else
        {
            target = player.position;
        }
        Debug.Log("enemy chasing");
        moveEnemy(target, 1);
    }

    private void attacking()
    {
        Debug.Log("enemy attacking");
        moveEnemy(transform.position, 2); //stop enemy movement

        transform.LookAt(player); // have enemy face the player

        if(!hasAttacked)
        {
            //attack code

            if(enemyType == "Winged_Melee")
            {
                moveEnemy(player.position, 1);
                //attack code
                Vector3 backOff = player.position;
                backOff.y += 5;
                moveEnemy(backOff, 1);
            }
            //finish attack state
            hasAttacked = true;
            Invoke(nameof(ResetAttack), attackCoolDown);
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
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