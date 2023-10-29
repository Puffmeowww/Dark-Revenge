using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //Player movespeed
    public float moveSpeed = 5f;

    //Player animator
    Animator animator;

    //player attack variables
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public float playerDamage = 20f;

    AudioSource audioSource;
    public AudioClip swordAudio;

    //Player Health
    public float playerCurrentHealth = 100f;
    public float playerMaxHealth = 100f;

    //Health Bar
    FloatingHealthBar healthBar;


    //LoseScreen
    public Canvas loseScreen;

    //Player Sprite
    SpriteRenderer playerSprite;


    //Attack Interval
    public float attackInterval = 0.8f;
    private float lastAttackTime;

    EnemySpawner enemySpawner;

    //Enemy Respawn time
    public float respawnTime;


    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        playerSprite = GetComponent<SpriteRenderer>();

        loseScreen.enabled = false;

        GameObject spawner = GameObject.Find("EnemySpawner");
        enemySpawner = spawner.GetComponent<EnemySpawner>();

        lastAttackTime = -attackInterval;
    }


    private void Update()
    {

        //if normal attack
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackInterval)
        {
            animator.SetBool("Attack1",true);


            audioSource.clip = swordAudio;
            audioSource.Play();
            Attack();
            lastAttackTime = Time.time;
        }

        else
        {
            animator.SetBool("Attack1", false);
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // get mouse location
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // check the mouse's location
        bool isMouseOnLeft = mousePosition.x < transform.position.x;



        // Flip the character
        if (isMouseOnLeft)
        {
            playerSprite.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;
        }

        //Move the character
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * moveSpeed;
        transform.position += movement * Time.deltaTime;
        animator.SetFloat("Speed", movement.magnitude);

    }

    /*
     * 
     * Combat funtions
     * 
     */
    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().TakeDamage(playerDamage);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        playerCurrentHealth -= damageAmount;

        healthBar.UpdateHealthBar(playerCurrentHealth, playerMaxHealth);

        if (playerCurrentHealth <= 0)
        {
            animator.SetTrigger("Death");
            StartCoroutine(PauseAfterTwoSeconds());

        }

        animator.SetBool("Hurt", true);
    }
    public void Recover(float recoverAmount)
    {
        playerCurrentHealth += recoverAmount;
        if(playerCurrentHealth > playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth;
        }
        healthBar.UpdateHealthBar(playerCurrentHealth, playerMaxHealth);
    }

    IEnumerator PauseAfterTwoSeconds()
    {
        yield return new WaitForSeconds(1);
        loseScreen.enabled = true;
        this.enabled = false;
        
    }


    //When player killed enemy, spawn a new enemy
    public void KilledEnemy()
    {
        StartCoroutine(SpawnEnemyWithDelay(respawnTime));
    }

    private IEnumerator SpawnEnemyWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        enemySpawner.SpawnEnemy();
    }


}
