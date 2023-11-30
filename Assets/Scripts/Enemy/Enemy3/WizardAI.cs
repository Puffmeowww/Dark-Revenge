using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAI : EnemyAI
{

    protected State state;
    protected enum State
    {
        UseMagic,
        ChaseTarget,
        MeleeAttack,
        Dead,
        Hurt,
    }


    void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        enemySprite = GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        playerMovement = player.GetComponent<PlayerMovement>();
        //Initialize enemy state
        state = State.UseMagic;

        WarningCanvas.SetActive(false);
    }

    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.UseMagic:

              
                break;


            case State.ChaseTarget:

                pathfindingMovement.speed = 2f;
                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);
                FlipFace(player.transform.position, transform.position);

                FindTarget();
                break;


            case State.MeleeAttack:

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
}
 