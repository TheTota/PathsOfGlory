using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitsFightManager : MonoBehaviour
{
    [SerializeField]
    private BattleManager bm;

    [Header("Units Instanciation")]
    [SerializeField]
    private GameObject archerPrefab;
    [SerializeField]
    private GameObject knightPrefab;
    [SerializeField]
    private GameObject magePrefab;
    [SerializeField]
    private GameObject shieldPrefab;
    [SerializeField]
    private GameObject spearmanPrefab;

    [SerializeField]
    private float minBoundY, maxBoundY;

    public bool FightIsOver { get; internal set; }

    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        this.mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartUnitsFight(UnitType playerUnit, UnitType enemyUnit)
    {
        this.FightIsOver = false;

        // init vars
        int unitsAmount = 6;
        float yInc = this.maxBoundY - ((this.maxBoundY + this.minBoundY) / 2f);
        float yIncMultiplicator = 0f;

        float xSpacer = Random.Range(0.2f, 0.5f);

        // Instantiate player units
        for (int i = 0; i < unitsAmount; i++)
        {
            if (i != 0 && i % 2 == 0) // si i est différent de 0 et paire, on incrémente la val pour la pos sur Y 
            {
                yIncMultiplicator++;
                xSpacer = Random.Range(0.2f, 0.5f);
            }

            InstantiateUnit(this.bm.PlayerBC, playerUnit, yInc, yIncMultiplicator, -350f, xSpacer);
            InstantiateUnit(this.bm.EnemyBC, enemyUnit, yInc, yIncMultiplicator, Screen.width + 200f, xSpacer);
            xSpacer = Random.Range(1.5f, 1.75f);
        }
    }

    private void InstantiateUnit(BattleCommander bc, UnitType ut, float yInc, float yIncMultiplicator, float xOffset, float xSpacer)
    {
        // determine position
        Vector3 instPos = new Vector3(this.mainCam.ScreenToWorldPoint(new Vector3(xOffset, 0f)).x + xSpacer, this.minBoundY + yInc * yIncMultiplicator, 1f);

        // instantiate sprite
        GameObject instUnit = Instantiate(GetPrefabFromUnitType(ut), instPos, Quaternion.identity, this.gameObject.transform);

        // turn sprite for the player
        if (bc == this.bm.PlayerBC)
        {
            instUnit.transform.localScale = new Vector3(-instUnit.transform.localScale.x, instUnit.transform.localScale.y, instUnit.transform.localScale.z);
        }

        // color the sprite
        instUnit.transform.Find("Body").GetComponent<SpriteRenderer>().color = bc.Commander.Color;
    }

    private GameObject GetPrefabFromUnitType(UnitType ut)
    {
        switch (ut)
        {
            case UnitType.Knights:
                return this.knightPrefab;
            case UnitType.Shields:
                return this.shieldPrefab;
            case UnitType.Spearmen:
                return this.spearmanPrefab;
            case UnitType.Mages:
                return this.magePrefab;
            case UnitType.Archers:
                return this.archerPrefab;
            default:
                throw new Exception("error");
        }
    }
}
