using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    PlayerMovement player;

    public float damage = 10f;

    public float speed = 20f;

    public Vector3 direction;

    public float destroyTime = 6f;
    private float timeCount = 0;



    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.Find("player");
        player = playerObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        if(timeCount >= destroyTime)
        {
            
            Destroy(gameObject);
        }
        timeCount += Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("damage");
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


}
