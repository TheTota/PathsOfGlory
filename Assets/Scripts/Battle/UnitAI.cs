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
    protected UnitAI targetEnemy;
    protected Vector3 targetPos;

    // units movement and hop effect (better looking than linear movement)
    protected float speed;
    protected float hopFrequency = 30f;
    protected float hopMagnitude = .01f;

    // units move speed constants
    private const float SPEARMAN_SPEED = 2f;
    private const float SHIELD_SPEED = 2f;
    private const float KNIGHT_SPEED = 5f;
    private const float MAGE_SPEED = 3f;
    private const float ARCHER_SPEED = 3f;

    public bool DemoMode { get; internal set; }

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
    public void Init(UnitsFightManager ufm, UnitType ut, bool isDemoMode)
    {
        this.unitsFightMgr = ufm;
        this.ut = ut;

        // demo mode for intro screen
        this.DemoMode = isDemoMode;
        if (isDemoMode)
        {
            this.speed /= 2f;
            this.hopFrequency /= 1.5f;
        }
    }
    public void SetTargetEnemy(UnitAI target)
    {
        this.targetEnemy = target;
    }

    /// <summary>
    /// Assigns a target to this unit.
    /// </summary>
    /// <param name="target"></param>
    public void SetTargetPos(Vector3 target) {
        this.targetPos = target;
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
