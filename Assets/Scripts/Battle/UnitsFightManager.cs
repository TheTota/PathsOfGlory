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

    [Header("Global Battle Sounds")]
    [SerializeField]
    private AudioSource mainMusic;
    [SerializeField]
    private AudioSource fightDrums;

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

    [SerializeField]
    private bool introMode;

    public bool FightIsOver { get; internal set; }

    // store the units on the battlefield
    public List<UnitAI> playerUnitAIs { get; set; }
    public List<UnitAI> enemyUnitAIs { get; set; }

    private Camera mainCam;

    private float lowMainMusicVol;
    private float normalMainMusicVol;
    private Coroutine fightEndCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        this.mainCam = Camera.main;
        if (introMode)
        {
            StartCoroutine(StartIntroMode());
        }
        else // if not in intro, set min and max volumes for music, used to switch it up during unit fights
        {
            normalMainMusicVol = mainMusic.volume;
            lowMainMusicVol = mainMusic.volume / 2f;
        }
    }

    private IEnumerator StartIntroMode()
    {
        int i = Random.Range(0, 5);
        while (true)
        {
            InstanciateUnitsForIntro((UnitType)i);
            yield return new WaitForSeconds(7f);

            // go to next unit
            if (i + 1 >= 5)
            {
                i = 0;
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    /// Instancie des unités pour l'intro (côté joueur seulement).
    /// </summary>
    /// <param name="unitTypeToSpawn"></param>
    private void InstanciateUnitsForIntro(UnitType unitTypeToSpawn)
    {
        // init vars
        int unitsAmount = 18;
        int unitsPerColumn = 3;
        float yInc = this.maxBoundY - ((this.maxBoundY + this.minBoundY) / 2f);
        float yIncMultiplicator = 0f;
        this.playerUnitAIs = new List<UnitAI>();
        this.enemyUnitAIs = new List<UnitAI>();

        float xSpacer = .3f;

        // Instantiate player units : row per row
        for (int i = 0; i < unitsAmount; i++)
        {
            // reached limit of unit per row, go down
            if (i != 0 && i % (unitsAmount / unitsPerColumn) == 0)
            {
                yIncMultiplicator++;
                xSpacer = .3f;
            }

            this.playerUnitAIs.Add(InstantiateUnit(true, true, GameManager.Instance.Player.Color, unitTypeToSpawn, yInc, yIncMultiplicator, -170f, xSpacer, playerUnitsParent));
            this.playerUnitAIs[i].SetTargetPos(new Vector3(mainCam.ScreenToWorldPoint(new Vector3(Screen.width + 500f, 0f, 0f)).x, this.playerUnitAIs[i].transform.position.y, 1f));

            xSpacer += 1.3f;
        }
    }

    /// <summary>
    /// Starts a fight between 2 given units.
    /// </summary>
    /// <param name="playerUnit"></param>
    /// <param name="enemyUnit"></param>
    public void StartUnitsFight(UnitType playerUnit, UnitType enemyUnit)
    {
        this.FightIsOver = false;
        this.fightEndCoroutine = StartCoroutine(EndFightAfterDelay(10f));

        SwitchBattleSoundsMode();

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

            this.playerUnitAIs.Add(InstantiateUnit(true, false, this.bm.PlayerBC.Commander.Color, playerUnit, yInc, yIncMultiplicator, -170f, xSpacer, playerUnitsParent));
            this.enemyUnitAIs.Add(InstantiateUnit(false, false, this.bm.EnemyBC.Commander.Color, enemyUnit, yInc, yIncMultiplicator, Screen.width + 170f, xSpacer, enemyUnitsParent));
            xSpacer = 1.6f;
        }

        AttributeTargets();
    }

    /// <summary>
    /// Handles global sounds switching when units fights starts and ends.
    /// </summary>
    /// <param name="fightActive"></param>
    private void SwitchBattleSoundsMode()
    {
        // fight starts
        if (!this.FightIsOver)
        {
            this.mainMusic.volume = this.lowMainMusicVol;
            this.fightDrums.Play();
        }
        else // fight ends
        {
            this.mainMusic.volume = this.normalMainMusicVol;
            this.fightDrums.Stop();
        }
    }

    /// <summary>
    /// Instantiates a unit and fills some of its attributes.
    /// </summary>
    private UnitAI InstantiateUnit(bool isPlayer, bool isDemoMode, Color unitColor, UnitType ut, float yInc, float yIncMultiplicator, float xOffset, float xSpacer, Transform parent)
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
        if (isPlayer)
        {
            instUnit.transform.localScale = new Vector3(-instUnit.transform.localScale.x, instUnit.transform.localScale.y, instUnit.transform.localScale.z);
        }
        else // reverse order in hierarchy if enemy
        {
            instUnit.transform.SetAsLastSibling();
        }

        // color the sprite
        instUnit.transform.Find("GFX").Find("Body").GetComponent<SpriteRenderer>().color = unitColor;

        // init the ai script 
        instUnit.GetComponent<UnitAI>().Init(this, ut, isDemoMode);

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
                this.playerUnitAIs[i].SetTargetEnemy(this.enemyUnitAIs[i + 1]);
                this.enemyUnitAIs[i].SetTargetEnemy(this.playerUnitAIs[i + 1]);
            }
            else
            {
                this.playerUnitAIs[i].SetTargetEnemy(this.enemyUnitAIs[i]);
                this.enemyUnitAIs[i].SetTargetEnemy(this.playerUnitAIs[i]);
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
            StopCoroutine(this.fightEndCoroutine);
            StartCoroutine(EndFightAfterDelay(2f));
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
        SwitchBattleSoundsMode();

        ClearField();
    }

    /// <summary>
    /// Destroys every unit on the field.
    /// </summary>
    private void ClearField()
    {
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
