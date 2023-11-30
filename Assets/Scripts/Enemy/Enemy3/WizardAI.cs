using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAI : EnemyAI
{

    public float magicIntervalTime = 3;
    public GameObject projectilePrefab;
    private bool isSpawning = false;


    protected WizardState wizardState;
    protected enum WizardState
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
        wizardState = WizardState.UseMagic;

        WarningCanvas.SetActive(false);
    }

    void Start()
    {
     
    }

    IEnumerator SpawnProjectile()
    {
        isSpawning = true;

        while (true)
        {
            //Debug.Log("Spawn");
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
        isSpawning = false;
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
        switch (wizardState)
        {
            default:
            case WizardState.UseMagic:

                if (!isSpawning)
            {
                StartCoroutine(SpawnProjectile());
            }


                break;


            case WizardState.ChaseTarget:

                pathfindingMovement.speed = 2f;
                pathfindingMovement.MoveTo(player.transform.position);
                animator.SetBool("IsMove", true);
                FlipFace(player.transform.position, transform.position);

                FindTarget();
                break;


            case WizardState.MeleeAttack:

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

            case WizardState.Dead:

                WarningCanvas.SetActive(false);
                animator.SetBool("IsMove", false);
                animator.SetTrigger("Death");

                Destroy(healthCanvas);
                Destroy(gameObject);
                this.enabled = false;


                break;

            case WizardState.Hurt:
                animator.SetBool("IsMove", false);
                break;
        }
    }
}
 