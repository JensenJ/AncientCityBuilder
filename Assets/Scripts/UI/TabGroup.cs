using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    //Settings
    [SerializeField] List<TabButton> tabButtons = null;
    [SerializeField] Color tabIdleColour = new Color();
    [SerializeField] Color tabHoverColour = new Color();
    [SerializeField] Color tabActiveColour = new Color();
    [SerializeField] TabButton selectedTab = null;
    [SerializeField] bool disableTabsOnSwitch = true;
    [SerializeField] bool canReselectTabs = true;
    [SerializeField] List<GameObject> objectsToSwap = null;

    //Function called on all tabbuttons to add them to this grouping
    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    //When the tab is hovered over
    public void OnTabEnter(TabButton button)
    {
        //Reset tabs
        ResetTabColours();
        //Change to hover colour
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = tabHoverColour;
        }
    }

    //When the tab is no longer hovered over
    public void OnTabExit(TabButton button)
    {
        //Reset tabs
        ResetTabColours();
    }

    //When the tab is selected
    public void OnTabSelected(TabButton button)
    {
        //If tabs cannot be reselected
        if (!canReselectTabs)
        {
            //If new tab is equal to current tab
            if(button == selectedTab)
            {
                //return out of function
                return;
            }
        }

        //Callback on deselect
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        //Assign selected tab
        selectedTab = button;

        //Select callback
        selectedTab.Select();

        //Reset tabs
        ResetTabColours();
        //Set to active colour
        button.background.color = tabActiveColour;
        //Get page index for that tab
        int index = button.transform.GetSiblingIndex();
        //Set page index active
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                if (disableTabsOnSwitch)
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }
    }

    //Function to reset tabs
    public void ResetTabColours()
    {
        //For every tab button
        foreach(TabButton button in tabButtons)
        {
            //If this button is selected, skip it
            if(selectedTab != null && button == selectedTab) { continue; }
            //Set to idle colour
            button.background.color = tabIdleColour;
        }
    }

    //Function to disable the tab system
    public void DisableTabSystem()
    {
        ResetTabColours();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            tabButtons[i].Deselect();
            objectsToSwap[i].SetActive(false);
        }
    }
}
