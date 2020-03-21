using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

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

    /// <summary>
    /// Renders the portrait with the given values.
    /// </summary>
    /// <param name="commanderColor"></param>
    /// <param name="onClickAction"></param>
    /// <param name="hairSprite"></param>
    /// <param name="eyesSprite"></param>
    /// <param name="mouthSprite"></param>
    public void RenderPortrait(Commander commander, UnityAction onClickAction)
    {
        // Color portrait
        if (!commander.Locked) // if unlocked
        {
            background.color = commander.Color;
            if (commander.WinsCount != 0 || commander.LossesCount != 0)
            {
                winsText.gameObject.SetActive(true);
                lossText.gameObject.SetActive(true);
                winsText.text = "W" + commander.WinsCount;
                lossText.text = "L" + commander.LossesCount;
            }
        }
        else // if locked
        {
            background.color = new Color(.5f, .5f, .5f);
            winsText.gameObject.SetActive(false);
            lossText.gameObject.SetActive(false);
        }

        // Button
        btn.onClick.AddListener(onClickAction);
        btn.interactable = !commander.Locked;

        // Face attributes
        hair.sprite = commander.Portrait.Hair.sprite;
        eyes.sprite = commander.Portrait.Eyes.sprite;
        mouth.sprite = commander.Portrait.Mouth.sprite;

        // hide the face if commander is locked
        face.SetActive(!commander.Locked);
    }
}
