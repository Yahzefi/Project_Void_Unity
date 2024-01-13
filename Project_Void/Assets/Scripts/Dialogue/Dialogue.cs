using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public int number;
    public string characterName;
    public Sprite characterImage;
    public Color textColor;
    [TextArea(3, 10)]
    public string[] lines;
}
