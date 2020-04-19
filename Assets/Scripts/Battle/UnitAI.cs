using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitAI : MonoBehaviour
{
    protected UnitsFightManager unitsFightMgr;
    protected UnitType ut;
    protected UnitAI target;

    protected float speed;
    protected float hopFrequency = 30f;
    protected float hopMagnitude = .01f;

    private const float SPEARMAN_SPEED = 1f;
    private const float SHIELD_SPEED = 1f;
    private const float KNIGHT_SPEED = 5f;
    private const float MAGE_SPEED = 3f;
    private const float ARCHER_SPEED = 3f;

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

    public void Init(UnitsFightManager ufm, UnitType ut)
    {
        this.unitsFightMgr = ufm;
        this.ut = ut;
    }

    public void SetTarget(UnitAI target) {
        this.target = target;
    }

    public void Die()
    {
        unitsFightMgr.RemoveUnitFromList(this);
        Destroy(this.gameObject);
    }
}
