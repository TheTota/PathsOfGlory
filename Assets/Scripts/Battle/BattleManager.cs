using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    [SerializeField]
    private TextMeshProUGUI playerScoreText;
    [SerializeField]
    private TextMeshProUGUI enemyScoreText;
    [SerializeField]
    private TextMeshProUGUI nextRoundValueText;


    // unit pick popup 
    [Header("Unit Pick Popup")]
    [SerializeField]
    private EventSystem eventSystem;
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

    [Header("Units Fight")]
    [SerializeField]
    private TextMeshProUGUI playerUnitText;
    [SerializeField]
    private TextMeshProUGUI enemyUnitText;

    // Player units pick
    private bool playerAllowedToPick;
    private bool playerHasPicked;

    // Picked units
    private UnitType playerPickedUnit;
    private UnitType aiPickedUnit;

    // Timer logic & display
    private float pickTimer;
    private float remainingTime;

    /// <summary>
    /// Gives the BC that won the round n at the index n-1.
    /// </summary>
    public List<BattleCommander> RoundsWinnersHistory { get; set; }

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
            timerText.text = remainingTime.ToString("0.#0") + "s";
        }
    }

    /// <summary>
    /// Let the player pick a unit with the keyboard.
    /// </summary>
    private void PlayerKeyboardPick()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ExecuteEvents.Execute(knightsBtn.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ExecuteEvents.Execute(shieldsBtn.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteEvents.Execute(spearmenBtn.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ExecuteEvents.Execute(magesBtn.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            ExecuteEvents.Execute(archersBtn.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
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
        ColorSelectedBtn(ut);
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
        RoundsWinnersHistory = new List<BattleCommander>();

        // Init Player
        PlayerBC = (BattleCommander)playerPR.gameObject.AddComponent(typeof(BattleCommander));
        PlayerBC.Init(GameManager.Instance.Player);
        playerPR.RenderPortrait(PlayerBC.Commander);

        // Init Enemy 
        EnemyBC = (BattleCommander)enemyPR.gameObject.AddComponent(typeof(BattleCommander));
        EnemyBC.Init(GameManager.Instance.BattledCommander);
        enemyPR.RenderPortrait(EnemyBC.Commander);

        // Init score slider
        InitScoreUI();

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
        while (CurrentRound <= MAX_ROUNDS)
        {
            roundText.text = "Manche : " + CurrentRound + " / " + MAX_ROUNDS;
            nextRoundValueText.text = "Valeur de la manche : " + scoreDefinitionTable[CurrentRound - 1];

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
            PlayerBC.AddToPlaysHistory(playerPickedUnit);
            PlayerBC.Army.RemoveUnitFromStock(playerPickedUnit);

            // Make AI Pick
            aiPickedUnit = ai.PickUnit();
            EnemyBC.AddToPlaysHistory(aiPickedUnit);
            EnemyBC.Army.RemoveUnitFromStock(aiPickedUnit);

            // Fight the units
            BattleCommander winner = GetWinnerFromUnitsFight();

            DisplayBattlingUnits(true); // TODO: play fight animation instead
            yield return new WaitForSeconds(2f);
            DisplayBattlingUnits(false);

            // Update score
            if (winner)
            {
                winner.Score += scoreDefinitionTable[CurrentRound - 1];
                RoundsWinnersHistory.Add(winner);
                UpdateScoreUI();
            }
            else
            {
                RoundsWinnersHistory.Add(null);
            }

            CurrentRound++;
        }

        HandleWinner();
    }

    /// <summary>
    /// Display the battling units temporarly by just showing a text with the name of the units called into the battle.
    /// </summary>
    /// <param name="v"></param>
    private void DisplayBattlingUnits(bool v)
    {
        if (v)
        {
            this.playerUnitText.text = playerPickedUnit.ToString();
            this.enemyUnitText.text = aiPickedUnit.ToString();
        }
        else
        {
            this.playerUnitText.text = "";
            this.enemyUnitText.text = "";
        }
    }

    /// <summary>
    /// Leave battle before it's actually over.
    /// </summary>
    public void Surrender()
    {
        EnemyBC.Commander.LossesCount++;
        ReturnToMainMenu();
    }

    /// <summary>
    /// Returns to main menu and saves the stats of the AI.
    /// </summary>
    private void ReturnToMainMenu()
    {
        EnemyBC.Commander.SaveStats();
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Inits mainly the colors of the parts of the slide to match the player's.
    /// </summary>
    private void InitScoreUI()
    {
        this.scoreSlider.value = .5f;
        this.scoreSliderPlayerImg.color = PlayerBC.Commander.Color;
        this.scoreSliderEnemyImg.color = EnemyBC.Commander.Color;
        this.playerScoreText.text = PlayerBC.Score.ToString();
        this.enemyScoreText.text = EnemyBC.Score.ToString();
    }

    /// <summary>
    /// Updates the score slider to represent the current score (called balance) of the battle.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (PlayerBC.Score == 0 && EnemyBC.Score == 0)
        {
            this.scoreSlider.value = .5f;
        }
        else
        {
            this.scoreSlider.value = (float)PlayerBC.Score / ((float)PlayerBC.Score + (float)EnemyBC.Score);
            this.playerScoreText.text = PlayerBC.Score.ToString();
            this.enemyScoreText.text = EnemyBC.Score.ToString();
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
        ResetUnitBtnsColor();

        // display it!
        this.unitPickPopup.SetActive(true);
    }

    /// <summary>
    /// Colors the selected btn and uncolors the other ones.
    /// </summary>
    /// <param name="ut"></param>
    private void ColorSelectedBtn(UnitType ut)
    {
        // reset btn colors
        ResetUnitBtnsColor();

        // highlight clicked one
        switch (ut)
        {
            case UnitType.Knights:
                knightsBtn.gameObject.GetComponent<Image>().color = PlayerBC.Commander.Color;
                break;

            case UnitType.Shields:
                shieldsBtn.gameObject.GetComponent<Image>().color = PlayerBC.Commander.Color;
                break;

            case UnitType.Spearmen:
                spearmenBtn.gameObject.GetComponent<Image>().color = PlayerBC.Commander.Color;
                break;

            case UnitType.Mages:
                magesBtn.gameObject.GetComponent<Image>().color = PlayerBC.Commander.Color;
                break;

            case UnitType.Archers:
                archersBtn.gameObject.GetComponent<Image>().color = PlayerBC.Commander.Color;
                break;

            default:
                throw new Exception("Couldnt highlight this button, weird UT..");
        }
    }

    private void ResetUnitBtnsColor()
    {
        knightsBtn.gameObject.GetComponent<Image>().color = Color.white;
        shieldsBtn.gameObject.GetComponent<Image>().color = Color.white;
        spearmenBtn.gameObject.GetComponent<Image>().color = Color.white;
        magesBtn.gameObject.GetComponent<Image>().color = Color.white;
        archersBtn.gameObject.GetComponent<Image>().color = Color.white;
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
    private BattleCommander GetWinnerFromUnitsFight()
    {
        // DRAW
        if (playerPickedUnit == aiPickedUnit)
        {
            return null;
        }
        // PLAYER WINS
        else if ((playerPickedUnit == UnitType.Knights && (aiPickedUnit == UnitType.Archers || aiPickedUnit == UnitType.Mages))
         || (playerPickedUnit == UnitType.Archers && (aiPickedUnit == UnitType.Mages || aiPickedUnit == UnitType.Spearmen))
         || (playerPickedUnit == UnitType.Mages && (aiPickedUnit == UnitType.Spearmen || aiPickedUnit == UnitType.Shields))
         || (playerPickedUnit == UnitType.Spearmen && (aiPickedUnit == UnitType.Shields || aiPickedUnit == UnitType.Knights))
         || (playerPickedUnit == UnitType.Shields && (aiPickedUnit == UnitType.Knights || aiPickedUnit == UnitType.Archers)))
        {
            return PlayerBC;
        }
        // AI WINS
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
            // Unlock next commander, if first time we beat this AI
            if (EnemyBC.Commander.WinsCount == 0)
            {
                GameManager.Instance.UnlockNextCommander();
            }
            // Update wins against this commander
            EnemyBC.Commander.WinsCount++;
        }
        else
        {
            Debug.Log("Player lost or drew the battle.");
            EnemyBC.Commander.LossesCount++;
        }
        ReturnToMainMenu(); // TODO: Replace with battle recap
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
                ai = new AICounterClockwise(this);
                break;

            case AIType.DrunkResilient:
                ai = new AIDrunkResilient(this);
                break;

            case AIType.Drunk:
                ai = new AIDrunk(this);
                break;

            case AIType.SelfCounter:
                ai = new AISelfCounter(this);
                break;

            case AIType.PlayerCounter:
                ai = new AICounterPlayer(this);
                break;

            case AIType.CommonHuman:
                ai = new AICommonHuman(this);
                break;

            case AIType.PlayerStock:
                ai = new AIPlayerStock(this);
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
