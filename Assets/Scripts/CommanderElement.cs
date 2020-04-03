using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to define cleanly every commander in the game.
/// Those files will be used to init the commanders.
/// </summary>
[CreateAssetMenu(fileName = "New Commander Element", menuName = "CommanderElement")]
public class CommanderElement : ScriptableObject
{
    public string commanderName;
    public bool Locked;
    public Color Color;
    public AIType AiType;
    public PortraitElement PortraitElementToUnlock;

    // reaction lines

    [Header("Lignes Bataille")]
    /// <summary>Debut bataille - Premiere bataille</summary>
    public string preBattleFirstTimeLine;
    
    /// <summary>Debut bataille - A gagné la dernière</summary>
    public string[] preBattleWonLastLines;
    
    /// <summary>Debut bataille - A perdu la dernière</summary>
    public string[] preBattleLostLastLines;

    /// <summary>Fin bataille - A gagné</summary>
    public string[] postBattleWinLines;

    /// <summary>Fin bataille - A perdu</summary>
    public string[] postBattleLossLines;

    [Header("Lignes Affrontement Unites")]
    /// <summary>Fin affrontement unités - A perdu</summary>
    public string[] postUnitsFightLossLines;

    /// <summary>Fin affrontement unités - A gagné</summary>
    public string[] postUnitsFightWinLines;

    /// <summary>Fin affrontement unités - Egalité</summary>
    public string[] postUnitsFightDrawLines;
}
