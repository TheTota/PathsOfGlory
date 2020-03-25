using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDrunkResilient : AI
{
    private UnitType lastPickedUnit;
    private bool goingResilient;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AIDrunkResilient(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 10f;
        this.goingResilient = false;
    }


    /// <summary>
    /// Picks a random available unit and then keeps picking it until it's out of stock.
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        // Stop picking same unit if we run out of stock
        if (goingResilient && !base.battleManager.EnemyBC.Army.HasStockOf(this.lastPickedUnit))
        {
            this.goingResilient = false;
        }

        if (!goingResilient)
        {
            this.lastPickedUnit = base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
            this.goingResilient = true;
        }

        return this.lastPickedUnit;
    }
}
