using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUIManager : MonoBehaviour
{
    public GameObject unitsRecapPanel;
    public Animator helperAnimator;

    private bool b;

    public void SwitchUnitsRecapUIVisibility()
    {
        helperAnimator.SetBool("Opened", !helperAnimator.GetBool("Opened"));
    }

    public void CloseUnitsRecapUI()
    {
        b = helperAnimator.GetBool("Opened");
        helperAnimator.SetBool("Opened", false);
    }

    public void OpenUnitsRecapIfWasOpened()
    {
        if (b)
        {
            helperAnimator.SetBool("Opened", true);
        }
    }

}
