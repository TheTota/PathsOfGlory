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

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }    
    }

    /// <summary>
    /// Generates a random portrait with any of the available Portrait Elements.
    /// </summary>
    /// <returns></returns>
    public Portrait GenerateRandomAIPortrait()
    {
        return GeneratePortrait(availableHair, availableEyes, availableMouth);
    }

    /// <summary>
    /// Generates a random portrait with the available and UNLOCKED Portraits Elements.
    /// </summary>
    /// <returns></returns>
    public Portrait GenerateRandomPlayerPortrait()
    {
        return GeneratePortrait(GetUnlockedHair(), GetUnlockedEyes(), GetUnlockedMouth());
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
        return new Portrait(hairPool[Random.Range(0, hairPool.Length)], eyesPool[Random.Range(0, eyesPool.Length)], mouthPool[Random.Range(0, mouthPool.Length)]);
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
