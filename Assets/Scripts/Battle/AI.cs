using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType
{
    NONE,
    Clockwise,
    CounterClockwise,
    Drunk,
    DrunkResilient,
    SelfCounter,
    PlayerCounter,
    CommonHuman,
    PlayerStock,
    Throwback,
    SmartHuman,
}

public abstract class AI
{
    protected BattleManager battleManager;

    /// <summary>
    /// Time in milliseconds before the AI will pick a unit and start the battle.
    /// </summary>
    public float SecondsBeforeAction { get; set; }

    /// <summary>
    /// Called every round by the AI.
    /// </summary>
    /// <returns></returns>
    public abstract UnitType PickUnit();
}
