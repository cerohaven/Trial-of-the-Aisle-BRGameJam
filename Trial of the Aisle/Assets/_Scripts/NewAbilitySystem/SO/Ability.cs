using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/ParentControllers/Ability")]
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea(2, 5)]
    public string abilityDescription = "";
    public Sprite abilityIcon;
    public float cooldownTime;
    public int ID; // The ID will be set based on its position in the AbilityDatabase list.

    // Abstract method to activate the ability. This needs to be implemented by subclasses.
    public abstract void Activate(GameObject owner);

    // Method to update the ID, called by the AbilityDatabase.
    public void UpdateID(int newID)
    {
        ID = newID;
        // Note: SetDirty is used in the editor script to ensure changes are saved.
    }
}
