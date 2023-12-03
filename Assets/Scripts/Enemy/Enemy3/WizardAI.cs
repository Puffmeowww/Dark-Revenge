using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAI : EnemyAI
{

    public float magicIntervalTime = 3f;
    public GameObject projectilePrefab;
    private bool isSpawning = false;

    private float timer = 0f;


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
        state = EnemyAI.State.UseMagic;

        WarningCanvas.SetActive(false);
    }

    void Start()
    {
        StartCoroutine("SpawnProjectile");
        isSpawning = true;
    }

    IEnumerator SpawnProjectile()
    {
        //isSpawning = true;

        while (true)
        {
            SetProjectile(new Vector3(1, 0, 0));
            SetProjectile(new Vector3(1, 1, 0));

            SetProjectile(new Vector3(0, 1, 0));
            SetProjectile(new Vector3(-1, 1, 0));

            SetProjectile(new Vector3(-1, 0, 0));
            SetProjectile(new Vector3(-1, -1, 0));

            SetProjectile(new Vector3(0, -1, 0));
            SetProjectile(new Vector3(1, -1, 0));

            yield return new WaitForSeconds(magicIntervalTime);
        }
        //isSpawning = false;
    }

    public void SetProjectile(Vector3 direction)
    {
        GameObject newObject = Instantiate(projectilePrefab,transform.position,transform.rotation);
        Projectile pj = newObject.GetComponent<Projectile>();
        pj.direction = direction;

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case EnemyAI.State.UseMagic:

                FindTarget();
                break;


            case EnemyAI.State.ChaseTarget:

                isSpawning = false;
                StopCoroutine("SpawnProjectile");

                pathfindingMovement.speed = 2f;
                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);
                FlipFace(player.transform.position, transform.position);

                FindTarget();
                break;


            case EnemyAI.State.MeleeAttack:

                isSpawning = false;
                StopCoroutine("SpawnProjectile");

                animator.SetBool("IsMove", false);
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

            case EnemyAI.State.Dead:

                isSpawning = false;
                StopCoroutine("SpawnProjectile");

                WarningCanvas.SetActive(false);
                animator.SetBool("IsMove", false);
                animator.SetTrigger("Death");

                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;


                break;

            case EnemyAI.State.Hurt:

                isSpawning = false;
                StopCoroutine("SpawnProjectile");

                animator.SetTrigger("Hurt");

                animator.SetBool("IsMove", false);
                break;
        }
    }

    protected override void FindTarget()
    {

        //In attack range
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {

            WarningCanvas.SetActive(false);
            state = EnemyAI.State.MeleeAttack;
            return;
        }

        //In chase range
        else if (Vector3.Distance(transform.position, player.transform.position) < targetRange)
        {

            if (audioSource.isPlaying == false)
            {
                audioSource.clip = findTargetAudio;
                audioSource.Play();

            }
            state = EnemyAI.State.ChaseTarget;
            WarningCanvas.SetActive(true);
            return;
        }

        //No target in melee range, use magic
        else
        {
            WarningCanvas.SetActive(false);
            state = EnemyAI.State.UseMagic;

            if(isSpawning == false)
            {
                StartCoroutine("SpawnProjectile");
                isSpawning = true;
            }

            return;
        }
    }


    protected override void FlipFace(Vector3 targetPos, Vector3 currentPos)
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
}
 