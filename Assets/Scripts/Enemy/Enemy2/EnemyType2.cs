using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : MonoBehaviour
{
    public enum State
    {
        Roaming,
        ChaseTarget,
        Attack,
        Dead,
        Hurt,
        Dodge,
    }

    public State state;

    protected EnemyPathfinding pathfindingMovement;

    protected GameObject player;

    public float attackRange = 1.5f;

    //public GameObject WarningCanvas;
    protected EnemyType2HealthBar healthBar;
    public GameObject healthCanvas;

    public float targetRange = 4f;

    protected AudioSource audioSource;
    public AudioClip findTargetAudio;

    protected Animator animator;

    protected Vector3 startingPosition;
    protected Vector3 roamPosition;

    protected SpriteRenderer enemySprite;
    private bool iscollision;
    protected float nextAttackTime;
    protected float attackRate = 1.5f;
    public Transform attackPoint;

    public LayerMask playerLayers;

    public float enemyCurrentHealth = 100f;

    public float enemyMaxHealth = 100f;

    protected PlayerMovement playerMovement;

    public AudioClip attackAudio;

    public float enemyDamage = 5f;


    protected void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<EnemyType2HealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        playerMovement = player.GetComponent<PlayerMovement>();
        //Initialize enemy state
        state = State.Roaming;

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
        switch(state)
        {
            default:
            case State.Roaming:

                pathfindingMovement.speed = 3f;
                // Debug.Log("Roam 1 --" +roamPosition);
                // Debug.Log("Start current pos" + transform.position);
                pathfindingMovement.MoveTo(roamPosition);
                //CheckWallCollision();
                float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
                {
                    roamPosition = GetRoamingPosition();
                    // Debug.Log("Inside if current pos" + transform.position);
                    // Debug.Log("Roam 2 --" +roamPosition);
                }

                // else if(animator.GetBool("IsMove") == false)
                // {
                //     roamPosition = GetRoamingPosition();
                //     Debug.Log("Roam 3 --" +roamPosition);
                // }

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

                //WarningCanvas.SetActive(false);
                animator.SetBool("IsMove", false);
                animator.SetTrigger("Death");
  
                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;


                break;

        }

    }

    protected Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(1f, 20f);
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
    }

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

    protected virtual void FindTarget()
    {

        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {

            //WarningCanvas.SetActive(false);
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
            //WarningCanvas.SetActive(true);
        }

        //No target in range
        else
        {
            //WarningCanvas.SetActive(false);
            state = State.Roaming;
        }       
    }

    public bool CheckWallCollision()
    {
        Debug.Log("Inside chk collision");
        // Raycast to check for collisions with the wall
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.1f);

        // If a collision is detected, handle it
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            // Handle wall collision here
            Debug.Log("Enemy collided with a wall!");
            iscollision = true;
        }
        return iscollision;
    }

    protected void Attack()
    {
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
}
