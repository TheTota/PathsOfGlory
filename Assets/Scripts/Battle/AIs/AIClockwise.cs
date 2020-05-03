using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIClockwise : AI
{
    private int clockwiseIndex;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AIClockwise(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 20f;
        clockwiseIndex = 0;
    }

    /// <summary>
    /// Picks a unit in the clockwise order, starting by the 1rst one.
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        UnitType pickedUnit = (UnitType)clockwiseIndex;
        
        if (clockwiseIndex + 1 > 4) // si on est à 4, on retourne à 0
        {
            clockwiseIndex = 0;
        }
        else // sinon on incrémente dans le "sens horaire"
        {
            clockwiseIndex++;
        }

        return pickedUnit;
    }
}
