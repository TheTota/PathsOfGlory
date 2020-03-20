using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleUI;

    [SerializeField]
    private GameObject mainMenuUI;

    [SerializeField]
    private GameObject portraitCustomizerPopup;

    private bool isInTitleScreen;

    private void Awake()
    {
        isInTitleScreen = true;
    }

    private void Update()
    {
        // handle that "press any key" thing
        if (isInTitleScreen && Input.anyKeyDown)
        {
            GoToMainMenu();
            isInTitleScreen = false;
        }
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
        // TODO: save stuff?
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
    private void RenderPlayerPortrait()
    {
        Debug.Log("TODO: render player portrait");
    }

    /// <summary>
    /// Renders the grid of commanders, based on the declared enemy commanders in the game manager.
    /// </summary>
    private void RenderCommandersGrid()
    {
        Debug.Log("TODO: render commanders grid");
    }

    /// <summary>
    /// Opens the popup to customize the commander of the player.
    /// </summary>
    public void OpenPortraitCustomizer()
    {
        Debug.Log("TODO: open portrait customizer");
    }

    /// <summary>
    /// Closes the popup to customize the commander of the player.
    /// </summary>
    public void ClosePortraitCustomize()
    {
        Debug.Log("TODO: close portrait customizer");
    }

    /// <summary>
    /// Starts a battle against the enemy commander given in parameters.
    /// </summary>
    /// <param name="enemyCommander"></param>
    public void StartBattle(Commander enemyCommander)
    {
        Debug.Log("TODO: start battle");
    }
}
