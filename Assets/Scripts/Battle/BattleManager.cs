using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [SerializeField]
    private GameObject unitsRecapPanel;

    // score
    private int[] scoreDefinitionTable;
    [Header("Score")]
    [SerializeField]
    private GameObject scoreAndRoundUI;
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
    [SerializeField]
    private GameObject postBattleScreen;


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
    private TimerRenderer timerRenderer;

    [Header("Units Fight")]
    [SerializeField]
    private UnitsFightManager unitsFightManager;

    [Header("Plays History UI")]

    [SerializeField]
    private UIPlaysHistoryHandler uiPlaysHistoryHandler;

    [Header("Reactions")]
    [SerializeField]
    private GameObject enemyDialogPanel;
    [SerializeField]
    private TextMeshProUGUI enemyDialogText;
    [SerializeField]
    private Image enemySeal;
    [SerializeField]
    private TextMeshProUGUI enemyNameNearSealText;
    [SerializeField]
    private AudioSource enemyDialogCloseAS;
    [SerializeField]
    private AudioClip[] enemyDialogCloseClips;

    // Animators
    private Animator enemyDialogAnimator;
    private Animator unitsPickPopupAnimator;
    private Animator playsHistoryAnimator;

    // Player units pick
    private bool playerAllowedToPick;
    private bool playerHasPicked;

    // Picked units
    private UnitType playerPickedUnit;
    private UnitType aiPickedUnit;

    // Timer logic & display
    private float pickTimer;
    private float remainingTime;

    public BattleCommander BattleWinner { get; set; }
    public bool UnlockedRewards { get; set; }

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
        BattleWinner = null;
        UnlockedRewards = false;
        enemyDialogAnimator = this.enemyDialogPanel.GetComponent<Animator>();
        unitsPickPopupAnimator = this.unitPickPopup.GetComponent<Animator>();
        playsHistoryAnimator = this.uiPlaysHistoryHandler.gameObject.GetComponent<Animator>();

        // Init Player
        PlayerBC = (BattleCommander)playerPR.gameObject.AddComponent(typeof(BattleCommander));
        PlayerBC.Init(GameManager.Instance.Player, false);
        playerPR.RenderPortrait(PlayerBC.Commander);

        // Init Enemy 
        EnemyBC = (BattleCommander)enemyPR.gameObject.AddComponent(typeof(BattleCommander));
        EnemyBC.Init(GameManager.Instance.BattledCommander, Array.IndexOf(GameManager.Instance.Enemies, GameManager.Instance.BattledCommander) == 0);
        enemyPR.RenderPortrait(EnemyBC.Commander);
        enemySeal.color = EnemyBC.Commander.Color;
        enemyNameNearSealText.text = EnemyBC.Commander.CommanderName;

        // Init score slider
        InitNonCommanderUI();

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
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(DisplayPreBattleLine());
        // Display battle UI
        uiPlaysHistoryHandler.gameObject.SetActive(true);
        scoreAndRoundUI.SetActive(true);
        unitsRecapPanel.SetActive(true);

        // Start the battle
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
            timerRenderer.StartRenderingTimer(remainingTime);
            yield return new WaitUntil(() => remainingTime == 0f);
            this.unitsPickPopupAnimator.SetBool("Opened", false);
            yield return new WaitForSeconds(this.unitsPickPopupAnimator.runtimeAnimatorController.animationClips[0].length);
            unitPickPopup.SetActive(false);
            timerRenderer.StopRenderingTimer();

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

            // Units fight
            playsHistoryAnimator.SetBool("Opened", false);
            yield return new WaitForSeconds(this.playsHistoryAnimator.runtimeAnimatorController.animationClips[0].length);
            uiPlaysHistoryHandler.gameObject.SetActive(false);

            BattleCommander winner = GetWinnerFromUnitsFight();
            unitsRecapPanel.SetActive(false);

            // Play fight animation instead
            this.unitsFightManager.StartUnitsFight(playerPickedUnit, aiPickedUnit);
            yield return new WaitUntil(() => this.unitsFightManager.FightIsOver);

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

            // AI speaks after units fight if it's not the final round + facial reaction
            if (CurrentRound != MAX_ROUNDS)
            {
                SetMoods(winner);
                yield return StartCoroutine(DisplayPostUnitsFightLine(winner));
                ResetMoods();
            }

            // update plays history UI
            uiPlaysHistoryHandler.gameObject.SetActive(true);
            uiPlaysHistoryHandler.RenderPlaysHistoryUI(playerPickedUnit, aiPickedUnit, winner);
            unitsRecapPanel.SetActive(true);

            CurrentRound++;
        }

        // Handle winner of the battle, update his stats
        BattleWinner = GetAndHandleWinner();
        SetMoods(BattleWinner);
        yield return StartCoroutine(DisplayPostBattleLine());

        // display end battle panel that will be handled by attached script
        postBattleScreen.SetActive(true);
    }

    #region Reaction Moods
    /// <summary>
    /// Resets commanders moods to neutral.
    /// </summary>
    private void ResetMoods()
    {
        enemyPR.RenderMood(EnemyBC.Commander, PortraitMood.Neutral);
        playerPR.RenderMood(PlayerBC.Commander, PortraitMood.Neutral);
    }

    /// <summary>
    /// Sets commanders moods depending on the result.
    /// </summary>
    /// <param name="enemyWon"></param>
    /// <param name="displayMood"></param>
    private void SetMoods(BattleCommander winner)
    {
        if (winner == EnemyBC) // AI won
        {
            enemyPR.RenderMood(EnemyBC.Commander, PortraitMood.Proud);
            playerPR.RenderMood(PlayerBC.Commander, PortraitMood.Angry);
        }
        else // Player won
        {
            enemyPR.RenderMood(EnemyBC.Commander, PortraitMood.Angry);
            playerPR.RenderMood(PlayerBC.Commander, PortraitMood.Proud);
        }
    }
    #endregion

    #region Dialogs
    /// <summary>
    /// Displays a pre battle line (at the beginning of the battle).
    /// </summary>
    private IEnumerator DisplayPreBattleLine()
    {
        // Commandant jamais affronté
        if (EnemyBC.Commander.LossesCount == 0 && EnemyBC.Commander.WinsCount == 0)
        {
            // display first fight line
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.preBattleFirstTimeLine));
        }
        else if (EnemyBC.Commander.WonLastFightAgainstPlayer) // commander won last fight
        {
            // display won last line
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPreBattleWonLastLine()));
        }
        else // commander lost last fight
        {
            // display lost last line
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPreBattleLostLastLine()));
        }
    }

    /// <summary>
    /// Displays a post battle line (at the end of the battle).
    /// </summary>
    private IEnumerator DisplayPostBattleLine()
    {
        // Victoire de l'IA
        if (BattleWinner == EnemyBC)
        {
            // display random post battle line for AI win
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPostBattleWinLine()));
        }
        else
        {
            //display random post battle line for AI loss
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPostBattleLossLine()));
        }
    }

    /// <summary>
    /// Displays a line after the units have fought and a winner is visible.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayPostUnitsFightLine(BattleCommander w)
    {
        // print specific msgs for tutorial
        if (EnemyBC.IsTutorial)
        {
            yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetNextTutorialLine()));
        }
        else // non tutorial battle
        {
            // ai wins
            if (w == EnemyBC)
            {
                // display random post units fight line for AI win
                yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPostUnitsFightWinLine()));
            }
            else if (w == PlayerBC) // ai loses
            {
                // display random post units fight line for AI loss
                yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPostUnitsFightLossLine()));
            }
            else // draw
            {
                // display random post units fight line for draw result
                yield return StartCoroutine(DisplayReactionLine(EnemyBC.Commander.Dialogs.GetRandomPostUnitsFightDrawLine()));
            }
        }
    }
    #endregion

    /// <summary>
    /// Displays given line on screen for the given amount of seconds.
    /// </summary>
    private IEnumerator DisplayReactionLine(string line)
    {
        enemyDialogText.text = line;
        enemyDialogPanel.SetActive(true);

        // play open msg SFX
        yield return new WaitUntil(() => !this.enemyDialogAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.enemyDialogAnimator.runtimeAnimatorController.animationClips[0].length);

        // play close msg SFX
        enemyDialogCloseAS.clip = this.enemyDialogCloseClips[Random.Range(0, this.enemyDialogCloseClips.Length)];
        enemyDialogPanel.SetActive(false);
    }

    /// <summary>
    /// Trigger this from click on msg panel. Hides panel and continues the battle.
    /// </summary>
    public void SkipReactionLine()
    {
        enemyDialogAnimator.SetBool("Opened", false);
    }

    /// <summary>
    /// Leave battle before it's actually over.
    /// </summary>
    public void Surrender()
    {
        EnemyBC.Commander.LossesCount++;
        EnemyBC.Commander.WonLastFightAgainstPlayer = true;
        EnemyBC.Commander.SaveStats();
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Inits mainly the colors of the parts of the slide to match the player's.
    /// </summary>
    private void InitNonCommanderUI()
    {
        this.scoreSlider.value = .5f;
        this.scoreSliderPlayerImg.color = PlayerBC.Commander.Color;
        this.scoreSliderEnemyImg.color = EnemyBC.Commander.Color;
        this.playerScoreText.text = PlayerBC.Score.ToString();
        this.enemyScoreText.text = EnemyBC.Score.ToString();

        this.uiPlaysHistoryHandler.gameObject.SetActive(false);

        // hide it at the beginning
        scoreAndRoundUI.SetActive(false);
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
        UpdateUnitBtn(knightsBtn, UnitType.Knights);
        UpdateUnitBtn(shieldsBtn, UnitType.Shields);
        UpdateUnitBtn(spearmenBtn, UnitType.Spearmen);
        UpdateUnitBtn(magesBtn, UnitType.Mages);
        UpdateUnitBtn(archersBtn, UnitType.Archers);
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
        Color selectedColor = new Color(129f / 255f, 206f / 255f, 1f);
        switch (ut)
        {
            case UnitType.Knights:
                knightsBtn.gameObject.GetComponent<Image>().color = selectedColor;
                break;

            case UnitType.Shields:
                shieldsBtn.gameObject.GetComponent<Image>().color = selectedColor;
                break;

            case UnitType.Spearmen:
                spearmenBtn.gameObject.GetComponent<Image>().color = selectedColor;
                break;

            case UnitType.Mages:
                magesBtn.gameObject.GetComponent<Image>().color = selectedColor;
                break;

            case UnitType.Archers:
                archersBtn.gameObject.GetComponent<Image>().color = selectedColor;
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
    private void UpdateUnitBtn(Button btn, UnitType ut)
    {
        if (PlayerBC.Army.unitsStock[ut] == 0)
        {
            btn.interactable = false;
        }
        btn.GetComponentInChildren<TextMeshProUGUI>().text = PlayerBC.Army.unitsStock[ut].ToString();
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
    private BattleCommander GetAndHandleWinner()
    {
        if (PlayerBC.Score > EnemyBC.Score)
        {
            Debug.Log("Player won the battle.");
            // Unlock next commander, if first time we beat this AI
            if (EnemyBC.Commander.WinsCount == 0)
            {
                GameManager.Instance.UnlockRewards();
                this.UnlockedRewards = true;
            }
            // Update wins against this commander
            EnemyBC.Commander.WinsCount++;
            EnemyBC.Commander.WonLastFightAgainstPlayer = false;
            EnemyBC.Commander.SaveStats();

            return PlayerBC;
        }
        else
        {
            Debug.Log("Player lost or drew the battle.");
            EnemyBC.Commander.LossesCount++;
            EnemyBC.Commander.WonLastFightAgainstPlayer = true;
            EnemyBC.Commander.SaveStats();

            return EnemyBC;
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
                ai = new AIThrowback(this);
                break;

            case AIType.SmartHuman:
                ai = new AISmartHuman(this);
                break;

            default:
                throw new Exception("Unknown AI!");
        }
    }

}
