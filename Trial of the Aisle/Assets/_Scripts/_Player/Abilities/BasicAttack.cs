using System.Collections;
using UnityEngine;

public class BasicAttack : MonoBehaviour, IAbility
{
    public GameObject AbilityPrefab { get; private set; } // Assign in Inspector or Awake/Start
    public float AbilityForce { get; private set; } = 10f; // Example force value

    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;
    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
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