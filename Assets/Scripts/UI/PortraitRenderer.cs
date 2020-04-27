using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public enum PortraitMood
{
    Neutral,
    Proud,
    Angry,
}

/// <summary>
/// Allows to render a given commander's portrait graphically.
/// </summary>
public class PortraitRenderer : MonoBehaviour
{
    // Every UI portrait attribute to render
    [SerializeField]
    private Image background;
    [SerializeField]
    private Button btn;
    [SerializeField]
    private GameObject face;
    [SerializeField]
    private Image hair;
    [SerializeField]
    private Image eyes;
    [SerializeField]
    private Image mouth;
    [SerializeField]
    private TextMeshProUGUI winsText;
    [SerializeField]
    private TextMeshProUGUI lossText;
    [SerializeField]
    private EventTrigger btnSFXEventTrigger;

    /// <summary>
    /// Renders the portrait with the given values.
    /// </summary>
    /// <param name="commanderColor"></param>
    /// <param name="onClickAction"></param>
    /// <param name="hairSprite"></param>
    /// <param name="eyesSprite"></param>
    /// <param name="mouthSprite"></param>
    public void RenderPortrait(Commander commander, UnityAction onClickAction = null)
    {
        // Color portrait
        if (!commander.Locked) // if unlocked
        {
            background.color = commander.Color;

            // Face attributes
            hair.sprite = commander.Portrait.Hair.neutralSprite;
            eyes.sprite = commander.Portrait.Eyes.neutralSprite;
            mouth.sprite = commander.Portrait.Mouth.neutralSprite;
            face.GetComponent<Image>().color = commander.Portrait.SkinTone;

            if (commander.WinsCount != 0 || commander.LossesCount != 0)
            {
                winsText.gameObject.SetActive(true);
                lossText.gameObject.SetActive(true);
                winsText.text = "V" + commander.WinsCount;
                lossText.text = "D" + commander.LossesCount;
            }

            // enable btn audio
            btnSFXEventTrigger.enabled = true;
        }
        else // if locked
        {
            background.color = new Color(.5f, .5f, .5f);
            winsText.gameObject.SetActive(false);
            lossText.gameObject.SetActive(false);

            // disable btn audio
            btnSFXEventTrigger.enabled = false;
        }

        // Button
        btn.onClick.AddListener(onClickAction);
        btn.interactable = !commander.Locked;

        // hide the face if commander is locked
        face.SetActive(!commander.Locked);
    }

    /// <summary>
    /// Renders 1 element on the portrait. Used to display unlocked portrait element to play post battle.
    /// </summary>
    /// <param name="elt"></param>
    public void RenderElement(PortraitElement elt)
    {
        // if element to render is Hair
        if (Array.IndexOf(PortraitGenerator.Instance.availableHair, elt) > -1)
        {
            hair.sprite = elt.neutralSprite;
            hair.gameObject.SetActive(true);
        }
        // else if Eyes
        else if (Array.IndexOf(PortraitGenerator.Instance.availableEyes, elt) > -1)
        {
            eyes.sprite = elt.neutralSprite;
            eyes.gameObject.SetActive(true);
        }
        // else if Mouth
        else if (Array.IndexOf(PortraitGenerator.Instance.availableMouth, elt) > -1)
        {
            mouth.sprite = elt.neutralSprite;
            mouth.gameObject.SetActive(true);
        }
        // problem 
        else
        {
            throw new Exception("Portrait element isn't referenced as an available hair/eyes/mouth.");
        }
    }

    /// <summary>
    /// Displays a given mood on the associated portrait.
    /// </summary>
    /// <param name="commander"></param>
    /// <param name="mood"></param>
    internal void RenderMood(Commander commander, PortraitMood mood)
    {
        switch (mood)
        {
            case PortraitMood.Neutral:
                eyes.sprite = commander.Portrait.Eyes.neutralSprite;
                mouth.sprite = commander.Portrait.Mouth.neutralSprite;
                break;

            case PortraitMood.Proud:
                eyes.sprite = commander.Portrait.Eyes.happySprite;
                mouth.sprite = commander.Portrait.Mouth.happySprite;
                break;

            case PortraitMood.Angry:
                eyes.sprite = commander.Portrait.Eyes.angrySprite;
                mouth.sprite = commander.Portrait.Mouth.angrySprite;
                break;

            default:
                throw new Exception("what is that mood? " + mood);
        }
    }
}
