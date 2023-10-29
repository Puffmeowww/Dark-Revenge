using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCompanion : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private float distance;
    private float minDistanceFromPlayer = 2.0f; // Minimum distance from the player
    private float maxDistanceFromPlayer = 5.0f;
    private float minDistanceBetweenCompanions = 2.0f; // Minimum distance between companions
    private float distanceToPlayer;

    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer > minDistanceFromPlayer && distanceToPlayer < maxDistanceFromPlayer)
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;
            MoveTowards(directionToPlayer);
        }
        
        CatCompanion[] companions = FindObjectsOfType<CatCompanion>();
        foreach (CatCompanion otherCompanion in companions)
        {
            if (otherCompanion != this)
            {
                float distanceBetweenCompanions = Vector2.Distance(transform.position, otherCompanion.transform.position);
                if (distanceBetweenCompanions < minDistanceBetweenCompanions)
                {
                    Vector2 directionBetweenCompanions = transform.position - otherCompanion.transform.position;
                    MoveTowards(directionBetweenCompanions);
                }
            }
        }
    }

    void MoveTowards(Vector2 direction)
    {
        direction.Normalize();
        transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;
    }
}
