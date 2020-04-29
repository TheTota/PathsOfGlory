using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUIManager : MonoBehaviour
{
    public GameObject unitsRecapPanel;
    public Animator helperAnimator;

    public void SwitchUnitsRecapUIVisibility()
    {
        helperAnimator.SetBool("Opened", !helperAnimator.GetBool("Opened"));
    }

}
