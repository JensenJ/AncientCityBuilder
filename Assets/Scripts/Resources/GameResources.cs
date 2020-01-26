using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources
{
    public static event EventHandler OnGoldAmountChanged;

    private static int goldAmount;

    public static void AddGoldAmount(int amount)
    {
        goldAmount += amount;
        OnGoldAmountChanged?.Invoke(null, EventArgs.Empty);
    }

    public static int GetGoldAmount()
    {
        return goldAmount;
    }
}
