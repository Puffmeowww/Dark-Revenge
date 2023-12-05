
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : EnemyAI
{
    // public enum State
    // {
    //     Roaming,
    //     ChaseTarget,
    //     Attack,
    //     Dead,
    //     Hurt,
    //     Dodge,
    // }

    //public State state;

    //protected EnemyPathfinding pathfindingMovement;

    //protected GameObject player;

    //public float attackRange = 1.5f;

    //public GameObject WarningCanvas;

    protected EnemyType2HealthBar enemy2HealthBar;
    //public GameObject healthCanvas;


    //public float targetRange = 4f;

    //protected AudioSource audioSource;
    //public AudioClip findTargetAudio;

    //protected Animator animator;

    //protected Vector3 startingPosition;
    //protected Vector3 roamPosition;

    //protected SpriteRenderer enemySprite;
    private bool iscollision;
    // protected float nextAttackTime;
    // protected float attackRate = 1.5f;
    // public Transform attackPoint;

    // public LayerMask playerLayers;

    // public float enemyCurrentHealth = 100f;

    // public float enemyMaxHealth = 100f;

    // protected PlayerMovement playerMovement;

    // public AudioClip attackAudio;

    // public float enemyDamage = 5f;

    private bool isDodging = false;
    private float dodgeDuration = 2.0f; // Adjust the duration as needed
    private float nextDodgeEndTime;
    private float dodgeSpeed = 5.0f;
    private int checkDodge;
    public float distanceToPlayer = 1f;


    protected void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        enemy2HealthBar = GetComponentInChildren<EnemyType2HealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        playerMovement = player.GetComponent<PlayerMovement>();
        //Initialize enemy state
        state = EnemyAI.State.Roaming;

        //WarningCanvas.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current State: "+state);
        switch(state)
        {
            default:
            case EnemyAI.State.Roaming:
                animator.SetTrigger("IsWalking");
                pathfindingMovement.speed = 3f;
                //Debug.Log("Step 1 Roaming");
                //Debug.Log("Start current pos" + transform.position);
                pathfindingMovement.MoveTo(roamPosition);
                //CheckWallCollision();
                float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
                {
                    roamPosition = GetRoamingPosition();
                    //Debug.Log("Step 2 Finding new roam position");
                }

                // else if(animator.GetBool("IsMove") == false)
                // {
                //     roamPosition = GetRoamingPosition();
                //     Debug.Log("Roam 3 --" +roamPosition);
                // }

                FlipFace(roamPosition, transform.position);

                FindTarget();
                break;

            case EnemyAI.State.ChaseTarget:
                animator.SetTrigger("IsWalking");
                //Debug.Log("Moving towards player");
                pathfindingMovement.speed = 2f;
                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);
                FlipFace(player.transform.position, transform.position);

                FindTarget();
                break;

            case EnemyAI.State.Attack:
                
                FlipFace(player.transform.position, transform.position);

                if (audioSource.isPlaying == false)
                {
                    audioSource.clip = attackAudio;
                    audioSource.Play();
                }

                if (Time.time > nextAttackTime)
                {
                    nextAttackTime = Time.time + attackRate;
                    Attack();   
                }
                
                FindTarget();
                break;

            case EnemyAI.State.Dead:
                animator.SetTrigger("IsDead");
                //WarningCanvas.SetActive(false);
                //animator.SetBool("IsMove", false);
                //animator.SetTrigger("Death");
  
                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;


                break;

              case EnemyAI.State.Dodge:
                // checkDodge = Random.Range(1, 10);
                // if(checkDodge % 2 == 0)
                // {
                //     //isDodging=true;
                // }
                // else
                // {
                    
                //     //Debug.Log("D
                // }
                // isDodging=true;
                FindTarget();
                //Dodge(isDodging);
                break;

            //     Debug.Log("Inside Dodge state");
            //     checkDodge = Random.Range(1, 10);
            //     //Debug.Log("Random value : "+checkDodge);
            //     if(checkDodge % 2 == 0)
            //     {
            //         //Debug.Log("Dodging is true");
            //         //isDodging=true;
            //     }
            //     else
            //     {
            //         //isDodging=false;
            //         //Debug.Log("Dodging is False");
            //     }
            //     isDodging=true;
            //     Dodge(isDodging);

            //     // Transition back to Roaming after dodging
            //     // if (Time.time > nextDodgeEndTime)
            //     // {
            //     //     state = State.Roaming;
            //     // }
            //     break;

        }

    }

    protected Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(1f, 15f);
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
    }

    protected virtual void FlipFace(Vector3 targetPos, Vector3 currentPos)
    {
        if ((targetPos - currentPos).x > 0)
        {
            enemySprite.flipX = false;
        }
        else
        {
            enemySprite.flipX = true;
        }
    }

    protected virtual void FindTarget()
    {
        //Debug.Log("current state Inside Find target: "+state);
        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {

            //WarningCanvas.SetActive(false);
            state = EnemyAI.State.Attack;
            //Debug.Log("current state inside 1st if: "+state);
            //return;
        }

        //In chase range
        else if (Vector3.Distance(transform.position,player.transform.position) < targetRange)
        {

            if (audioSource.isPlaying == false)
            {
                audioSource.clip = findTargetAudio;
                audioSource.Play();
                
            }
            state = EnemyAI.State.ChaseTarget;
            //Debug.Log("current state inside 2nd if: "+state);
            //WarningCanvas.SetActive(true);
        }

        //No target in range
        else
        {
            //WarningCanvas.SetActive(false);
            //roamPosition = GetRoamingPosition();
            state = EnemyAI.State.Roaming;
            //Debug.Log("current state inside 3rd if: "+state);
        }       
    }

    public bool CheckWallCollision()
    {
        //Debug.Log("Inside chk collision");
        // Raycast to check for collisions with the wall
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.1f);

        // If a collision is detected, handle it
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            // Handle wall collision here
            //Debug.Log("Enemy collided with a wall!");
            iscollision = true;
        }
        return iscollision;
    }

    protected void Attack()
    {
        Debug.Log("Attacking player");
        animator.SetTrigger("IsAttacking");
        Collider2D [] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerMovement>().TakeDamage(enemyDamage);
        }     
    }

    public void TakeDamage(float damageAmount)
    {
        Debug.Log("Enemy 2 attacked");
        enemyCurrentHealth -= damageAmount;
        Debug.Log("Enemy Damaged");
        enemy2HealthBar.UpdateHealthBar(enemyCurrentHealth, enemyMaxHealth);
        
        if (enemyCurrentHealth <= 0)
        {
            Debug.Log("Enemy 2 died");
            playerMovement.KilledEnemy();
            state = EnemyAI.State.Dead;
            CoinControl.AddCoin(10);
            return;
        }
        // else if(enemyCurrentHealth<50)
        // {
        //     Debug.Log("Changing State to Dodge");
        //     state = State.Dodge;
        // }

        //animator.SetTrigger("Hurt");
        
    }

    private void Dodge(bool isDodging)
    {
        Debug.Log("Will it dodge -- "+ isDodging);
        if (isDodging)
        {
            // Choose a random direction to dodge
            Debug.Log("Dodging is true");
            Vector3 directionToPlayer = player.transform.position - transform.position;


            // Calculate the desired position away from the player
            Vector3 targetPosition = transform.position - directionToPlayer.normalized * 6;

            // Move towards the target position
            Debug.Log("Moving away from player");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position,player.transform.position) > 5f)
            {
                Debug.Log("Changing state to Roam after dodging");
                state = EnemyAI.State.Roaming;
            }

            
        }
    }
}

