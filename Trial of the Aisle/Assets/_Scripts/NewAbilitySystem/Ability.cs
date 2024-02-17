using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public float cooldownTime;

    // The method that will be overridden by each specific ability
    public abstract void Activate(GameObject owner);
}