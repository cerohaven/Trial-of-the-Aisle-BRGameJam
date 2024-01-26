using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Control Scheme HUD", menuName = "Scriptable Objects/Control Scheme HUD")]
public class SO_ControlSchemeHUD : ScriptableObject
{
    public enum SpriteType
    {
        Img_Interact,
        Img_Confirm,
        Img_Back
    }


    [Title("Input HUD Icons",TextAlignment.Center,TextColour.Aqua,30)]
    [Separator(5,10)]
    public List<ControlSchemeHUD> controlSchemes;

    public Sprite UpdateSpriteHUD(string _controlScheme, SpriteType spriteType)
    {
        //Go through the list and find the control scheme with the same name
        foreach (ControlSchemeHUD controlScheme in controlSchemes)
        {
            if (controlScheme.controlSchemeName != _controlScheme)
                continue;
            
            switch(spriteType)
            {
                case SpriteType.Img_Interact:
                    return controlScheme.interactSprite;

                case SpriteType.Img_Confirm:
                    return controlScheme.confirmSprite;

                case SpriteType.Img_Back:
                    return controlScheme.backSprite;

                default:
                    break;

            }
            
        }

        Debug.LogError("The Control Scheme changed to is not in this List! Please Update the 'Control Scheme HUD' List with it");
        return null;
    }

}

[System.Serializable]
public class ControlSchemeHUD
{
    public string controlSchemeName;
    public Sprite interactSprite;
    public Sprite confirmSprite;
    public Sprite backSprite;

}