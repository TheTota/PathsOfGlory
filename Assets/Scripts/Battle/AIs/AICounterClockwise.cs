using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICounterClockwise : AI
{
    private int counterClockwiseIndex;

    /// <summary>
    /// Inits theAI.
    /// </summary>
    /// <param name="bm"></param>
    public AICounterClockwise(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 5f;
        counterClockwiseIndex = 4;
    }

    /// <summary>
    /// Picks a unit in the clockwise order, starting by the 1rst one.
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
