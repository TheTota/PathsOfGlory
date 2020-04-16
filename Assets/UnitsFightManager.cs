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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartUnitsFight(UnitType playerUnit, UnitType enemyUnit)
    {
        this.FightIsOver = false;

        // Instantiate player units
        int unitsAmount = Random.Range(3, 5);
        for (int i = 0; i < unitsAmount; i++)
        {
            // instantiate sprite
            Vector3 instPos = new Vector3(-2f, Random.Range(this.minBoundY, this.maxBoundY), 1f);
            GameObject instUnit = Instantiate(GetPrefabFromUnitType(playerUnit), instPos, Quaternion.identity, this.gameObject.transform);
            // turn sprite around for the player
            instUnit.transform.localScale = new Vector3(-instUnit.transform.localScale.x, instUnit.transform.localScale.y, instUnit.transform.localScale.z);
            // color the sprite
            instUnit.transform.Find("Body").GetComponent<SpriteRenderer>().color = this.bm.PlayerBC.Commander.Color;
        }
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
