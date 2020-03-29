using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Version : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<TextMeshProUGUI>().text = Application.version;
    }
}
