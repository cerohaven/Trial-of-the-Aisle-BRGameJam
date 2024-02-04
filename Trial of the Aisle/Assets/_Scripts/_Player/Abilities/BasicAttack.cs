using System.Collections;
using UnityEngine;

public class BasicAttack : MonoBehaviour, IAbility
{
    public float Cooldown => 1f; // Example cooldown
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "Basic Attack";
    public void Activate(Transform firePoint, GameObject prefab, float force)
    {
        GameObject bullet = GameObject.Instantiate(prefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * force, ForceMode2D.Impulse);
    }

    public IEnumerator CooldownRoutine()
    {
        CanUse = false;
        yield return new WaitForSeconds(Cooldown);
        CanUse = true;
    }

    IEnumerator IAbility.CooldownRoutine()
    {
        throw new System.NotImplementedException();
    }
}