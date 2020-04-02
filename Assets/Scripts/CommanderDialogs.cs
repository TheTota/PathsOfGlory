using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CommanderDialogs
{
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

    /// <summary>Fin affrontement unités - A perdu</summary>
    public string[] postUnitsFightLossLines;
    /// <summary>Fin affrontement unités - A gagné</summary>
    public string[] postUnitsFightWinLines;
    /// <summary>Fin affrontement unités - Egalité</summary>
    public string[] postUnitsFightDrawLines;

    /// <summary>
    /// Used by the tutorial guy.
    /// </summary>
    int dialogIndex = 0;

    public CommanderDialogs(string preBattleFirstTimeLine, string[] preBattleWonLastLines, string[] preBattleLostLastLines, string[] postBattleWinLines, string[] postBattleLossLines, string[] postUnitsFightLossLines, string[] postUnitsFightWinLines, string[] postUnitsFightDrawLines)
    {
        this.preBattleFirstTimeLine = preBattleFirstTimeLine;
        this.preBattleWonLastLines = preBattleWonLastLines;
        this.preBattleLostLastLines = preBattleLostLastLines;

        this.postBattleWinLines = postBattleWinLines;
        this.postBattleLossLines = postBattleLossLines;

        this.postUnitsFightLossLines = postUnitsFightLossLines;
        this.postUnitsFightWinLines = postUnitsFightWinLines;
        this.postUnitsFightDrawLines = postUnitsFightDrawLines;
    }

    public void Init()
    {
        this.dialogIndex = 0;
    }

    internal string GetRandomPreBattleWonLastLine()
    {
        return preBattleWonLastLines[Random.Range(0, preBattleWonLastLines.Length)];
    }

    internal string GetRandomPreBattleLostLastLine()
    {
        return preBattleLostLastLines[Random.Range(0, preBattleLostLastLines.Length)];
    }

    internal string GetRandomPostBattleWinLine()
    {
        return postBattleWinLines[Random.Range(0, postBattleWinLines.Length)];
    }

    internal string GetRandomPostBattleLossLine()
    {
        return postBattleLossLines[Random.Range(0, postBattleLossLines.Length)];
    }

    /// <summary>
    /// Used for the tutorial. To use it : fill commander element field "post units fight loss lines" with tutorial lines in the right order.
    /// Call this function to print a line and incr the index.
    /// </summary>
    /// <returns></returns>
    internal string GetNextTutorialLine()
    {
        return postUnitsFightLossLines[dialogIndex++];
    }

    internal string GetRandomPostUnitsFightWinLine()
    {
        return postUnitsFightWinLines[Random.Range(0, postUnitsFightWinLines.Length)];
    }

    internal string GetRandomPostUnitsFightLossLine()
    {
        return postUnitsFightLossLines[Random.Range(0, postUnitsFightLossLines.Length)];
    }

    internal string GetRandomPostUnitsFightDrawLine()
    {
        return postUnitsFightDrawLines[Random.Range(0, postUnitsFightDrawLines.Length)];
    }
}