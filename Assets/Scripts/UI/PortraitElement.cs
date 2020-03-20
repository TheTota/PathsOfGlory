using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Portrait Element", menuName = "PortraitElement")]
public class PortraitElement : ScriptableObject
{
    public Sprite sprite;
    public bool locked;
}
