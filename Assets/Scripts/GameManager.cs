using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        // TODO: CHECK SAVES
        InitPlayer();
        InitEnemies();
    }

    private void InitPlayer()
    {
        Player = new Commander(new Color(.2f, .6f, .86f), PortraitGenerator.Instance.GenerateRandomPlayerPortrait());
    }

    private void InitEnemies()
    {
        Debug.Log("TODO: init enemies");
    }
}
