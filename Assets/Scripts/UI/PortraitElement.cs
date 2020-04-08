using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to define cleanly every graphical portrait element (hair, eyes, mouth) in the game.
/// Those files will be used to handle the portraits generation/customization.
/// </summary>
[CreateAssetMenu(fileName = "New Portrait Element", menuName = "PortraitElement")]
public class PortraitElement : ScriptableObject
{
    public Sprite neutralSprite;
    public Sprite happySprite;
    public Sprite angrySprite;
    [HideInInspector]
    public bool locked;
}
