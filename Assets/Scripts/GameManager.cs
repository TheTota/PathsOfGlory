using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Mettre dans l'ordre de difficulté")]
    /// <summary>
    /// Elements qui définissent les différents commandants à créer si pas de sauvegardes préalables.
    /// Les commandants sont indexés dans le tableau par ordre de difficulté (index 0 = easy, 9 = max difficulty).
    /// </summary>
    [SerializeField]
    private CommanderElement[] enemyCommandersElements;

    public static GameManager Instance { get; set; }
    public Commander Player { get; set; }
    public Commander[] Enemies { get; set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        InitGame();
    }

    public void InitGame()
    {
        // TODO: CHECK SAVES <==
        InitPlayer();
        InitEnemies();
    }

    private void InitPlayer()
    {
        Player = new Commander(new Color(.2f, .6f, .86f), PortraitGenerator.Instance.GenerateRandomPlayerPortrait());
    }

    private void InitEnemies()
    {
        int amountOfCommanders = enemyCommandersElements.Length;

        Enemies = new Commander[amountOfCommanders];
        for (int i = 0; i < amountOfCommanders; i++)
        {
            Enemies[i] = new Commander(enemyCommandersElements[i].Color, PortraitGenerator.Instance.GenerateRandomAIPortrait(), enemyCommandersElements[i].Locked);
        }
    }
}
