using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander
{
    public bool Locked { get; set; }
    public int VictoryCount { get; set; }
    public int LossCount { get; set; }
    public Color Color { get; set; }
    public Portrait Portrait { get; set; }


    public Commander(Color color, Portrait portrait)
    {
        Locked = true;
        VictoryCount = 0;
        LossCount = 0;
        Color = color;
        Portrait = portrait;
    }

    public Commander(bool locked, int victoryCount, int lossCount, Color color, Portrait portrait)
    {
        Locked = locked;
        VictoryCount = victoryCount;
        LossCount = lossCount;
        Color = color;
        Portrait = portrait;
    }
}
