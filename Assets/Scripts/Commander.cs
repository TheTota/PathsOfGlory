﻿using System.Collections;
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

    /// <summary>
    /// Used to save stats.
    /// </summary>
    private int commanderIndex; 

    public Commander(Color color, Portrait portrait, bool locked, AIType ai = AIType.NONE)
    {
        Locked = locked;
        WinsCount = 0;
        LossesCount = 0;
        Color = color;
        Portrait = portrait;
        AiType = ai;
    }

    public Commander(int index, Color color, Portrait portrait, AIType ai, bool locked, int victoryCount, int lossCount)
    {
        Locked = locked;
        WinsCount = victoryCount;
        LossesCount = lossCount;
        Color = color;
        Portrait = portrait;
        AiType = ai;
        commanderIndex = index;
    }

    /// <summary>
    /// Saves : lock status, wins count, losses count.
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_locked", Locked ? 1 : 0);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_wins", WinsCount);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_losses", LossesCount);
    }
}
