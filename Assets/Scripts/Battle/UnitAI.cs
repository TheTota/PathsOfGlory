using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Mother class for the AI of a unit used during a units fight, multiple times during a battle.
/// </summary>
public class UnitAI : MonoBehaviour
{
    // useful infos
    protected UnitsFightManager unitsFightMgr;
    protected UnitType ut;
    protected UnitAI target;

    // units movement and hop effect (better looking than linear movement)
    protected float speed;
    protected float hopFrequency = 30f;
    protected float hopMagnitude = .01f;

    // units move speed constants
    private const float SPEARMAN_SPEED = 1f;
    private const float SHIELD_SPEED = 1f;
    private const float KNIGHT_SPEED = 5f;
    private const float MAGE_SPEED = 3f;
    private const float ARCHER_SPEED = 3f;

    /// <summary>
    /// Set speed with constant value depending on the underlying unit.
    /// </summary>
    private void Awake()
    {
        if (ut == UnitType.Knights)
        {
            speed = KNIGHT_SPEED;
        }
        else if (ut == UnitType.Spearmen)
        {
            speed = SPEARMAN_SPEED;
        }
        else if (ut == UnitType.Shields)
        {
            speed = SHIELD_SPEED;
        }
        else if (ut == UnitType.Mages)
        {
            speed = MAGE_SPEED;
        }
        else if (ut == UnitType.Archers)
        {
            speed = ARCHER_SPEED;
        }
    }

    /// <summary>
    /// Init some useful variables of the unit.
    /// </summary>
    public void Init(UnitsFightManager ufm, UnitType ut)
    {
        this.unitsFightMgr = ufm;
        this.ut = ut;
    }

    /// <summary>
    /// Assigns a target to this unit.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(UnitAI target) {
        this.target = target;
    }

    /// <summary>
    /// Kills this unit, properly.
    /// </summary>
    public void Die()
    {
        unitsFightMgr.RemoveUnitFromList(this);
        Destroy(this.gameObject);
    }
}
