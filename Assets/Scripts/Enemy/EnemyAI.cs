using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    //Enemy Health
    public float enemyCurrentHealth = 100f;
    public float enemyMaxHealth = 100f;

    //Health Bar
    FloatingHealthBar healthBar;

    //Enemy Roaming
    private Vector3 startingPosition;
    private Vector3 roamPosition;

    //Pathfinding
    private EnemyPathfinding pathfindingMovement;
    private GameObject player;

    //Animator
    private Animator animator;

    //Detect Range
    public float targetRange = 4f;
    public float attackRange = 1.5f;

    //Attack Time
    private float nextAttackTime;
    private float attackRate = 1.5f;

    //Attack damage
    public Transform attackPoint;
    public LayerMask playerLayers;
    public float enemyDamage = 5f;

    //Audio resource
    AudioSource audioSource;

    public AudioClip attackAudio;
    public AudioClip findTargetAudio;


    //Enemy Sprite
    SpriteRenderer enemySprite;


    //State Machine
    private enum State
    {
        Roaming,
        ChaseTarget,
        Attack,
        Dead,
        Hurt,
    }

    private State state;

    private void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        //Initialize enemy state
        state = State.Roaming;

        
    }

    private void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    // Update is called once per frame
    private void Update()
    {
        switch(state)
        {
            default:
            case State.Roaming:

                //Debug.Log("Roaming");

                pathfindingMovement.MoveTo(roamPosition);
                float reachedPositionDistance = 2f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
                {
                    roamPosition = GetRoamingPosition();
                }
                else if(animator.GetBool("IsMove") == false)
                {
                    roamPosition = GetRoamingPosition();
                }

                FlipFace(roamPosition, transform.position);

                FindTarget();
                break;

            case State.ChaseTarget:

                //Debug.Log("Chasing Player");

                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);

                FlipFace(player.transform.position, transform.position);

                FindTarget();
                break;



            case State.Attack:


                animator.SetTrigger("Attack");

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



            case State.Dead:
                animator.SetBool("IsMove", false);

                animator.SetTrigger("Death");
                break;


            case State.Hurt:

                animator.SetBool("IsMove", false);
                break;
        }

       



    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(3f, 5f);
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
    }


    //Check the distance between enemy and player
    private void FindTarget()
    {

        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            state = State.Attack;
            return;
        }

        //In chase range
        else if (Vector3.Distance(transform.position,player.transform.position) < targetRange)
        {

            if (audioSource.isPlaying == false)
            {
                audioSource.clip = findTargetAudio;
                audioSource.Play();
                
            }
            state = State.ChaseTarget;
        }

        //No target in range
        else
        {
            state = State.Roaming;
        }


        
    }


    //Take damage from the player
    public void TakeDamage(float damageAmount)
    {
        enemyCurrentHealth -= damageAmount;


        healthBar.UpdateHealthBar(enemyCurrentHealth, enemyMaxHealth);

        if (enemyCurrentHealth <= 0)
        {
            state = State.Dead;
            return;
        }

        state = State.Hurt;

        animator.SetTrigger("Hurt");
        
    }

    //Attack
    private void Attack()
    {
        Collider2D [] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerMovement>().TakeDamage(enemyDamage);
        }
        
    }


    //Make enemy always face the target direction
    private void FlipFace(Vector3 targetPos, Vector3 currentPos)
    {
        if ((targetPos - currentPos).x > 0)
        {
            enemySprite.flipX = true;
        }
        else
        {
            enemySprite.flipX = false;
        }
    }


}
