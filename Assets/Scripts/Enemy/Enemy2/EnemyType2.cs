
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : EnemyAI
{
    protected EnemyType2HealthBar enemy2HealthBar;
    private bool iscollision;
    private bool isDodging = false;
    private float dodgeSpeed = 5.0f;
    private int checkDodge;
    AGrid grid;
    Vector3 targetPosition;
    ANode targetNode;

    protected void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        enemy2HealthBar = GetComponentInChildren<EnemyType2HealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        playerMovement = player.GetComponent<PlayerMovement>();
        state = EnemyAI.State.Roaming;
    }

    void Start()
    {
        GameObject AStarObject = GameObject.Find("A*");
        grid = AStarObject.GetComponent<AGrid>();

        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    void Update()
    {
        Debug.Log("Current State: "+state);
        switch(state)
        {
            default:

            case EnemyAI.State.Roaming:
                animator.SetTrigger("IsWalking");
                pathfindingMovement.speed = 3f;
                pathfindingMovement.MoveTo(roamPosition);
                float reachedPositionDistance = 4f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
                {
                    Debug.Log("New roam position after distance is less than 4");
                    roamPosition = GetRoamingPosition();
                }

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
                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;
                break;

              case EnemyAI.State.Dodge:
                animator.SetTrigger("IsWalking");
                checkDodge = Random.Range(1, 10);
                Debug.Log("Random" + checkDodge);
                isDodging=true;
                Dodge(isDodging);
                break;
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
        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            state = EnemyAI.State.Attack;
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
        }

        //No target in range
        else
        {
            state = EnemyAI.State.Roaming;
        }       
    }

    protected void Attack()
    {
        animator.SetTrigger("IsAttacking");
        Collider2D [] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerMovement>().TakeDamage(enemyDamage);
        }     
    }

    public void TakeDamage(float damageAmount)
    {
        enemyCurrentHealth -= damageAmount;
        enemy2HealthBar.UpdateHealthBar(enemyCurrentHealth, enemyMaxHealth);
        
        if (enemyCurrentHealth <= 0)
        {
            playerMovement.KilledEnemy();
            state = EnemyAI.State.Dead;
            CoinControl.AddCoin(10);
            return;
        }
        else if(enemyCurrentHealth<80)
        {
            Debug.Log("Changing State to Dodge");
            state = State.Dodge;
        }
        
    }

    private void Dodge(bool isDodging)
    {
        Debug.Log("Will it dodge -- "+ isDodging);
        if (isDodging)
        {
            // Choose a random direction to dodge
            Vector3 directionToPlayer = player.transform.position - transform.position;

            // Calculate the desired position away from the player
            targetPosition = transform.position - directionToPlayer.normalized * 6;
            targetNode = grid.NodeFromWorldPoint(targetPosition);
            // Move towards the target position only if the target position is walkable
            if(targetNode.walkable)
            {
                if (Vector3.Distance(transform.position,player.transform.position) < 10f)
                {
                    Debug.Log("Moving away from player");
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);
                }
                if(Vector3.Distance(transform.position,player.transform.position) >= 10f)
                {
                    Debug.Log("Changing state to Roam after dodging");
                    isDodging=false;
                    roamPosition = GetRoamingPosition();
                    FindTarget();
                    
                }
            }

            
        }
    }
}

