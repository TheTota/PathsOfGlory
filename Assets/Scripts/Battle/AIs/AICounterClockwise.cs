using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICounterClockwise : AI
{
    private int counterClockwiseIndex;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AICounterClockwise(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 15f;
        counterClockwiseIndex = 4;
    }

    /// <summary>
    /// Picks a unit in the counter-clockwise order, starting by the last one.
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        UnitType pickedUnit = (UnitType)counterClockwiseIndex;

        if (counterClockwiseIndex - 1 < 0) // si on est à 0, on retourne à 4
        {
            counterClockwiseIndex = 4;
        }
        else // sinon on décrémente dans le "sens anti-horaire"
        {
            counterClockwiseIndex--;
        }

        return pickedUnit;
    }
}
