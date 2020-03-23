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
        unitsStock.Add(UnitType.Knights, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Shields, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Spearmen, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Mages, INITIAL_STOCK_OF_EACH);
        unitsStock.Add(UnitType.Archers, INITIAL_STOCK_OF_EACH);
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
            throw new System.Exception("trying to remove a unit from a stock already empty");
        }
    }
}
