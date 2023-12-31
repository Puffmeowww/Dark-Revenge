using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    //Movement Speed
    public float speed = 1.0f;
    Coroutine moveCoroutine;
    int targetIndex;
    Vector3[] path;
    PathRequestManager pathRequestManager;

    //Animator
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }


    public void MoveTo(Vector3 PFtargetPosition)
    {
        pathRequestManager.RequestPath(transform.position, PFtargetPosition, OnPathFound);
    }

    //Callback method that is called when a path is found
    void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            // Move the player along the path
            path = newPath;
            //Stop current coroutine
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            //Start a new coroutine
            moveCoroutine = StartCoroutine("FollowPath");
            animator.SetBool("IsMove", true);
        }
        else
        {
            animator.SetBool("IsMove", false);
        }


    }

    IEnumerator FollowPath()
    {
        if (path != null && path.Length > 0)
        {
            Vector3 currentWaypoint = path[0];
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        targetIndex = 0;
                        currentWaypoint = path[targetIndex];
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                
                //FaceTarget(currentWaypoint - transform.position);
                yield return null;
            }
        }
    }

 
}
