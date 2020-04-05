using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stores all the available portrait elements and also allows to generate random portraits.
/// </summary>
public class PortraitGenerator : MonoBehaviour
{
    public static PortraitGenerator Instance { get; set; }

    // Every graphical portrait element created
    public PortraitElement[] availableHair;
    public PortraitElement[] availableEyes;
    public PortraitElement[] availableMouth;

    public Color[] availableSkinTones;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Inits the portrait elements (whether they're locked or not) for the rest of game by loading or generating & saving.
    /// </summary>
    public void InitPortraitElements()
    {
        // Put every element in a list
        List<PortraitElement> everyElement = new List<PortraitElement>();
        everyElement.AddRange(availableHair);
        everyElement.AddRange(availableEyes);
        everyElement.AddRange(availableMouth);

        // go through each element to init it 
        for (int i = 0; i < everyElement.Count; i++)
        {
            // if we have saves for portrait elements 
            if (PlayerPrefs.HasKey("portrait_elements_saves"))
            {
                everyElement[i].locked = PlayerPrefs.GetInt(everyElement[i].name) == 1 ? true : false;
            }
            else // if we have NO saves
            {
                // on new game, unlock first element of each category
                if (everyElement[i] == availableHair[0] || everyElement[i] == availableEyes[0] || everyElement[i] == availableMouth[0])
                {
                    everyElement[i].locked = false;
                    PlayerPrefs.SetInt(everyElement[i].name, 0);
                }
                else
                {
                    everyElement[i].locked = true;
                    PlayerPrefs.SetInt(everyElement[i].name, 1);
                }
            }
        }

        // if we didnt have saves before, now we do
        if (!PlayerPrefs.HasKey("portrait_elements_saves"))
        {
            PlayerPrefs.SetInt("portrait_elements_saves", 1);
            Debug.Log("Portrait elements initialized and saved.");
        }
        else
        {
            Debug.Log("Portrait elements loaded.");
        }
    }

    /// <summary>
    /// Generates a random portrait with the available and UNLOCKED Portraits Elements.
    /// </summary>
    /// <returns></returns>
    public Portrait GenerateRandomPortraitFromUnlockedElements()
    {
        return GeneratePortrait(GetUnlockedHair(), GetUnlockedEyes(), GetUnlockedMouth());
    }

    /// <summary>
    /// Generates a random portrait with the available and UNLOCKED Portraits Elements + adds the given element to the portrait.
    /// </summary>
    /// <returns></returns>
    public Portrait GenerateRandomPortraitFromUnlockedElementsAndWithGivenElement(PortraitElement elt)
    {
        Portrait p = GenerateRandomPortraitFromUnlockedElements();

        // Replace element by given one
        if (availableHair.Contains(elt))
        {
            p.Hair = elt;
        }
        else if (availableEyes.Contains(elt))
        {
            p.Eyes = elt;
        }
        else if (availableMouth.Contains(elt))
        {
            p.Mouth = elt;
        }

        return p;
    }

    /// <summary>
    /// Generates a portrait from the given pools.
    /// </summary>
    /// <param name="hairPool"></param>
    /// <param name="eyesPool"></param>
    /// <param name="mouthPool"></param>
    /// <returns></returns>
    private Portrait GeneratePortrait(PortraitElement[] hairPool, PortraitElement[] eyesPool, PortraitElement[] mouthPool)
    {
        return new Portrait(
            hairPool[UnityEngine.Random.Range(0, hairPool.Length)],
            eyesPool[UnityEngine.Random.Range(0, eyesPool.Length)],
            mouthPool[UnityEngine.Random.Range(0, mouthPool.Length)],
            availableSkinTones[UnityEngine.Random.Range(0, availableSkinTones.Length)]
        );
    }

    /// <summary>
    /// Returns the available unlocked hair.
    /// </summary>
    /// <returns></returns>
    public PortraitElement[] GetUnlockedHair()
    {
        return availableHair.Where(x => !x.locked).ToArray();
    }

    /// <summary>
    /// Returns the available unlocked eyes.
    /// </summary>
    /// <returns></returns>
    public PortraitElement[] GetUnlockedEyes()
    {
        return availableEyes.Where(x => !x.locked).ToArray();
    }

    /// <summary>
    /// Returns the available unlocked mouth.
    /// </summary>
    /// <returns></returns>
    public PortraitElement[] GetUnlockedMouth()
    {
        return availableMouth.Where(x => !x.locked).ToArray();
    }
}
