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

public class AI
{
    private BattleManager battleManager;

    private AIType type;
    /// <summary>
    /// Time in milliseconds before the AI will pick a unit and start the battle.
    /// </summary>
    public float MillisBeforeAction { get; set; }

    // picking action
    public delegate UnitType PickUnitWithAI();
    private PickUnitWithAI aiUnitPick;

    /// <summary>
    /// Inits the AI completly so it's ready to play.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="bm"></param>
    public AI(AIType type, BattleManager bm)
    {
        this.type = type;

        if (type != AIType.NONE)
        {
            switch (type)
            {
                case AIType.Clockwise:
                    MillisBeforeAction = 7000;
                    aiUnitPick = PickClockwise();
                    break;

                case AIType.CounterClockwise:
                    MillisBeforeAction = 6500;
                    break;

                case AIType.Drunk:
                    MillisBeforeAction = 6000;
                    break;

                case AIType.DrunkResilient:
                    MillisBeforeAction = 5500;
                    break;

                case AIType.SelfCounter:
                    MillisBeforeAction = 5000;
                    break;

                case AIType.PlayerCounter:
                    MillisBeforeAction = 4500;
                    break;

                case AIType.CommonHuman:
                    MillisBeforeAction = 4000;
                    break;

                case AIType.PlayerStock:
                    MillisBeforeAction = 3500;
                    break;

                case AIType.Throwback:
                    MillisBeforeAction = 3000;
                    break;

                case AIType.SmartHuman:
                    MillisBeforeAction = 2500;
                    break;

                default:
                    throw new Exception("AI not found.");
            }
        }
    }

    /// <summary>
    /// Called every round by the AI.
    /// </summary>
    /// <returns></returns>
    public UnitType PickUnit()
    {
        return aiUnitPick();
    }

    ///// TODO : héritage des IAs

    /// <summary>
    /// Picks a unit in the clockwise order, starting by the 1rst one.
    /// </summary>
    /// <returns></returns>
    public UnitType PickClockwise()
    {
        return (UnitType)battleManager.CurrentRound;
    }
}
