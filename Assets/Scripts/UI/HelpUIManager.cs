using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUIManager : MonoBehaviour
{
    public GameObject unitsRecapPanel;

    public void SwitchUnitsRecapUIVisibility()
    {
        unitsRecapPanel.SetActive(!unitsRecapPanel.activeInHierarchy);
    }

}
