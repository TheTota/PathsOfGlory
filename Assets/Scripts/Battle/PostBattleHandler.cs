using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles the post battle screen. Takes some weight off the battle manager.
/// </summary>
public class PostBattleHandler : MonoBehaviour
{
    [SerializeField]
    private BattleManager bm;
    [SerializeField]
    private UIPlaysHistoryHandler playsHistoryHandler;
    [SerializeField]
    private GameObject[] uiToHide;
    [SerializeField]
    private GameObject refightBtn;

    [Header("Result")]
    [SerializeField]
    private TextMeshProUGUI resultText;

    [Header("Battle Recap")]
    [SerializeField]
    private GameObject recapRoundTextPrefab;
    [SerializeField]
    private GameObject recapElementPrefab;
    [SerializeField]
    private RectTransform roundsRowParent;
    [SerializeField]
    private RectTransform playerRowParent;
    [SerializeField]
    private RectTransform enemyRowParent;

    [Header("Rewards")]
    [SerializeField]
    private RectTransform rewardsParent;
    [SerializeField]
    private TextMeshProUGUI rewardsText;
    [SerializeField]
    private GameObject unlockedCommanderText;
    [SerializeField]
    private PortraitRenderer unlockedCommanderPR;
    [SerializeField]
    private PortraitRenderer unlockedElementPR;

    [Header("Audio")]
    [SerializeField]
    private AudioSource victoryAS;
    [SerializeField]
    private AudioSource defeatAS;


    private void Awake()
    {
        InitPostBattle();
    }

    private void InitPostBattle()
    {
        HideUI();

        // init victory/defeat text
        DisplayResultTitle();

        // fill the battle recap
        FillBattleRecap();

        // display rewards
        HandleRewards();
    }

    /// <summary>
    /// Hides the gameobjets to hide.
    /// </summary>
    private void HideUI()
    {
        foreach (var go in this.uiToHide)
        {
            go.SetActive(false);
        }
    }

    /// <summary>
    /// Displays victory or defeat based on result of the game
    /// </summary>
    private void DisplayResultTitle()
    {
        if (this.bm.BattleWinner == this.bm.PlayerBC)
        {
            victoryAS.Play();

            // translation
            if (GameManager.Instance.Language == Lang.FR)
            {
                resultText.text = "VICTOIRE";
            }
            else if (GameManager.Instance.Language == Lang.EN)
            {
                resultText.text = "VICTORY";
            }

        }
        else
        {
            defeatAS.Play();

            // translation
            if (GameManager.Instance.Language == Lang.FR)
            {
                resultText.text = "DEFAITE";
            }
            else if (GameManager.Instance.Language == Lang.EN)
            {
                resultText.text = "DEFEAT";
            }
        }
    }

    /// <summary>
    /// Displays a recap of the battle, with rounds history.
    /// </summary>
    private void FillBattleRecap()
    {
        // Init variables 
        List<UnitType> playerPlaysHistory = this.bm.PlayerBC.PlaysHistory;
        List<UnitType> enemyPlaysHistory = this.bm.EnemyBC.PlaysHistory;

        if (playerPlaysHistory.Count != enemyPlaysHistory.Count)
        {
            throw new Exception("Player and AI dont have the same amount of rounds played in their history.");
        }

        int amountOfRoundsPlayed = playerPlaysHistory.Count;

        // Init UI
        InitBackgroundColor();

        // Fill the recap
        for (int i = 0; i < amountOfRoundsPlayed; i++)
        {
            // Round Text
            TextMeshProUGUI round = Instantiate(this.recapRoundTextPrefab, this.roundsRowParent).GetComponent<TextMeshProUGUI>();
            round.text = (i + 1).ToString();

            // Player recap
            RenderRecapElement(this.playerRowParent, playerPlaysHistory[i], this.bm.RoundsWinnersHistory[i] == this.bm.PlayerBC, this.bm.RoundsWinnersHistory[i] == null);

            // Enemy recap
            RenderRecapElement(this.enemyRowParent, enemyPlaysHistory[i], this.bm.RoundsWinnersHistory[i] == this.bm.EnemyBC, this.bm.RoundsWinnersHistory[i] == null);
        }
    }

