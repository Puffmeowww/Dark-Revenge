using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform player;

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
            //spawnPosition = player.position + Random.insideUnitSphere * spawnDistance;
            spawnPosition = CalculateRandomPos();

            spawnNode = grid.NodeFromWorldPoint(spawnPosition);

            if(spawnNode.walkable)
            {
                Debug.Log("Success");
                break;
            }
            Debug.Log("failed");

        }

        spawnPosition.z = 0.0f; 

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

    }


    private Vector3 CalculateRandomPos()
    {

        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0f, spawnDistance);


        Vector3 playerPosition = player.position; 
        float x = playerPosition.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float z = playerPosition.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
        float y = playerPosition.z; 
        return new Vector3(x, y, z);
    }


}
