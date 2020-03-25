using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICommonHuman : AI
{
    private UnitType lastPickedUnit;
    private bool hasDoneFirstPick;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AICommonHuman(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 10f;
        this.hasDoneFirstPick = false;
    }

    /// <summary>
    /// Picks a random unit on first round. Then goes by the following rules : 
    /// - If the AI wins => plays the same unit
    /// - If the AI loses => plays another random unit
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        // pick a random unit if : first pick OR lost round OR out of stock on last unit
        if (!hasDoneFirstPick || base.battleManager.RoundsWinnersHistory[base.battleManager.CurrentRound - 2] == base.battleManager.PlayerBC || !base.battleManager.EnemyBC.Army.HasStockOf(this.lastPickedUnit))
        {
            this.lastPickedUnit = base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
            this.hasDoneFirstPick = true;
        }
        // else obvs, just pick the same unit

        return this.lastPickedUnit;
    }
}
