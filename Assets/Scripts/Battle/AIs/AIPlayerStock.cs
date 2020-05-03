using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayerStock : AI
{
    /// <summary>
    /// Inits the AI.
    /// </summary>
    /// <param name="bm"></param>
    public AIPlayerStock(BattleManager bm)
    {
        base.battleManager = bm;
        base.SecondsBeforeAction = 7f;
    }

    /// <summary>
    /// Picks a random units that has the max stock in the player's army.
    /// </summary>
    /// <returns></returns>
    public override UnitType PickUnit()
    {
        // Get the max stock in player's army
        int maxStockInPlayerArmy = base.battleManager.PlayerBC.Army.unitsStock.Values.Max();

        // Get units that have the max stock
        int i = 1;
        List<UnitType> matchingUnitsThatAICanUse = GetMatchingUnitsThatAICanUse(maxStockInPlayerArmy);
        while (matchingUnitsThatAICanUse.Count == 0) // go down by 1 each times in the player stock to keep matching the highest possible stock with what the AI possesses
        {
            matchingUnitsThatAICanUse = GetMatchingUnitsThatAICanUse(maxStockInPlayerArmy - i);
        }

        // Return a random unit along these units
        return matchingUnitsThatAICanUse[Random.Range(0, matchingUnitsThatAICanUse.Count - 1)];
    }

    /// <summary>
    /// With a given max stock value, returns the list of units that match that stock in the player's army, that the AI also possesses!
    /// </summary>
    /// <param name="maxStockInPlayerArmy"></param>
    /// <returns></returns>
    private List<UnitType> GetMatchingUnitsThatAICanUse(int maxStockInPlayerArmy)
    {
        List<UnitType> matchingUnits = new List<UnitType>();
        //Knights
        if (base.battleManager.PlayerBC.Army.unitsStock[UnitType.Knights] == maxStockInPlayerArmy)
        {
            matchingUnits.Add(UnitType.Knights);
        }
        //Shields
        if (base.battleManager.PlayerBC.Army.unitsStock[UnitType.Shields] == maxStockInPlayerArmy)
        {
            matchingUnits.Add(UnitType.Shields);
        }
        //Spearmen
        if (base.battleManager.PlayerBC.Army.unitsStock[UnitType.Spearmen] == maxStockInPlayerArmy)
        {
            matchingUnits.Add(UnitType.Spearmen);
        }
        //Mages
        if (base.battleManager.PlayerBC.Army.unitsStock[UnitType.Mages] == maxStockInPlayerArmy)
        {
            matchingUnits.Add(UnitType.Mages);
        }
        //Archers
        if (base.battleManager.PlayerBC.Army.unitsStock[UnitType.Archers] == maxStockInPlayerArmy)
        {
            matchingUnits.Add(UnitType.Archers);
        }

        // Filter with AI stock
        List<UnitType> matchingUnitsThatAICanUse = new List<UnitType>();
        foreach (var unit in matchingUnits)
        {
            if (base.battleManager.EnemyBC.Army.HasStockOf(unit))
            {
                matchingUnitsThatAICanUse.Add(unit); // add if stock of it
            }
        }

        return matchingUnitsThatAICanUse;
    }
}
