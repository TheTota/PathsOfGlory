using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows the player to customize his commander with the unlocked portrait elements.
/// </summary>
public class PortraitCustomizer : MonoBehaviour
{
    [SerializeField]
    private PortraitRenderer portraitRenderer;

    [SerializeField]
    private GameObject mainMenuUI;

    // buttons references to disable them if only 1 available hair/eyes/mouth
    [Header("Portrait elements")]
    [SerializeField]
    private Button prevHairBtn;
    [SerializeField]
    private Button prevEyesBtn;
    [SerializeField]
    private Button prevMouthBtn;
    [SerializeField]
    private Button nextHairBtn;
    [SerializeField]
    private Button nextEyesBtn;
    [SerializeField]
    private Button nextMouthBtn;

    [Header("Skin tone")]
    [SerializeField]
    private Transform skinToneBtnParent;
    [SerializeField]
    private GameObject skinToneBtnPrefab;

    // browsing indexes
    private int currentHairIndex;
    private int currentEyesIndex;
    private int currentMouthIndex;

    private bool skinTonesBtnsGenerated;

    private void OnEnable()
    {
        if (!skinTonesBtnsGenerated)
        {
            GenerateSkinToneBtns();
            skinTonesBtnsGenerated = true;
        }
        RenderPlayerPortrait();

        // init browsing indexes
        Portrait p = GameManager.Instance.Player.Portrait;
        currentHairIndex = Array.IndexOf(PortraitGenerator.Instance.GetUnlockedHair(), p.Hair);
        currentEyesIndex = Array.IndexOf(PortraitGenerator.Instance.GetUnlockedEyes(), p.Eyes);
        currentMouthIndex = Array.IndexOf(PortraitGenerator.Instance.GetUnlockedMouth(), p.Mouth);

        HandleButtonsAvailabilityBasedOnUnlockedElements();
    }

    /// <summary>
    /// Generates buttons to change the skin tone of the player commander, according to a list of skin tones in the portrait generator singleton.
    /// If 10 skin tones available ==> generates 10 btns.
    /// </summary>
    private void GenerateSkinToneBtns()
    {
        foreach (var skinTone in PortraitGenerator.Instance.availableSkinTones)
        {
            GameObject skinToneBtn = Instantiate(skinToneBtnPrefab, skinToneBtnParent);
            skinToneBtn.GetComponent<Image>().color = skinTone;
            skinToneBtn.GetComponent<Button>().onClick.AddListener(() => ChangeSkinTone(skinTone));
        }
    }

    /// <summary>
    /// Changes and renders the skin tone of the player to the given one.
    /// </summary>
    /// <param name="skinTone"></param>
    private void ChangeSkinTone(Color skinTone)
    {
        GameManager.Instance.Player.Portrait.SkinTone = skinTone;
        RenderPlayerPortrait();
    }

    /// <summary>
    /// Enables/disable the interactivity with the customization buttons, depending on the amound of unlocked elements.
    /// </summary>
    private void HandleButtonsAvailabilityBasedOnUnlockedElements()
    {
        // disable hair buttons if only 1 unlocked hair
        if (PortraitGenerator.Instance.GetUnlockedHair().Length <= 1)
        {
            nextHairBtn.interactable = false;
            nextHairBtn.GetComponent<EventTrigger>().enabled = false;

            prevHairBtn.interactable = false;
            prevHairBtn.GetComponent<EventTrigger>().enabled = false;
        }
        else
        {
            nextHairBtn.interactable = true;
            nextHairBtn.GetComponent<EventTrigger>().enabled = true;

            prevHairBtn.interactable = true;
            prevHairBtn.GetComponent<EventTrigger>().enabled = true;
        }

        // disable eyes buttons if only 1 unlocked eyes
        if (PortraitGenerator.Instance.GetUnlockedEyes().Length <= 1)
        {
            nextEyesBtn.interactable = false;
            nextEyesBtn.GetComponent<EventTrigger>().enabled = false;

            prevEyesBtn.interactable = false;
            prevEyesBtn.GetComponent<EventTrigger>().enabled = false;
        }
        else
        {
            nextEyesBtn.interactable = true;
            nextEyesBtn.GetComponent<EventTrigger>().enabled = true;

            prevEyesBtn.interactable = true;
            prevEyesBtn.GetComponent<EventTrigger>().enabled = true;
        }

        // disable mouth buttons if only 1 unlocked mouth
        if (PortraitGenerator.Instance.GetUnlockedMouth().Length <= 1)
        {
            nextMouthBtn.interactable = false;
            nextMouthBtn.GetComponent<EventTrigger>().enabled = false;

            prevMouthBtn.interactable = false;
            prevMouthBtn.GetComponent<EventTrigger>().enabled = false;
        }
        else
        {
            nextMouthBtn.interactable = true;
            nextMouthBtn.GetComponent<EventTrigger>().enabled = true;

            prevMouthBtn.interactable = true;
            prevMouthBtn.GetComponent<EventTrigger>().enabled = true;
        }
    }

