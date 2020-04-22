using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu interactions and behaviours. 
/// Does not handle the portrait customization interactions, although it allows to open the popup.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleUI;
    [SerializeField]
    private GameObject mainMenuUI;
    [SerializeField]
    private GameObject portraitCustomizerPopup;

    [SerializeField]
    private GameObject commanderPortraitPrefab;

    [SerializeField]
    private RectTransform playerPortrait;
    [SerializeField]
    private RectTransform commandersGrid;

    [Header("(First start) Focus Backgrounds")]
    // black almost transparent backgrounds to focus on UI
    [SerializeField]
    private GameObject fbgPlayerCommander;
    [SerializeField]
    private GameObject fbgCommanderGrid;
    [SerializeField]
    private GameObject fbgBoth;

    [Header("(First start) Main UI Elements")]
    [SerializeField]
    private GameObject commanderGridPopup;
    [SerializeField]
    private TextMeshProUGUI commanderGridInstructions;
    [SerializeField]
    private GameObject playerCommanderTopLeft;

    [Header("(First start) Text Panel")]
    [SerializeField]
    private GameObject introTextPanel;
    [SerializeField]
    private TextMeshProUGUI introText;
    [SerializeField]
    private Image reactionTextSeal;
    [SerializeField]
    private TextMeshProUGUI commanderNameText;

    // Intro texts 
    private const string INTRO_GAME_MSG = "<size=120%><b>Bienvenue dans la Ligue des Commandants d'Elite de l'Empire.</b></size>\n\nIl s'agit d'une compétition entre nos plus fins stratèges permettant de déterminer le meilleur commandant de l'Empire.\n\nVos exploits passés ont impressionné l'Empereur en personne, qui vous invite à rejoindre la Ligue en tant que challenger.";
    private const string INTRO_GRID_INSTRUCTIONS = "Ici vous verrez un tableau récapitulatif des commandants de la Ligue. A tout moment vous pouvez affronter un concurrent disponible, même si vous l'avez déjà vaincu.";
    private const string INTRO_GRID_MSG = "La grille que vous avez vu...";
    private const string INTRO_MY_COMMANDER_MSG = "Vous pouvez personnaliser votre commandant...";
    private const string INTRO_FINAL_MSG = "Venez m'affronter maintenant...";

    private bool isInGridIntro = false;

    private bool isInTitleScreen;

    private void Start()
    {
        // skip title screen if not first time on scene
        if (GameManager.Instance.GameHasBeenInit)
        {
            GoToMainMenu();
            isInTitleScreen = false;
        }
        else
        {
            isInTitleScreen = true;
        }
    }

    private void Update()
    {
        // handle that "press any key" thing
        if (isInTitleScreen && Input.anyKeyDown)
        {
            GoToMainMenu();
            isInTitleScreen = false;
            GameManager.Instance.GameHasBeenInit = true;
        }
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Goes from the title screen to the main menu. 
    /// De/activates UI components and populates contents.
    /// </summary>
    public void GoToMainMenu()
    {
        titleUI.SetActive(false);

        RenderPlayerPortrait();
        RenderCommandersGrid();

        mainMenuUI.SetActive(true);
        if (GameManager.Instance.FirstStart) // cas premier lancement => intro/tuto
        {
            StartCoroutine(HandleFirstStart());
        }
        else
        {
            this.playerCommanderTopLeft.SetActive(true);
            this.commanderGridPopup.SetActive(true);
        }
    }

    /// <summary>
    /// Handles the intro/tutorial on first start of the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleFirstStart()
    {
        // init intro message elements such as seal and name
        this.reactionTextSeal.color = GameManager.Instance.Enemies[0].Color;
        this.commanderNameText.text = GameManager.Instance.Enemies[0].CommanderName;

        // 1rst msg : intro to game
        this.fbgCommanderGrid.SetActive(true);
        DisplayIntroMessage(INTRO_GAME_MSG);
        yield return new WaitUntil(() => !this.introTextPanel.activeInHierarchy); // wait until user skips msg with a clic

        // display commander grid
        this.commandersGrid.GetComponentInChildren<Button>().enabled = false;
        this.commanderGridPopup.SetActive(true);
        string normalText = this.commanderGridInstructions.text;
        this.commanderGridInstructions.text = INTRO_GRID_INSTRUCTIONS;

        isInGridIntro = true;
        yield return new WaitUntil(() => !this.commanderGridPopup.activeInHierarchy); // wait until grid popup is unactive

        this.commanderGridInstructions.text = normalText;
        this.commanderGridPopup.SetActive(false);

        // 2nd msg : commander grid
        DisplayIntroMessage(INTRO_GRID_MSG);
        yield return new WaitUntil(() => !this.introTextPanel.activeInHierarchy); // wait until user skips msg with a clic

        // display player commander + 3rd msg : player commander
        this.fbgCommanderGrid.SetActive(false);
        this.fbgPlayerCommander.SetActive(true);
        this.playerCommanderTopLeft.SetActive(true);

        DisplayIntroMessage(INTRO_MY_COMMANDER_MSG);
        yield return new WaitUntil(() => !this.introTextPanel.activeInHierarchy); // wait until user skips msg with a clic

        this.fbgPlayerCommander.SetActive(false);

        // 4th msg : come fight me
        this.fbgBoth.SetActive(true);
        this.commanderGridPopup.SetActive(true);

        DisplayIntroMessage(INTRO_FINAL_MSG);
        yield return new WaitUntil(() => !this.introTextPanel.activeInHierarchy); // wait until user skips msg with a clic

        this.fbgBoth.SetActive(false);
        this.commandersGrid.GetComponentInChildren<Button>().enabled = true;
    }

    /// <summary>
    /// Displays a given message on the screen.
    /// </summary>
    /// <param name="msg"></param>
    private void DisplayIntroMessage(string msg)
    {
        this.introText.text = msg;
        this.introTextPanel.SetActive(true);
    }

    /// <summary>
    /// SetActive(false) on grid popup if in intro mode.
    /// </summary>
    public void HandleClickOnPopup()
    {
        if (isInGridIntro)
        {
            this.commanderGridPopup.SetActive(false);
        }
    }

    /// <summary>
    /// Renders the portrait of the player, based on the player commander declared in the game manager.
    /// </summary>
    public void RenderPlayerPortrait()
    {
        playerPortrait.GetComponent<PortraitRenderer>().RenderPortrait(GameManager.Instance.Player, OpenPortraitCustomizer);
    }

    /// <summary>
    /// Renders the grid of commanders, based on the declared enemy commanders in the game manager.
    /// </summary>
    private void RenderCommandersGrid()
    {
        foreach (var commander in GameManager.Instance.Enemies)
        {
            GameObject instanciatedCommander = Instantiate(this.commanderPortraitPrefab, this.commandersGrid);
            instanciatedCommander.GetComponent<PortraitRenderer>().RenderPortrait(commander, () => StartBattleBtn(commander));
        }
    }

    /// <summary>
    /// Opens the popup to customize the commander of the player.
    /// </summary>
    private void OpenPortraitCustomizer()
    {
        this.portraitCustomizerPopup.SetActive(true);
        this.mainMenuUI.SetActive(false);
    }

    /// <summary>
    /// Starts a battle against the enemy commander given in parameters.
    /// </summary>
    /// <param name="enemyCommander"></param>
    public void StartBattleBtn(Commander enemyCommander)
    {
        GameManager.Instance.StartBattle(enemyCommander);
    }
}
