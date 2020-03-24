using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inits and manages the course of a battle!
/// </summary>
public class BattleManager : MonoBehaviour
{
    // Player data
    [SerializeField]
    private PortraitRenderer playerPR;
    public BattleCommander PlayerBC { get; set; }

    // Enemy data
    [SerializeField]
    private PortraitRenderer enemyPR;
    public BattleCommander EnemyBC { get; set; }

    /// <summary>
    /// AI that player will face.
    /// </summary>
    public AI ai;

    // Rounds
    private const int MAX_ROUNDS = 15;
    private const int ROUNDS_BETWEEN_SCORE_INCR = 3;
    public int CurrentRound { get; set; }
    [SerializeField]
    private TextMeshProUGUI roundText;

    // score
    private int[] scoreDefinitionTable;
    [Header("Score Slider")]
    [SerializeField]
    private Slider scoreSlider;
    [SerializeField]
    private Image scoreSliderPlayerImg;
    [SerializeField]
    private Image scoreSliderEnemyImg;

    // unit pick popup 
    [SerializeField]
    private GameObject unitPickPopup;
    [SerializeField]
    private Button knightsBtn;
    [SerializeField]
    private Button shieldsBtn;
    [SerializeField]
    private Button spearmenBtn;
    [SerializeField]
    private Button magesBtn;
    [SerializeField]
    private Button archersBtn;
    [SerializeField]
    private TextMeshProUGUI timerText;

    // Player units pick
    private bool playerAllowedToPick;
    private UnitType playerPickedUnit;
    private bool playerHasPicked;

    private float pickTimer;
    private float remainingTime;

    // Start is called before the first frame update
    void Awake()
    {
        InitScoreDefinitionTable();
        InitBattle();
    }

    private void Update()
    {
        if (playerAllowedToPick)
        {
            PlayerKeyboardPick();
            
            remainingTime = Mathf.Clamp(pickTimer - Time.time, 0f, ai.SecondsBeforeAction);
            timerText.text = remainingTime.ToString("#.##") + "s";
        }
    }

