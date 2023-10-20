using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private Vector3 startingPosition;
    private Vector3 roamPosition;

    private EnemyPathfinding pathfindingMovement;

    public GameObject player;

    private Animator animator;

    private enum State
    {
        Roaming,
        ChaseTarget,
    }

    private State state;

    private void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();

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

                if ((player.transform.position - transform.position).x > 0)
                {
                    FlipCharacter(-1f);
                }
                else
                {
                    FlipCharacter(1f);
                }

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
        float targetRange = 2f;
        if(Vector3.Distance(transform.position,player.transform.position) < targetRange)
        {
            state = State.ChaseTarget;
        }
    }

}
