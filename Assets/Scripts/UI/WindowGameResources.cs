using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindowGameResources : MonoBehaviour
{
    private void Awake()
    {
        GameResources.onGoldAmountChanged += delegate (object sender, EventArgs e)
        {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }

    private void UpdateResourceTextObject()
    {
        transform.Find("GoldAmount").GetComponent<TextMeshProUGUI>().text = "Gold: " + GameResources.GetGoldAmount();
    }
}
