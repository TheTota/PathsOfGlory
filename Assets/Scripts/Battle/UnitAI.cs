using System.Collections;
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
        unitsFightMgr.RemoveUnitFromList(this);
        Destroy(this.gameObject);
    }
}
