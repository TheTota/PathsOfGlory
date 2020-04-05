using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A commander's portrait, in 1 var instead of many.
/// </summary>
public class Portrait
{
    public PortraitElement Hair { get; set; }
    public PortraitElement Eyes { get; set; }
    public PortraitElement Mouth { get; set; }
    public Color SkinTone { get; internal set; }

    public Portrait(PortraitElement hair, PortraitElement eye, PortraitElement mouth, Color skinTone)
    {
        Hair = hair;
        Eyes = eye;
        Mouth = mouth;
        SkinTone = skinTone;
    }
}
