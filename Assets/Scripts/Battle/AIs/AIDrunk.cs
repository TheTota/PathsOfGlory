using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDrunk : AI
{
    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AIDrunk(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 15f;
    }

    /// <summary>
    /// Picks a random available unit everytime.
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        return base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
    }
}
