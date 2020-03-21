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

    private void Update()
    {
        // DEBUG KEYS 
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("DEBUG: Clearing player prefs.");
            PlayerPrefs.DeleteAll();
        }
    }

    public void InitGame()
    {
        InitPlayer();
        InitEnemies();
    }

    /// <summary>
    /// Inits the player commander by setting his color and (loading or generating + saving) his portrait.
    /// </summary>
    private void InitPlayer()
    {
        Color playercolor = new Color(.2f, .6f, .86f);

        Portrait playerPortrait;
        // IF WE HAVE SAVES, load portrait from them
        if (PlayerPrefs.HasKey("player_saves"))
        {
            playerPortrait = new Portrait(
                PortraitGenerator.Instance.availableHair[PlayerPrefs.GetInt("player_portrait_hair")],
                PortraitGenerator.Instance.availableEyes[PlayerPrefs.GetInt("player_portrait_eyes")],
                PortraitGenerator.Instance.availableMouth[PlayerPrefs.GetInt("player_portrait_mouth")]
            );
            Debug.Log("Player portrait loaded.");
        }
        else // IF WE DO NOT HAVE SAVES, generate a portrait and save it 
        {
            playerPortrait = PortraitGenerator.Instance.GenerateRandomPlayerPortrait();

            // save the portrait
            PlayerPrefs.SetInt("player_portrait_hair", Array.IndexOf(PortraitGenerator.Instance.availableHair, playerPortrait.Hair));
            PlayerPrefs.SetInt("player_portrait_eyes", Array.IndexOf(PortraitGenerator.Instance.availableEyes, playerPortrait.Eyes));
            PlayerPrefs.SetInt("player_portrait_mouth", Array.IndexOf(PortraitGenerator.Instance.availableMouth, playerPortrait.Mouth));
            PlayerPrefs.SetInt("player_saves", 1); // used to check if we have saves for the player

            Debug.Log("Player portrait generated and saved.");
        }

        // Init player commander with loaded/generated stuff
        Player = new Commander(playercolor, playerPortrait, false);
    }

    /// <summary>
    /// Inits enemy commanders by loading their attributes or init + save them.
    /// </summary>
    private void InitEnemies()
    {
        // Init the array
        int amountOfCommanders = enemyCommandersElements.Length;
        Enemies = new Commander[amountOfCommanders];

        for (int i = 0; i < amountOfCommanders; i++)
        {
            Portrait portrait;
            bool locked;
            int winsCount = 0, lossesCount = 0;

            // IF WE HAVE SAVES, load them
            if (PlayerPrefs.HasKey("enemies_saves"))
            {
                // load portrait
                portrait = new Portrait(
                    PortraitGenerator.Instance.availableHair[PlayerPrefs.GetInt("enemy_" + i + "_portrait_hair")],
                    PortraitGenerator.Instance.availableEyes[PlayerPrefs.GetInt("enemy_" + i + "_portrait_eyes")],
                    PortraitGenerator.Instance.availableMouth[PlayerPrefs.GetInt("enemy_" + i + "_portrait_mouth")]
                );

                // load lock status
                locked = PlayerPrefs.GetInt("enemy_" + i + "_locked") == 1 ? true : false;

                // load wins & losses count
                winsCount = PlayerPrefs.GetInt("enemy_" + i + "_wins");
                lossesCount = PlayerPrefs.GetInt("enemy_" + i + "_losses");
            }
            else // IF WE DO NOT HAVE SAVES, just init a new commander
            {
                // generate and save portrait
                portrait = PortraitGenerator.Instance.GenerateRandomAIPortrait();
                PlayerPrefs.SetInt("enemy_" + i + "_portrait_hair", Array.IndexOf(PortraitGenerator.Instance.availableHair, portrait.Hair));
                PlayerPrefs.SetInt("enemy_" + i + "_portrait_eyes", Array.IndexOf(PortraitGenerator.Instance.availableEyes, portrait.Eyes));
                PlayerPrefs.SetInt("enemy_" + i + "_portrait_mouth", Array.IndexOf(PortraitGenerator.Instance.availableMouth, portrait.Mouth));

                // set and save lock status
                locked = enemyCommandersElements[i].Locked;
                PlayerPrefs.SetInt("enemy_" + i + "_locked", locked ? 1 : 0);

                // init and save w/l 
                PlayerPrefs.SetInt("enemy_" + i + "_wins", winsCount);
                PlayerPrefs.SetInt("enemy_" + i + "_losses", lossesCount);
            }

            Enemies[i] = new Commander(enemyCommandersElements[i].Color, portrait, locked, winsCount, lossesCount);
        }

        // if we didnt have saves before, now we do
        if (!PlayerPrefs.HasKey("enemies_saves"))
        {
            PlayerPrefs.SetInt("enemies_saves", 1);
            Debug.Log("Enemy commanders initialized and saved.");
        }
        else
        {
            Debug.Log("Enemy commanders loaded.");
        }
    }
}
