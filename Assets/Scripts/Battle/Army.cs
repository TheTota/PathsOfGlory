using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enumération pour les 5 types d'unités du jeu.
/// </summary>
public enum UnitType
{
    Knights,
    Shields,
    Spearmen,
    Mages,
    Archers,
}

/// <summary>
/// Représente l'armée d'un commandant lors d'une bataille.
/// </summary>
public class Army
{
    private const int INITIAL_STOCK_OF_EACH = 3;

    // armies stock
    public Dictionary<UnitType, int> unitsStock;

    /// <summary>
    /// Inits the stocks of the army to the class constant.
    /// </summary>
    public Army()
    {
        unitsStock = new Dictionary<UnitType, int>();

        unitsStock.Add(UnitType.Knights, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Shields, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Spearmen, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Mages, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Archers, INITIAL_STOCK_OF_EACH);
    }

    /// <summary>
    /// Returns the counters of the given unit.
    /// </summary>
    /// <param name="ut"></param>
    /// <returns></returns>
    public List<UnitType> GetAvailableCounters(UnitType ut)
    {
        List<UnitType> availableCounters;

        // Add counters
        switch (ut)
        {
            case UnitType.Knights:
                availableCounters = new List<UnitType>() { UnitType.Spearmen, UnitType.Shields };
                break;

            case UnitType.Shields:
                availableCounters = new List<UnitType>() { UnitType.Mages, UnitType.Spearmen };
                break;

            case UnitType.Spearmen:
                availableCounters = new List<UnitType>() { UnitType.Archers, UnitType.Mages };
                break;

            case UnitType.Mages:
                availableCounters = new List<UnitType>() { UnitType.Knights, UnitType.Archers };
                break;

            case UnitType.Archers:
                availableCounters = new List<UnitType>() { UnitType.Shields, UnitType.Knights };
                break;

            default:
                throw new Exception("Cant identify the unit type");
        }

        // Filter with stock availability
        // Knights
        if (availableCounters.Contains(UnitType.Knights) && unitsStock[UnitType.Knights] == 0)
        {
            availableCounters.Remove(UnitType.Knights);
        }
        
        // Shields
        if (availableCounters.Contains(UnitType.Shields) && unitsStock[UnitType.Shields] == 0)
        {
            availableCounters.Remove(UnitType.Shields);
        }

        // Spearmen
        if (availableCounters.Contains(UnitType.Spearmen) && unitsStock[UnitType.Spearmen] == 0)
        {
            availableCounters.Remove(UnitType.Spearmen);
        }

        // Mages
        if (availableCounters.Contains(UnitType.Mages) && unitsStock[UnitType.Mages] == 0)
        {
            availableCounters.Remove(UnitType.Mages);
        }

        // Archers
        if (availableCounters.Contains(UnitType.Archers) && unitsStock[UnitType.Archers] == 0)
        {
            availableCounters.Remove(UnitType.Archers);
        }

        return availableCounters;
    }

    /// <summary>
    /// Removes a unit of the given type from the stock, if stock isn't empty.
    /// </summary>
    /// <param name="ut">Type of unit to remove from stock.</param>
    public void RemoveUnitFromStock(UnitType ut)
    {
        if (unitsStock[ut] > 0)
        {
            unitsStock[ut]--;
        }
        else
        {
            throw new System.Exception("Trying to remove a unit from a stock already empty : " + ut);
        }
    }

    /// <summary>
    /// Returns whether the player has at least 1 unit of the given unit type.
    /// </summary>
    /// <param name="ut"></param>
    /// <returns></returns>
    public bool HasStockOf(UnitType ut)
    {
        return unitsStock[ut] > 0;
    }

    /// <summary>
    /// Returns a random available (got stock of it) unit.
    /// </summary>
    /// <returns></returns>
    public UnitType GetRandomAvailableUnit()
    {
        // Get the available units
        List<UnitType> availableUnits = new List<UnitType>();
        for (int i = 0; i < 5; i++)
        {
            if (HasStockOf((UnitType)i))
            {
                availableUnits.Add((UnitType)i);
            }
        }

        // return a random unit from available ones
        return availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
    }
}
