using System;
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
    public bool WonLastFightAgainstPlayer { get; set; }

    public Color Color { get; set; }
    public Portrait Portrait { get; set; }

    public AIType AiType { get; set; }
    public PortraitElement PortraitElementToUnlock { get; set; }

    public CommanderDialogs Dialogs { get; set; }

    /// <summary>
    /// Used to save stats.
    /// </summary>
    private int commanderIndex; 

    public Commander(Color color, Portrait portrait, bool locked)
    {
        Locked = locked;
        WinsCount = 0;
        LossesCount = 0;
        Color = color;
        Portrait = portrait;
        AiType = AIType.NONE;
    }

    public Commander(int index, Color color, Portrait portrait, PortraitElement portraitElementToUnlock, AIType ai, bool locked, int victoryCount, int lossCount, bool wonLastFight, CommanderDialogs cd)
    {
        Locked = locked;
        WinsCount = victoryCount;
        LossesCount = lossCount;
        WonLastFightAgainstPlayer = wonLastFight;
        Color = color;
        Portrait = portrait;
        AiType = ai;
        PortraitElementToUnlock = portraitElementToUnlock;
        commanderIndex = index;

        Dialogs = cd;
    }

    /// <summary>
    /// Saves : lock status, wins count, losses count.
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_locked", Locked ? 1 : 0);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_wins", WinsCount);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_losses", LossesCount);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_won_last", WonLastFightAgainstPlayer ? 1 : 0);
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_portrait_hair", Array.IndexOf(PortraitGenerator.Instance.availableHair, Portrait.Hair));
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_portrait_eyes", Array.IndexOf(PortraitGenerator.Instance.availableEyes, Portrait.Eyes));
        PlayerPrefs.SetInt("enemy_" + commanderIndex + "_portrait_mouth", Array.IndexOf(PortraitGenerator.Instance.availableMouth, Portrait.Mouth));

    }
}
