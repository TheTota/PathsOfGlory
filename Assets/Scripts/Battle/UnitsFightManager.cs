using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the course of a units fight : instanciates the battling units, 
/// handle the life of these units and the end of the fight.
/// </summary>
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

    [SerializeField]
    private Transform playerUnitsParent, enemyUnitsParent;

    public bool FightIsOver { get; internal set; }

    // store the units on the battlefield
    public List<UnitAI> playerUnitAIs { get; set; }
    public List<UnitAI> enemyUnitAIs { get; set; }

    private Camera mainCam;

    // Start is called before the first frame update
    void Awake()
    {
        this.mainCam = Camera.main;
    }

    /// <summary>
    /// Starts a fight between 2 given units.
    /// </summary>
    /// <param name="playerUnit"></param>
    /// <param name="enemyUnit"></param>
    public void StartUnitsFight(UnitType playerUnit, UnitType enemyUnit)
    {
        this.FightIsOver = false;

        // init vars
        int unitsAmount = 6;
        float yInc = this.maxBoundY - ((this.maxBoundY + this.minBoundY) / 2f);
        float yIncMultiplicator = 0f;
        this.playerUnitAIs = new List<UnitAI>();
        this.enemyUnitAIs = new List<UnitAI>();

        float xSpacer = .3f;

        // Instantiate player units
        for (int i = 0; i < unitsAmount; i++)
        {
            if (i != 0 && i % 2 == 0) // si i est différent de 0 et paire, on incrémente la val pour la pos sur Y 
            {
                yIncMultiplicator++;
                xSpacer = .3f;
            }

            this.playerUnitAIs.Add(InstantiateUnit(this.bm.PlayerBC, playerUnit, yInc, yIncMultiplicator, -170f, xSpacer, playerUnitsParent));
            this.enemyUnitAIs.Add(InstantiateUnit(this.bm.EnemyBC, enemyUnit, yInc, yIncMultiplicator, Screen.width + 170f, xSpacer, enemyUnitsParent));
            xSpacer = 1.6f;
        }

        AttributeTargets();
    }

    /// <summary>
    /// Instantiates a unit and fills some of its attributes.
    /// </summary>
    private UnitAI InstantiateUnit(BattleCommander bc, UnitType ut, float yInc, float yIncMultiplicator, float xOffset, float xSpacer, Transform parent)
    {
        // space knights more to make it look better
        float minY = this.minBoundY;
        if (ut == UnitType.Knights)
        {
            xSpacer *= 1.7f;
            yInc += .1f;
            minY += .1f;
        }

        // make xSpace negative if units come from the left (allows both units to come from same position out of screen)
        if (xOffset < 0)
        {
            xSpacer *= -1;
        }

        // determine position
        Vector3 instPos = new Vector3(this.mainCam.ScreenToWorldPoint(new Vector3(xOffset, 0f)).x + xSpacer, minY + yInc * yIncMultiplicator, 1f);

        // instantiate sprite
        GameObject instUnit = Instantiate(GetPrefabFromUnitType(ut), instPos, Quaternion.identity, parent);

        // turn sprite for the player
        if (bc == this.bm.PlayerBC)
        {
            instUnit.transform.localScale = new Vector3(-instUnit.transform.localScale.x, instUnit.transform.localScale.y, instUnit.transform.localScale.z);
        }
        else // reverse order in hierarchy if enemy
        {
            instUnit.transform.SetAsLastSibling();
        }

        // color the sprite
        instUnit.transform.Find("Body").GetComponent<SpriteRenderer>().color = bc.Commander.Color;

        // init the ai script 
        instUnit.GetComponent<UnitAI>().Init(this, ut);

        // return the instantiated GO
        return instUnit.GetComponent<UnitAI>();
    }

    /// <summary>
    /// Attributes targets to every unit everywhere!
    /// </summary>
    private void AttributeTargets()
    {
        for (int i = 0; i < this.playerUnitAIs.Count; i++)
        {
            if (i % 2 == 0)
            {
                this.playerUnitAIs[i].SetTarget(this.enemyUnitAIs[i + 1]);
                this.enemyUnitAIs[i].SetTarget(this.playerUnitAIs[i + 1]);
            }
            else
            {
                this.playerUnitAIs[i].SetTarget(this.enemyUnitAIs[i]);
                this.enemyUnitAIs[i].SetTarget(this.playerUnitAIs[i]);
            }
        }
    }

    /// <summary>
    /// Called on the death of a unit. Removes it from the lists of alive units and checks if 
    /// the fight is over (0 units left in one of the parties).
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitFromList(UnitAI unit)
    {
        // remove unit from list
        if (this.playerUnitAIs.Contains(unit))
        {
            this.playerUnitAIs.Remove(unit);
        }
        else if (this.enemyUnitAIs.Contains(unit))
        {
            this.enemyUnitAIs.Remove(unit);
        }

        // check if battle is over
        if (this.playerUnitAIs.Count == 0 || this.enemyUnitAIs.Count == 0)
        {
            StartCoroutine(EndFightAfterDelay(1f));
        }
    }

    /// <summary>
    /// Ends the fight after a delay. Clears the battlefield.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private IEnumerator EndFightAfterDelay(float s)
    {
        yield return new WaitForSeconds(s);
        this.FightIsOver = true;

        // destroy every child to end the fight
        foreach (Transform child in this.playerUnitsParent)
            Destroy(child.gameObject);
        foreach (Transform child in this.enemyUnitsParent)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Returns a prefab associated to a unit type.
    /// </summary>
    /// <param name="ut"></param>
    /// <returns></returns>
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
