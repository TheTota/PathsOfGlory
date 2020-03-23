using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a commander in the game.
/// </summary>
public class Commander
{
    /// <summary>
    /// Whether or not the commander can be battled by the player.
    /// </summary>
    public bool Locked { get; set; }

    public int WinsCount { get; set; }
    public int LossesCount { get; set; }

    public Color Color { get; set; }
    public Portrait Portrait { get; set; }

    public AIType AiType { get; set; }

    public Commander(Color color, Portrait portrait, bool locked, AIType ai = AIType.NONE)
    {
        Locked = locked;
        WinsCount = 0;
        LossesCount = 0;
        Color = color;
        Portrait = portrait;
        AiType = ai;
    }

    public Commander(Color color, Portrait portrait, AIType ai, bool locked, int victoryCount, int lossCount)
    {
        Locked = locked;
        WinsCount = victoryCount;
        LossesCount = lossCount;
        Color = color;
        Portrait = portrait;
        AiType = ai;
    }
}
