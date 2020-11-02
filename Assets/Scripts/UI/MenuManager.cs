﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private GameObject commanderGridSkippable;
    [SerializeField]
    private TextMeshProUGUI introText;
    [SerializeField]
    private Image reactionTextSeal;
    [SerializeField]
    private TextMeshProUGUI commanderNameText;

    [Header("(First start) Language Panel")]
    [SerializeField]
    private GameObject languagePanel;
    private bool languagePicked = false;

    [Header("Game Completed UI")]
    [SerializeField]
    private GameObject ggPanel;
    [SerializeField]
    private GameObject creditsPanel;

    [Header("SFX")]
    [SerializeField]
    private AudioSource pressKeySwooshSFX;

    private Animator mainMenuAnimator;
    private Animator introTextPanelAnimator;
    private Animator ggPanelAnimator;
    private Animator creditsPanelAnimator;

    // Intro texts 
    private const string INTRO_GAME_MSG = "<size=120%><b>Bienvenue dans la Ligue des Commandants d'Elite.</b></size>\n\nIl s'agit d'une compétition entre nos plus fins stratèges permettant de déterminer le meilleur commandant de l'Empire.\n\nVos exploits passés ont impressionné l'Empereur en personne, qui vous invite à rejoindre la Ligue en tant que challenger.";
    private const string INTRO_GRID_INSTRUCTIONS = "Ici vous verrez un tableau récapitulatif des commandants de la Ligue. A tout moment vous pouvez affronter un concurrent disponible, même si vous l'avez déjà vaincu.";
    private const string INTRO_GRID_MSG = "<size=110%><b>La Ligue se limite à 10 commandants, classés par ordre de compétence sur le champs de bataille.</b></size>\n\nEn tant que challenger, vous devrez vaincre chaque commandant un par un en commençant par le 10ème jusqu'à triompher du Grand Champion de la Ligue.";
    private const string INTRO_MY_COMMANDER_MSG = "<size=110%><b>Vos victoires sur le champs de bataille vous permettront de débloquer des éléments de personnalisation de votre apparence.</b></size>\n\nChaque commandant de la Ligue possède un trait d'apparence unique qui vous sera octroyé si l'emportez.\n\nLes meilleurs mages de l'Empire seront capable d'altérer votre apparence à tout moment avec ce que vous aurez débloqué.";
    private const string INTRO_FINAL_MSG = "<size=120%><b>Commandant, l'arène de combat vous attend.</b></size>\n\nJe serai votre premier adversaire. Ce sera l'occasion de vous expliquer le fonctionnement des batailles au sein de la Ligue.\n\nJ'ai hate de pouvoir me mesurer à vous sur le champs de bataille.";

    private bool isInGridIntro = false;

    private bool isInTitleScreen;

    private void Start()
    {
        mainMenuAnimator = this.mainMenuUI.GetComponent<Animator>();
        ggPanelAnimator = ggPanel.GetComponent<Animator>();
        creditsPanelAnimator = creditsPanel.GetComponent<Animator>();

        // make first start 
        if (GameManager.Instance.FirstStart)
        {
            GameManager.Instance.Language = Lang.NULL;
            this.languagePanel.SetActive(true);
            StartCoroutine(WaitForLanguage());
        }
        else
        {
            Debug.Log(PlayerPrefs.GetString("lang"));
            // Load language
            if (PlayerPrefs.GetString("lang") == "FR")
            {
                GameManager.Instance.SetFrenchLang();
                languagePicked = true;
            }
            else if (PlayerPrefs.GetString("lang") == "EN")
            {
                GameManager.Instance.SetEnglishLang();
                languagePicked = true;
            }
            else
            {
                throw new Exception("Unknown language");
            }
        }

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
        if (isInTitleScreen && languagePicked && Input.anyKeyDown)
        {
            this.languagePanel.SetActive(false);
            StartCoroutine(DoPressAnyKeyAction());
        }
    }

    private IEnumerator DoPressAnyKeyAction()
    {
        isInTitleScreen = false;

        this.pressKeySwooshSFX.Play();

        yield return new WaitForSeconds(.1f);

        GoToMainMenu();
        GameManager.Instance.GameHasBeenInit = true;
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

        if (GameManager.Instance.FirstStart) // cas premier lancement => intro/tuto
        {
            mainMenuUI.SetActive(true);
            mainMenuAnimator.SetBool("IntroMode", true);

            introTextPanelAnimator = this.introTextPanel.GetComponent<Animator>();
            StartCoroutine(HandleFirstStart());
            GameManager.Instance.FirstStart = false;
        }
        else if (GameManager.Instance.JustCompletedGame)
        {
            StartCoroutine(HandleGGScreens());
            GameManager.Instance.JustCompletedGame = false;
        }
        else
        {
            mainMenuUI.SetActive(true);

            this.playerCommanderTopLeft.SetActive(true);
            this.commanderGridPopup.SetActive(true);
        }
    }

    /// <summary>
    /// Wait for a.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForLanguage()
    {
        yield return new WaitUntil(() => GameManager.Instance.Language != Lang.NULL);
        languagePicked = true;
        PlayerPrefs.SetString("lang", GameManager.Instance.Language.ToString());
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
        this.introTextPanel.SetActive(true);
        DisplayIntroMessage(INTRO_GAME_MSG);
        yield return new WaitUntil(() => !this.introTextPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.introTextPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        // display commander grid
        this.commandersGrid.GetComponentInChildren<Button>().enabled = false;
        this.commandersGrid.GetComponentInChildren<EventTrigger>().enabled = false;
        this.commanderGridSkippable.SetActive(true);
        this.commanderGridPopup.SetActive(true);
        mainMenuAnimator.Play("MainMenuCommandersGrid");

        string normalText = this.commanderGridInstructions.text;
        this.commanderGridInstructions.text = INTRO_GRID_INSTRUCTIONS;

        isInGridIntro = true;
        yield return new WaitUntil(() => this.mainMenuAnimator.GetBool("CloseGridPopup")); // wait until grid popup is unactive
        yield return new WaitForSeconds(this.mainMenuAnimator.runtimeAnimatorController.animationClips[2].length);

        this.commanderGridInstructions.text = normalText;
        this.commanderGridPopup.SetActive(false);
        this.commanderGridSkippable.SetActive(false);

        // 2nd msg : commander grid
        DisplayIntroMessage(INTRO_GRID_MSG);
        yield return new WaitUntil(() => !this.introTextPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.introTextPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        // display player commander + 3rd msg : player commander
        this.playerCommanderTopLeft.SetActive(true);
        mainMenuAnimator.Play("MainMenuCommanderBtn");

        DisplayIntroMessage(INTRO_MY_COMMANDER_MSG);
        yield return new WaitUntil(() => !this.introTextPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.introTextPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        // 4th msg : come fight me
        isInGridIntro = false;

        DisplayIntroMessage(INTRO_FINAL_MSG);
        yield return new WaitUntil(() => !this.introTextPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.introTextPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        this.commanderGridPopup.SetActive(true);
        mainMenuAnimator.Play("MainMenuCommandersGrid");
        mainMenuAnimator.SetBool("IntroMode", false);

        this.commandersGrid.GetComponentInChildren<Button>().enabled = true;
        this.commandersGrid.GetComponentInChildren<EventTrigger>().enabled = true;
    }

    public void HideIntroTextPanel()
    {
        introTextPanelAnimator.SetBool("Opened", false);
    }

    public void HideGGPanel()
    {
        ggPanelAnimator.SetBool("Opened", false);
    }
    public void HideCreditsPanel()
    {
        creditsPanelAnimator.SetBool("Opened", false);
    }

    /// <summary>
    /// Handles the display of gg letter from emperor and the credits.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleGGScreens()
    {
        // display emperor's letter and wait for user to skip it 
        this.ggPanel.SetActive(true);
        yield return new WaitUntil(() => !this.ggPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.ggPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        // display credits 
        this.creditsPanel.SetActive(true);
        yield return new WaitUntil(() => !this.creditsPanelAnimator.GetBool("Opened")); // wait until user skips msg with a clic
        yield return new WaitForSeconds(this.creditsPanelAnimator.runtimeAnimatorController.animationClips[0].length);

        // return to normal menu
        mainMenuUI.SetActive(true);
        this.playerCommanderTopLeft.SetActive(true);
        this.commanderGridPopup.SetActive(true);
    }

    /// <summary>
    /// Displays a given message on the screen.
    /// </summary>
    /// <param name="msg"></param>
    private void DisplayIntroMessage(string msg)
    {
        this.introText.text = msg;
        introTextPanelAnimator.SetBool("Opened", true);
    }

    /// <summary>
    /// SetActive(false) on grid popup if in intro mode.
    /// </summary>
    public void HandleClickOnPopup()
    {
        if (isInGridIntro)
        {
            this.mainMenuAnimator.SetBool("CloseGridPopup", true);
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
