using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;


    public Transform player;

    //The probability of different enemy types
    float enemy1P;
    float enemy2P;
    float enemy3P;

    AGrid grid;

    public float spawnDistance = 5.0f; 

    // Start is called before the first frame update
    void Start()
    {
        GameObject AStarObject = GameObject.Find("A*");
        grid = AStarObject.GetComponent<AGrid>();
    }



    public void SpawnEnemy()
    {


        Vector3 spawnPosition = default;
        ANode spawnNode = default;
        while (true)
        {
            spawnPosition = CalculateRandomPos();
            spawnNode = grid.NodeFromWorldPoint(spawnPosition);

            if (spawnNode.walkable)
            {
                
                break;
            }

        }

        spawnPosition.z = 0.0f;

        int randomNumber = Random.Range(1, 101);
        if (CoinControl.coinNum >= 0 && CoinControl.coinNum <= 30)
        {
            enemy1P = 70;
            enemy2P = 20;
            enemy3P = 10;
        }
        else if (CoinControl.coinNum > 30 && CoinControl.coinNum <= 80)
        {
            enemy1P = 50;
            enemy2P = 30;
            enemy3P = 20;
        }
        else
        {
            enemy1P = 20;
            enemy2P = 30;
            enemy3P = 50;
        }


        if(randomNumber <= enemy1P)
        {
            GameObject newEnemy = Instantiate(enemy1Prefab, spawnPosition, Quaternion.identity);
        }
        else if(randomNumber > enemy1P && randomNumber <= (enemy1P + enemy2P))
        {
            GameObject newEnemy = Instantiate(enemy2Prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            GameObject newEnemy = Instantiate(enemy3Prefab, spawnPosition, Quaternion.identity);
        }

        

    }


    private Vector3 CalculateRandomPos()
    {

        /*        float randomAngle = Random.Range(0f, 360f);
                float randomRadius = Random.Range(0f, spawnDistance);


                Vector3 playerPosition = player.position; 
                float x = playerPosition.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
                float z = playerPosition.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
                float y = playerPosition.z; */

        Vector3 playerPosition = player.position;
        float x = Random.Range((playerPosition.x - spawnDistance), (playerPosition.x + spawnDistance));
        float y = Random.Range((playerPosition.y - spawnDistance), (playerPosition.y + spawnDistance));
        float z = playerPosition.z;

        return new Vector3(x, y, z);
    }


}
