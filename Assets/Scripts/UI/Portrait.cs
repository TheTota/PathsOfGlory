using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait
{
    public PortraitElement Hair { get; set; }
    public PortraitElement Eye { get; set; }
    public PortraitElement Mouth { get; set; }

    public Portrait(PortraitElement hair, PortraitElement eye, PortraitElement mouth)
    {
        Hair = hair;
        Eye = eye;
        Mouth = mouth;
    }
}
