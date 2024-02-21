using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public abstract class Ability : ScriptableObject
{
    public string abilityName;

    [TextArea(2, 5)]
    public string abilityDescription = "";

    public Sprite abilityIcon;
    public float cooldownTime;
    public int ID;
    // The method that will be overridden by each specific ability
    public abstract void Activate(GameObject owner);
}
