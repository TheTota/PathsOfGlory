using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    // Player units pick
    private bool playerAllowedToPick;
    private UnitType? playerPickedUnit;

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
        }
    }

    private void PlayerKeyboardPick()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerPickedUnit = UnitType.Knights;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            playerPickedUnit = UnitType.Shields;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            playerPickedUnit = UnitType.Spearmen;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            playerPickedUnit = UnitType.Mages;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            playerPickedUnit = UnitType.Archers;
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
            playerPickedUnit = null;
            roundText.text = "Manche : " + CurrentRound;

            // Allow the player to pick
            playerAllowedToPick = true;

            // After some time, get the AI pick 
            yield return new WaitForSeconds(ai.SecondsBeforeAction);
            playerAllowedToPick = false;
            UnitType aiPick = ai.PickUnit();

            // Fight the units
            BattleCommander winner = GetWinnerFromUnitsFight(aiPick);
            winner.Score += scoreDefinitionTable[CurrentRound - 1];

            Debug.Log("SCORES (PLAYER) " + PlayerBC.Score + " & " + EnemyBC.Score + "(AI)");

            CurrentRound++;
        }

        HandleWinner();
    }

    /// <summary>
    /// Fights units and gets the winner of the fight.
    /// </summary>
    /// <param name="aiPick"></param>
    /// <returns></returns>
    private BattleCommander GetWinnerFromUnitsFight(UnitType aiPick)
    {
        Debug.Log("FIGHT : (PLAYER) " + playerPickedUnit + " vs " + aiPick + "(AI)");
        if (playerPickedUnit != null)
        {
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
        else
        {
            Debug.Log("Player didn't pick, he loses");
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
        }
        else
        {
            Debug.Log("Player lost or drew the battle.");
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
