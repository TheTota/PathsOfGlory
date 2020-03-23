using System;
using System.Collections;
using System.Collections.Generic;
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

    // score
    public int[] scoreDefinitionTable;

    // Start is called before the first frame update
    void Awake()
    {
        InitScoreDefinitionTable();
        InitBattle();
    }

    private void Update()
    {

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
            if (i % ROUNDS_BETWEEN_SCORE_INCR == 0) // tous les 3 rounds, on incrémente le score
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
        ai = new AI(EnemyBC.Commander.AiType, this);

        // Start battle coroutine
        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        Debug.Log("The battle... begins!");
        while (CurrentRound <= MAX_ROUNDS)
        {
            // let player pick 
            yield return new WaitForSeconds(ai.MillisBeforeAction);
            UnitType aiPick = ai.PickUnit();
        }
    }
}
