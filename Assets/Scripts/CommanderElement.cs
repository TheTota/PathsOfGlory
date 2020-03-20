using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Commander Element", menuName = "CommanderElement")]
public class CommanderElement : ScriptableObject
{
    public bool Locked;
    public Color Color;
    // TODO: add AI enum i guess
}
