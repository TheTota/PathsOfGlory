using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente un commandant dans le cadre d'une bataille.
/// </summary>
public class BattleCommander : MonoBehaviour
{
    public Commander Commander { get; set; }

    public int Score { get; set; }

    public Army Army { get; set; }

    public UnitType ChosenUnit { get; set; }

    [SerializeField]
    private PortraitRenderer portraitRenderer;

    /// <summary>
    /// Gives the unit played for a round n at the index n-1.
    /// </summary>
    public UnitType[] playsHistory { get; set; }

    /// <summary>
    /// Inits a battle commander at the beginning of the battle.
    /// </summary>
    /// <param name="c"></param>
    public void Init(Commander c)
    {
        this.Commander = c;
        if (this.portraitRenderer != null)
        {
            this.portraitRenderer.RenderPortrait(c);
        }
    }
}
