using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISelfCounter : AI
{
    private UnitType lastPickedUnit;
    private bool hasDoneFirstPick;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AISelfCounter(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 15f;
        this.hasDoneFirstPick = false;
    }

    /// <summary>
    /// Picks a first random unit. Then, picks a unit that counters his previously picked unit (2 possibilities).
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        // pick first random unit
        if (!hasDoneFirstPick)
        {
            this.lastPickedUnit = base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
            this.hasDoneFirstPick = true;
        }
        else
        {
            // picks a unit that counters last picked one
            List<UnitType> availableCounters = base.battleManager.EnemyBC.Army.GetAvailableCounters(this.lastPickedUnit);
            if (availableCounters.Count != 0) // if we have available counters, get a random one of em
            {
                this.lastPickedUnit = availableCounters[Random.Range(0, availableCounters.Count - 1)];
            }
            else // if out of stock for counters, pick a random available unit
            {
                this.lastPickedUnit = base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
            }
        }

        return this.lastPickedUnit;
    }
}
