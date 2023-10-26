using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    //Enemy Health
    public float enemyCurrentHealth = 100f;
    public float enemyMaxHealth = 100f;

    FloatingHealthBar healthBar;


    private Vector3 startingPosition;
    private Vector3 roamPosition;

    private EnemyPathfinding pathfindingMovement;

    public GameObject player;

    private Animator animator;

    private float targetRange = 2f;
    private float attackRange = 1.5f;


    private float nextAttackTime;
    private float attackRate = 3f;



    private enum State
    {
        Roaming,
        ChaseTarget,
        Attack,
        Dead,
    }

    private State state;

    private void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();

        state = State.Roaming;


        //Health Bar
        healthBar = GetComponentInChildren<FloatingHealthBar>();
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
                pathfindingMovement.MoveTo(roamPosition);
                float reachedPositionDistance = 2f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
                {
                    roamPosition = GetRoamingPosition();
                }

                animator.SetBool("IsMove", true);

                if ((roamPosition - transform.position).x > 0)
                {
                    FlipCharacter(-1f);
                }
                else
                {
                    FlipCharacter(1f);
                }

                FindTarget();
                break;

            case State.ChaseTarget:
                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);
                animator.SetBool("IsAttack", false);

                if ((player.transform.position - transform.position).x > 0)
                {
                    FlipCharacter(-1f);
                }
                else
                {
                    FlipCharacter(1f);
                }

                FindTarget();
                break;



            case State.Attack:

                animator.SetBool("IsMove", false);
                animator.SetBool("IsAttack", true);

                if ((player.transform.position - transform.position).x > 0)
                {
                    FlipCharacter(-1f);
                }
                else
                {
                    FlipCharacter(1f);
                }

                if (Time.time > nextAttackTime)
                {
                    state = State.Attack;
                    nextAttackTime = Time.time + attackRate;
                }
                FindTarget();
                break;

            case State.Dead:
                animator.SetBool("IsMove", false);
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsDead", true);
                break;
        }

       



    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(3f, 10f);
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
    }


    private void FlipCharacter(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = direction;
        transform.localScale = scale;
    }


    private void FindTarget()
    {

        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            state = State.Attack;
            return;
        }

        if (Vector3.Distance(transform.position,player.transform.position) < targetRange)
        {
            state = State.ChaseTarget;
        }


        
    }





    public void TakeDamage(float damageAmount)
    {
        enemyCurrentHealth -= damageAmount;

        Debug.Log(enemyCurrentHealth);
        healthBar.UpdateHealthBar(enemyCurrentHealth, enemyMaxHealth);

        if (enemyCurrentHealth <= 0)
        {
            state = State.Dead;
        }

        
    }

}
