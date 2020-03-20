using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PortraitRenderer : MonoBehaviour
{
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

    /// <summary>
    /// Renders the portrait with the given values.
    /// </summary>
    /// <param name="commanderColor"></param>
    /// <param name="onClickAction"></param>
    /// <param name="hairSprite"></param>
    /// <param name="eyesSprite"></param>
    /// <param name="mouthSprite"></param>
    public void RenderPortrait(Color commanderColor, UnityAction onClickAction, Portrait portrait, bool locked)
    {
        // Color portrait
        if (!locked)
        {
            background.color = commanderColor;
        }
        else
        {
            background.color = new Color(.5f, .5f, .5f);
        }

        // Button
        btn.onClick.AddListener(onClickAction);
        btn.interactable = !locked;

        // Face attributes
        hair.sprite = portrait.Hair.sprite;
        eyes.sprite = portrait.Eye.sprite;
        mouth.sprite = portrait.Mouth.sprite;

        face.SetActive(!locked);
    }
}
