using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{

    public GameObject applePrefab;
    public Transform player;

    AGrid grid;

    public float spawnDistance = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject AStarObject = GameObject.Find("A*");
        grid = AStarObject.GetComponent<AGrid>();
    }

    // Update is called once per frame
    void Update()
    {

        GameObject[] appleList = GameObject.FindGameObjectsWithTag("Apple");

        if (appleList.Length == 0 && player.gameObject.GetComponent<PlayerMovement>().playerCurrentHealth <= 50)
        {
            SpawnApple();
            Debug.Log("Spawn Apple");
        }
        else
        {
        }
    }

    public void SpawnApple()
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
        GameObject newApple = Instantiate(applePrefab, spawnPosition, Quaternion.identity);

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