    /// <summary>
    /// Renders the player portrait in the customization popup.
    /// </summary>
    public void RenderPlayerPortrait()
    {
        portraitRenderer.RenderPortrait(GameManager.Instance.Player);
    }

    /// <summary>
    /// Saves the modified portrait and closes the popup to bring back the main menu with the commanders grid.
    /// </summary>
    public void ClosePopup()
    {
        // Save portrait
        Portrait p = GameManager.Instance.Player.Portrait;
        PlayerPrefs.SetInt("player_portrait_hair", Array.IndexOf(PortraitGenerator.Instance.availableHair, p.Hair));
        PlayerPrefs.SetInt("player_portrait_eyes", Array.IndexOf(PortraitGenerator.Instance.availableEyes, p.Eyes));
        PlayerPrefs.SetInt("player_portrait_mouth", Array.IndexOf(PortraitGenerator.Instance.availableMouth, p.Mouth));
        PlayerPrefs.SetInt("player_portrait_skin_tone", Array.IndexOf(PortraitGenerator.Instance.availableSkinTones, p.SkinTone));

        // Go back to menu
        mainMenuUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Goes to the next available hair : sets it for the player and renders it.
    /// </summary>
    public void NextHair()
    {
        GameManager.Instance.Player.Portrait.Hair = GetNextElement(PortraitGenerator.Instance.GetUnlockedHair(), ref currentHairIndex);
    }

    /// <summary>
    /// Goes to the previous available hair : sets it for the player and renders it.
    /// </summary>
    public void PrevHair()
    {
        GameManager.Instance.Player.Portrait.Hair = GetPreviousElement(PortraitGenerator.Instance.GetUnlockedHair(), ref currentHairIndex);
    }

    /// <summary>
    /// Goes to the next available eyes : sets it for the player and renders it.
    /// </summary>
    public void NextEyes()
    {
        GameManager.Instance.Player.Portrait.Eyes = GetNextElement(PortraitGenerator.Instance.GetUnlockedEyes(), ref currentEyesIndex);
    }

    /// <summary>
    /// Goes to the previous available eyes : sets it for the player and renders it.
    /// </summary>
    public void PrevEyes()
    {
        GameManager.Instance.Player.Portrait.Eyes = GetPreviousElement(PortraitGenerator.Instance.GetUnlockedEyes(), ref currentEyesIndex);
    }

    /// <summary>
    /// Goes to the next available mouth : sets it for the player and renders it.
    /// </summary>
    public void NextMouth()
    {
        GameManager.Instance.Player.Portrait.Mouth = GetNextElement(PortraitGenerator.Instance.GetUnlockedMouth(), ref currentMouthIndex);
    }

    /// <summary>
    /// Goes to the previous available mouth : sets it for the player and renders it.
    /// </summary>
    public void PrevMouth()
    {
        GameManager.Instance.Player.Portrait.Mouth = GetPreviousElement(PortraitGenerator.Instance.GetUnlockedMouth(), ref currentMouthIndex);
    }

    /// <summary>
    /// Returns the next element of the given list, based on the given index.
    /// </summary>
    /// <param name="unlockedElements">List of portrait elements, example: the list of unlocked hair.</param>
    /// <param name="index">Index used to browse</param>
    /// <returns>The next portrait element in the list.</returns>
    private PortraitElement GetNextElement(PortraitElement[] unlockedElements, ref int index)
    {
        PortraitElement elementToChange;
        if (index + 1 < unlockedElements.Length) // if we're browsing the available cuts and havent reached end of the list
        {
            index++;
            elementToChange = unlockedElements[index];
        }
        else // if we reached the end of list, go back to the first index
        {
            index = 0;
            elementToChange = unlockedElements[index];
        }

        return elementToChange;
    }

    /// <summary>
    /// Returns the previous element of the given list, based on the given index.
    /// </summary>
    /// <param name="unlockedElements">List of portrait elements, example: the list of unlocked hair.</param>
    /// <param name="index">Index used to browse</param>
    /// <returns>The previous portrait element in the list.</returns>
    private PortraitElement GetPreviousElement(PortraitElement[] unlockedElements, ref int index)
    {
        PortraitElement elementToChange;
        if (index - 1 >= 0) // if we're browsing the available cuts and havent reached end of the list
        {
            index--;
            elementToChange = unlockedElements[index];
        }
        else // if we reached the end of list, go back to the first index
        {
            index = unlockedElements.Length - 1;
            elementToChange = unlockedElements[index];
        }

        return elementToChange;
    }

}
