using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISmartHuman : AI
{
    private bool hasDoneFirstPick;

    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AISmartHuman(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 5f;
        this.hasDoneFirstPick = false;
    }

    /// <summary>
    /// Picks a random unit on first round. Then goes by the following rules : 
    /// - If the AI wins => plays the unit played by the play on the previous round
    /// - If the AI loses => plays one of the units that haven't been played last round
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        // pick a random unit if : first pick OR lost round OR out of stock on last unit
        if (!hasDoneFirstPick)
        {
            this.hasDoneFirstPick = true;
            return base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
        }
        else // || !base.battleManager.EnemyBC.Army.HasStockOf(this.lastPickedUnit))
        {
            // AI won last round, apply AI Win rule unless it cannot be applied because of stock issue, THEN => act as if AI lost
            if (base.battleManager.RoundsWinnersHistory[base.battleManager.CurrentRound - 2] == base.battleManager.EnemyBC)
            {
                UnitType playerLastPickedUnit = base.battleManager.PlayerBC.PlaysHistory[base.battleManager.CurrentRound - 2]; // pick the unit that the player played
                if (base.battleManager.EnemyBC.Army.HasStockOf(playerLastPickedUnit))
                {
                    return playerLastPickedUnit;
                }
            }

            // AI lost or cannot apply Win rule
            // pick a unit that hasn't been played last round, and that the AI has stock of
            List<UnitType> uts = new List<UnitType>() { UnitType.Knights, UnitType.Shields, UnitType.Spearmen, UnitType.Mages, UnitType.Archers };
            List<UnitType> lastRoundAvailableUnplayedUTs = new List<UnitType>();
            foreach (var ut in uts)
            {
                if (ut != base.battleManager.PlayerBC.PlaysHistory[base.battleManager.CurrentRound - 2]
                 && ut != base.battleManager.EnemyBC.PlaysHistory[base.battleManager.CurrentRound - 2]
                 && base.battleManager.EnemyBC.Army.HasStockOf(ut)) // also check stock
                {
                    lastRoundAvailableUnplayedUTs.Add(ut);
                }
            }

            // check if we can get somebody involved
            if (lastRoundAvailableUnplayedUTs.Count != 0)
            {
                return lastRoundAvailableUnplayedUTs[Random.Range(0, lastRoundAvailableUnplayedUTs.Count - 1)];
            }
            else
            {
                return base.battleManager.EnemyBC.Army.GetRandomAvailableUnit();
            }
        }
    }
}
