using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to define cleanly every commander in the game.
/// Those files will be used to init the commanders.
/// </summary>
[CreateAssetMenu(fileName = "New Commander Element", menuName = "CommanderElement")]
public class CommanderElement : ScriptableObject
{
    public bool Locked;
    public Color Color;
    // TODO: add AI enum i guess
}
