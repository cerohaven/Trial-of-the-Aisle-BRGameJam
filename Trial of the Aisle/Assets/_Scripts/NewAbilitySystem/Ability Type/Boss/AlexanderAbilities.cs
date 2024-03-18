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

[CreateAssetMenu(fileName = "SlowAbility", menuName = "Abilities/Alexander/Jammed Ability")]
public class JammedAbility : Ability
{
    public GameObject jamPrefab;
    public float jamThrowForce;

    public override void Activate(GameObject owner)
    {
        // Instantiate the jam at the owner's position
        GameObject jam = Instantiate(jamPrefab, owner.transform.position, Quaternion.identity);

        // Calculate the direction to throw the jam towards the mouse position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = owner.transform.position.z;
        Vector2 direction = (mouseWorldPosition - owner.transform.position).normalized;

        // Apply force to the jam to throw it in the direction
        Rigidbody2D rb = jam.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * jamThrowForce, ForceMode2D.Impulse);

        //add the logic for the jam to slow down the boss when it comes in contact (Unimplemented)
    }
}
