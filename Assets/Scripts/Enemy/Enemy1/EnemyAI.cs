using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    //Enemy Health
    public float enemyCurrentHealth = 100f;
    public float enemyMaxHealth = 100f;

    //Health Bar
    protected FloatingHealthBar healthBar;
    public GameObject healthCanvas;

    //Warning canvas
    public GameObject WarningCanvas;

    //Enemy Roaming
    protected Vector3 startingPosition;
    protected Vector3 roamPosition;

    //Pathfinding
    protected EnemyPathfinding pathfindingMovement;
    protected GameObject player;

    //Animator
    protected Animator animator;

    //Detect Range
    public float targetRange = 4f;
    public float attackRange = 1.5f;

    //Attack Time
    protected float nextAttackTime;
    protected float attackRate = 1.5f;

    //Attack damage
    public Transform attackPoint;
    public LayerMask playerLayers;
    public float enemyDamage = 5f;

    //Audio resource
    protected AudioSource audioSource;

    public AudioClip attackAudio;
    public AudioClip findTargetAudio;

    protected PlayerMovement playerMovement;


    //Enemy Sprite
    protected SpriteRenderer enemySprite;


    //State Machine
    public enum State
    {
        Roaming,
        ChaseTarget,
        Attack,
        Dead,
        Hurt,
        UseMagic,
        MeleeAttack,
    }

    public State state;

    protected void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        playerMovement = player.GetComponent<PlayerMovement>();
        //Initialize enemy state
        state = State.Roaming;

        WarningCanvas.SetActive(false);
    }

    protected void Start()
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

                pathfindingMovement.speed = 1f;
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

                pathfindingMovement.speed = 2f;
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

                WarningCanvas.SetActive(false);
                animator.SetBool("IsMove", false);
                animator.SetTrigger("Death");
  
                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;


                break;

            case State.Hurt:
                animator.SetBool("IsMove", false);
                break;
        }
    }



    protected Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(3f, 5f);
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
    }


    //Check the distance between enemy and player
    protected virtual void FindTarget()
    {

        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {

            WarningCanvas.SetActive(false);
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
            WarningCanvas.SetActive(true);
        }

        //No target in range
        else
        {
            WarningCanvas.SetActive(false);
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
            playerMovement.KilledEnemy();
            state = State.Dead;
            CoinControl.AddCoin(10);
            return;
        }

        state = State.Hurt;

        animator.SetTrigger("Hurt");
        
    }

    //Attack
    protected void Attack()
    {
        Collider2D [] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerMovement>().TakeDamage(enemyDamage);
        }     
    }


    //Make enemy always face the target direction
    protected virtual void FlipFace(Vector3 targetPos, Vector3 currentPos)
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
