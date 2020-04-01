using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager 
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Mettre dans l'ordre de difficulté")]
    /// <summary>
    /// Elements that define the commanders. Used for initialisation purposes (index 0 = easy, 9 = max difficulty).
    /// </summary>
    [SerializeField]
    private CommanderElement[] enemyCommandersElements;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    // Commanders references
    public Commander Player { get; set; }
    public Commander[] Enemies { get; set; }

    // Commandant que l'on est sur le point d'affronter
    public Commander BattledCommander { get; set; }

    public bool GameHasBeenInit { get; set; }

    private void Awake()
    {
        // init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            InitGame();

            DontDestroyOnLoad(gameObject);
        }
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

    /// <summary>
    /// Starts the battle against the given commander.
    /// </summary>
    /// <param name="enemyCommander"></param>
    public void StartBattle(Commander enemyCommander)
    {
        this.BattledCommander = enemyCommander;
        SceneManager.LoadScene("Battle");
    }

    /// <summary>
    /// Inits the player.
    /// </summary>
    public void InitGame()
    {
        PortraitGenerator.Instance.InitPortraitElements();
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
            playerPortrait = PortraitGenerator.Instance.GenerateRandomPortraitFromUnlockedElements();

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

        // Go through each commander and init them
        for (int i = 0; i < amountOfCommanders; i++)
        {
            Portrait portrait;
            bool locked;
            int winsCount = 0, lossesCount = 0;
            bool wonLast = false;

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
                wonLast = PlayerPrefs.GetInt("enemy_" + i + "_won_last") == 1 ? true : false;
            }
            else // IF WE DO NOT HAVE SAVES, just init a new commander
            {
                // set and save lock status
                locked = enemyCommandersElements[i].Locked;
                portrait = PortraitGenerator.Instance.GenerateRandomPortraitFromUnlockedElements(); // generate tmp portrait
            }

            // Create a commander dialog from the scriptable object commander element
            CommanderDialogs cd = new CommanderDialogs(enemyCommandersElements[i].preBattleFirstTimeLine, enemyCommandersElements[i].preBattleWonLastLines, enemyCommandersElements[i].preBattleLostLastLines, enemyCommandersElements[i].postBattleWinLines, enemyCommandersElements[i].postBattleLossLines, enemyCommandersElements[i].preUnitsFightLines, enemyCommandersElements[i].postUnitsFightLossLines, enemyCommandersElements[i].postUnitsFightWinLines, enemyCommandersElements[i].postUnitsFightDrawLines);

            // Generate commander
            Enemies[i] = new Commander(i, enemyCommandersElements[i].Color, portrait, enemyCommandersElements[i].PortraitElementToUnlock, enemyCommandersElements[i].AiType, locked, winsCount, lossesCount, wonLast, cd);
            Enemies[i].SaveStats();
        }

        // if we didnt have saves before, now we do
        if (!PlayerPrefs.HasKey("enemies_saves"))
        {
            PlayerPrefs.SetInt("enemies_saves", 1);
            UnlockNextCommander(); // unlocked 1rst commander 

            Debug.Log("Enemy commanders initialized and saved.");
        }
        else
        {
            Debug.Log("Enemy commanders loaded.");
        }
    }

    /// <summary>
    /// Unlocks the next commander on the list, if there is still some to be unlocked.
    /// </summary>
    public void UnlockNextCommander()
    {
        bool unlockedSomebody = false;
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i].Locked && !unlockedSomebody)
            {
                // Unlock beaten commander's portrait element
                if (i != 0)
                {
                    Enemies[i - 1].PortraitElementToUnlock.locked = false;
                    PlayerPrefs.SetInt(Enemies[i - 1].PortraitElementToUnlock.name, 0);
                }

                // Unlock next commander
                Enemies[i].Locked = false;
                Enemies[i].Portrait = PortraitGenerator.Instance.GenerateRandomPortraitFromUnlockedElementsAndWithGivenElement(Enemies[i].PortraitElementToUnlock); // generate new commander's portrait from unlocked stuff
                Enemies[i].SaveStats();

                unlockedSomebody = true;
            }
        }

        if (!unlockedSomebody)
        {
            // TODO: gg screen
            Debug.Log("Every commander has been unlocked already.");
        }
    }
}
