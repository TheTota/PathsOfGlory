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

    /// <summary>
    /// Gives the unit played for a round n at the index n-1.
    /// </summary>
    public List<UnitType> PlaysHistory { get; set; }

    /// <summary>
    /// Inits a battle commander at the beginning of the battle.
    /// </summary>
    /// <param name="c"></param>
    public void Init(Commander c)
    {
        PlaysHistory = new List<UnitType>();
        this.Commander = c;
        this.Army = new Army();
        this.Score = 0;
    }

    /// <summary>
    /// Adds a unit to the BC's plays history.
    /// </summary>
    /// <param name="unit"></param>
    public void AddToPlaysHistory(UnitType unit)
    {
        this.PlaysHistory.Add(unit);
    }
}