    /// <summary>
    /// Let the player pick a unit with the keyboard.
    /// </summary>
    private void PlayerKeyboardPick()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPlayerPickedUnit(UnitType.Knights);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            SetPlayerPickedUnit(UnitType.Shields);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetPlayerPickedUnit(UnitType.Spearmen);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetPlayerPickedUnit(UnitType.Mages);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SetPlayerPickedUnit(UnitType.Archers);
        }
    }

    /// <summary>
    /// Sets the unit picked by the player.
    /// </summary>
    /// <param name="ut"></param>
    public void SetPlayerPickedUnit(UnitType ut)
    {
        if (PlayerBC.Army.HasStockOf(ut))
        {
            playerPickedUnit = ut;
            playerHasPicked = true;
        }
    }

    /// <summary>
    /// Inits the table that will allow score calculation during the battle.
    /// </summary>
    private void InitScoreDefinitionTable()
    {
        scoreDefinitionTable = new int[MAX_ROUNDS];
        int rs = 0;
        for (int i = 0; i < MAX_ROUNDS; i++)
        {
            if (i != 0 && i % ROUNDS_BETWEEN_SCORE_INCR == 0) // tous les 3 rounds, on incrémente le score
            {
                rs++;
            }
            scoreDefinitionTable[i] = rs;
        }
    }

    /// <summary>
    /// Initialise et démarre une bataille entre 2 commandants.
    /// </summary>
    private void InitBattle()
    {
        // misc inits
        playerAllowedToPick = false;
        CurrentRound = 1;

        // Init Player
        PlayerBC = (BattleCommander)playerPR.gameObject.AddComponent(typeof(BattleCommander));
        PlayerBC.Init(GameManager.Instance.Player);
        playerPR.RenderPortrait(PlayerBC.Commander);

        // Init Enemy 
        EnemyBC = (BattleCommander)enemyPR.gameObject.AddComponent(typeof(BattleCommander));
        EnemyBC.Init(GameManager.Instance.BattledCommander);
        enemyPR.RenderPortrait(EnemyBC.Commander);

        // Init score slider
        InitScoreSlider();

        // Init popup 
        InitUnitPickPopup();

        // Init AI for the battle
        InitAI(EnemyBC.Commander.AiType);

        // Start battle coroutine
        StartCoroutine(Battle());
    }

    /// <summary>
    /// Coroutine for the whole battle, to handle rounds etc.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Battle()
    {
        Debug.Log("The battle... begins!");
        while (CurrentRound <= MAX_ROUNDS)
        {
            roundText.text = "Manche : " + CurrentRound + " / " + MAX_ROUNDS;

            // Allow the player to pick
            pickTimer = Time.time + ai.SecondsBeforeAction;
            remainingTime = ai.SecondsBeforeAction;
            playerAllowedToPick = true;
            UpdateAndShowUnitPickPopup();

            // After some time, get the AI pick 
            yield return new WaitUntil(() => remainingTime == 0f);

            // Handle Player pick
            playerAllowedToPick = false;
            unitPickPopup.SetActive(false);
            if (!playerHasPicked)
            {
                Debug.Log("Player didn't pick or tried to pick something out of stock, sending random available unit");
                this.playerPickedUnit = PlayerBC.Army.GetRandomAvailableUnit();
            }
            playerHasPicked = false;
            PlayerBC.Army.RemoveUnitFromStock(playerPickedUnit);

            // Make AI Pick
            UnitType aiPick = ai.PickUnit();
            EnemyBC.Army.RemoveUnitFromStock(aiPick);

            // Fight the units
            BattleCommander winner = GetWinnerFromUnitsFight(aiPick);

            // TODO: play fight animation 
            yield return new WaitForSeconds(2f);

            // Update score
            winner.Score += scoreDefinitionTable[CurrentRound - 1];
            UpdateScoreSlider();

            CurrentRound++;
        }

        HandleWinner();
    }

    /// <summary>
    /// Inits mainly the colors of the parts of the slide to match the player's.
    /// </summary>
    private void InitScoreSlider()
    {
        this.scoreSlider.value = .5f;
        this.scoreSliderPlayerImg.color = PlayerBC.Commander.Color;
        this.scoreSliderEnemyImg.color = EnemyBC.Commander.Color;
    }

    /// <summary>
    /// Updates the score slider to represent the current score (called balance) of the battle.
    /// </summary>
    private void UpdateScoreSlider()
    {
        if (PlayerBC.Score == 0 && EnemyBC.Score == 0)
        {
            this.scoreSlider.value = .5f;
        }
        else
        {
            this.scoreSlider.value = (float)PlayerBC.Score / ((float)PlayerBC.Score + (float)EnemyBC.Score);
        }
    }

    /// <summary>
    /// Inits the units pick popup mostly by assigning actions on btns.
    /// </summary>
    private void InitUnitPickPopup()
    {
        unitPickPopup.SetActive(false);

        // init btns actions
        knightsBtn.onClick.AddListener(() => SetPlayerPickedUnit(UnitType.Knights));
        shieldsBtn.onClick.AddListener(() => SetPlayerPickedUnit(UnitType.Shields));
        spearmenBtn.onClick.AddListener(() => SetPlayerPickedUnit(UnitType.Spearmen));
        magesBtn.onClick.AddListener(() => SetPlayerPickedUnit(UnitType.Mages));
        archersBtn.onClick.AddListener(() => SetPlayerPickedUnit(UnitType.Archers));
    }

    /// <summary>
    /// Updates values in the popup and displays it 
    /// </summary>
    private void UpdateAndShowUnitPickPopup()
    {
        // update values
        UpdateUnitBtn(knightsBtn, UnitType.Knights, "Chevaliers");
        UpdateUnitBtn(shieldsBtn, UnitType.Shields, "Boucliers");
        UpdateUnitBtn(spearmenBtn, UnitType.Spearmen, "Lanciers");
        UpdateUnitBtn(magesBtn, UnitType.Mages, "Mages");
        UpdateUnitBtn(archersBtn, UnitType.Archers, "Archers");

        // display it!
        this.unitPickPopup.SetActive(true);
    }

    /// <summary>
    /// Updates a given button depending on the stock of the unit.
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="ut"></param>
    /// <param name="lbl"></param>
    private void UpdateUnitBtn(Button btn, UnitType ut, string lbl)
    {
        if (PlayerBC.Army.unitsStock[ut] == 0)
        {
            btn.interactable = false;
        }
        btn.GetComponentInChildren<TextMeshProUGUI>().text = lbl + "\n(" + PlayerBC.Army.unitsStock[ut] + ")";
    }


    /// <summary>
    /// Fights units and gets the winner of the fight.
    /// </summary>
    /// <param name="aiPick"></param>
    /// <returns></returns>
    private BattleCommander GetWinnerFromUnitsFight(UnitType aiPick)
    {
        Debug.Log("FIGHT : (PLAYER) " + playerPickedUnit + " vs " + aiPick + "(AI)");

        // Check who won the fight and return the BC
        if ((playerPickedUnit == UnitType.Knights && (aiPick == UnitType.Archers || aiPick == UnitType.Mages))
         || (playerPickedUnit == UnitType.Archers && (aiPick == UnitType.Mages || aiPick == UnitType.Spearmen))
         || (playerPickedUnit == UnitType.Mages && (aiPick == UnitType.Spearmen || aiPick == UnitType.Shields))
         || (playerPickedUnit == UnitType.Spearmen && (aiPick == UnitType.Shields || aiPick == UnitType.Knights))
         || (playerPickedUnit == UnitType.Shields && (aiPick == UnitType.Knights || aiPick == UnitType.Archers)))
        {
            return PlayerBC;
        }
        else
        {
            return EnemyBC;
        }
    }

    /// <summary>
    /// Handles winner based on BC scores.
    /// </summary>
    private void HandleWinner()
    {
        if (PlayerBC.Score > EnemyBC.Score)
        {
            Debug.Log("Player won the battle.");
            // Unlock next commander
            GameManager.Instance.UnlockNextCommander();
            // Update wins against this commander
            EnemyBC.Commander.WinsCount++;
        }
        else
        {
            Debug.Log("Player lost or drew the battle.");
            EnemyBC.Commander.LossesCount++;
        }
    }

    /// <summary>
    /// Inits the AI for the battle depending on the AIType of the enemy commander.
    /// </summary>
    /// <param name="aiType"></param>
    private void InitAI(AIType aiType)
    {
        switch (aiType)
        {
            case AIType.Clockwise:
                ai = new AIClockwise(this);
                break;

            case AIType.CounterClockwise:
                break;

            case AIType.Drunk:
                break;

            case AIType.DrunkResilient:
                break;

            case AIType.SelfCounter:
                break;

            case AIType.PlayerCounter:
                break;

            case AIType.CommonHuman:
                break;

            case AIType.PlayerStock:
                break;

            case AIType.Throwback:
                break;

            case AIType.SmartHuman:
                break;

            default:
                throw new Exception("Unknown AI!");
        }
    }

}
