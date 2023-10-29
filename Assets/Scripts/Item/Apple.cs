using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{

    public GameObject player;
    public float RecoverAmount = 20f;
    PlayerMovement playerMovement;
    AudioSource audioSource;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement.Recover(RecoverAmount);
            this.enabled = false;
            audioSource.Play();
            Destroy(gameObject);
        }
    }

}
