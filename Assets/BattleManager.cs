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

    public AI ai;

    // Start is called before the first frame update
    void Awake()
    {
        InitBattle();
    }

    /// <summary>
    /// Initialise et démarre une bataille entre 2 commandants.
    /// </summary>
    private void InitBattle()
    {
        // Init Player
        PlayerBC = (BattleCommander)playerPR.gameObject.AddComponent(typeof(BattleCommander));
        PlayerBC.Init(GameManager.Instance.Player);
        playerPR.RenderPortrait(PlayerBC.Commander);

        // Init Enemy 
        EnemyBC = (BattleCommander)enemyPR.gameObject.AddComponent(typeof(BattleCommander));
        EnemyBC.Init(GameManager.Instance.BattledCommander);
        enemyPR.RenderPortrait(EnemyBC.Commander);

        // Start battle coroutine

    }
}
