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
        if (Instance == null)
        {
            Instance = this;
        }

        InitGame();
    }

    public void InitGame()
    {
        Debug.Log("TODO: Init the game");
    }

}
