using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAttribute : PropertyAttribute
{
    //Make all the variables the user will input as readonly
    public readonly string titleText;
    public readonly int titleSize;
    public readonly bool titleUnderline;
    public readonly TextAlignment titleAlignment;
    public readonly TextColour titleColour;


    //now make the attribute a constructor so people can input the values
    public TitleAttribute(string _text,
                          TextAlignment _alignment = TextAlignment.Left,
                          TextColour _colour = TextColour.White,
                          int _fontSize = 30)
    {
        //Set the values
        titleColour = _colour;
        titleAlignment = _alignment;
        titleText = _text;
        titleSize = _fontSize;
    }
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}

public enum TextColour
{
    White,
    Black,
    Red,
    Blue,
    Yellow,
    Green,
    Orange,
    Purple,
    Pink,
    Brown,
    Aqua
}