    /// <summary>
    /// Inits the color of the background of the players history panels.
    /// </summary>
    private void InitBackgroundColor()
    {
        // set background color for player
        Color playerColor = this.bm.PlayerBC.Commander.Color;
        playerRowParent.GetComponent<Image>().color = new Color(playerColor.r, playerColor.g, playerColor.b, .8f);
        // set background color for enemy
        Color enemyColor = this.bm.EnemyBC.Commander.Color;
        enemyRowParent.GetComponent<Image>().color = new Color(enemyColor.r, enemyColor.g, enemyColor.b, .8f);
    }

    /// <summary>
    /// Actually renders the plays history by adding a new element to it, based on received data.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="ut"></param>
    /// <param name="isWinner"></param>
    private void RenderRecapElement(RectTransform parent, UnitType ut, bool isWinner, bool isDraw = false)
    {
        // instantiate the history item
        GameObject item = Instantiate(recapElementPrefab, parent);

        // color depending on if win or not 
        if (isDraw)
        {
            item.GetComponent<Image>().color = new Color(.71f, .71f, .71f); // some light grey
        }
        else
        {
            if (isWinner)
            {
                item.GetComponent<Image>().color = new Color(.235f, .94f, .235f); // some green
            }
            else
            {
                item.GetComponent<Image>().color = new Color(.94f, .235f, .235f); // some pomegranate red
            }
        }

        // display the right unit 
        Image unitImg = item.transform.Find("UnitIcon").GetComponent<Image>();
        switch (ut)
        {
            case UnitType.Knights:
                unitImg.sprite = playsHistoryHandler.knightSprite;
                break;
            case UnitType.Shields:
                unitImg.sprite = playsHistoryHandler.shieldsSprite;
                break;
            case UnitType.Spearmen:
                unitImg.sprite = playsHistoryHandler.spearSprite;
                break;
            case UnitType.Mages:
                unitImg.sprite = playsHistoryHandler.mageSprite;
                break;
            case UnitType.Archers:
                unitImg.sprite = playsHistoryHandler.archerSprite;
                break;
            default:
                throw new Exception("ut is wrong");
        }
    }

    /// <summary>
    /// Displays rewards if there are any. Else, tell the player he already got every reward.
    /// </summary>
    private void HandleRewards()
    {
        if (this.bm.BattleWinner == this.bm.PlayerBC) // si on a gagné
        {
            if (this.bm.UnlockedRewards) // si on a débloqué des récompenses
            {
                this.rewardsParent.gameObject.SetActive(true);

                // display unlocked commander
                int unlockedCommanderIndex = Array.IndexOf(GameManager.Instance.Enemies, this.bm.EnemyBC.Commander) + 1;
                if (unlockedCommanderIndex < GameManager.Instance.Enemies.Length)
                {
                    this.unlockedCommanderPR.RenderPortrait(GameManager.Instance.Enemies[unlockedCommanderIndex]);
                }
                else // if it's last commander 
                {
                    this.unlockedCommanderPR.gameObject.SetActive(false);
                    this.unlockedCommanderText.SetActive(false);
                    this.refightBtn.SetActive(false);
                }

                // display unlocked portrait element
                this.unlockedElementPR.RenderElement(GameManager.Instance.LastUnlockedPortraitElement);
            }
            else
            {
                this.rewardsParent.gameObject.SetActive(false);
                this.rewardsText.text = "Toutes les récompenses pour ce commandant ont déja été obtenues.";
            }
        }
        else
        {
            if (this.bm.EnemyBC.Commander.WinsCount == 0) // si on peut avoir une récompense en battant l'adversaire
            {
                this.rewardsParent.gameObject.SetActive(false);
                this.rewardsText.text = "Battez ce commandant au moins une fois pour obtenir des récompenses.";
            }
            else
            {
                this.rewardsParent.gameObject.SetActive(false);
                this.rewardsText.text = "Toutes les récompenses pour ce commandant ont déja été obtenues.";
            }
        }
    }

    #region Buttons Interactions
    /// <summary>
    /// Returns to main menu.
    /// </summary>
    public void ReturnToMenu()
    {
        // saves already done in battle manager
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Reloads scene to start battle against same commander.
    /// </summary>
    public void Rematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
