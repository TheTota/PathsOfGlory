﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    protected UnitsFightManager unitsFightMgr;
    protected UnitType ut;
    protected UnitAI target;

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
        // TODO: set sprite(s) to dead
        // TODO: change rotation of gameobject?
        unitsFightMgr.RemoveUnitFromList(this);
        Destroy(this.gameObject);
    }
}
