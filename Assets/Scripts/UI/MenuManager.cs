using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
