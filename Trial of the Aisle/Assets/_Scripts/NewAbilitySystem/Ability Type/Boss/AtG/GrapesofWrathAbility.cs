using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinionAbility", menuName = "Abilities/Alexander/Fruits of Fury Ability")]
public class FruitsOfFuryAbility : Ability
{
    public GameObject grapeMinionPrefab;
    public int minionCount = 5;
    public float spawnRadius = 3f;
    public SO_AdjustHealth adjustHealthSO; // Reference to the SO_AdjustHealth ScriptableObject
    public float minDistanceFromWalls = 1f; // Minimum distance from walls

    public override void Activate(GameObject owner)
    {
        int spawnedMinions = 0;

        while (spawnedMinions < minionCount)
        {
            Vector3 spawnPosition = owner.transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;

            // Check if the spawnPosition is too close to a wall
            if (!IsCloseToWall(spawnPosition))
            {
                Instantiate(grapeMinionPrefab, spawnPosition, Quaternion.identity);
                spawnedMinions++;
            }
        }
    }

    private bool IsCloseToWall(Vector3 position)
    {
        // Change "Wall" to your wall's layer name or tag
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, minDistanceFromWalls, LayerMask.GetMask("Wall"));
        return colliders.Length > 0;
    }
}

