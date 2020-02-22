using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    //Settings
    [SerializeField] List<TabButton> tabButtons = null;
    [SerializeField] Color tabIdle = new Color();
    [SerializeField] Color tabHover = new Color();
    [SerializeField] Color tabActive = new Color();
    [SerializeField] TabButton selectedTab = null;
    [SerializeField] bool disableTabsOnSwitch = true;
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
        ResetTabs();
        //Change to hover colour
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = tabHover;
        }
    }

    //When the tab is no longer hovered over
    public void OnTabExit(TabButton button)
    {
        //Reset tabs
        ResetTabs();
    }

    //When the tab is selected
    public void OnTabSelected(TabButton button)
    {
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
        ResetTabs();
        //Set to active colour
        button.background.color = tabActive;
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
    public void ResetTabs()
    {
        //For every tab button
        foreach(TabButton button in tabButtons)
        {
            //If this button is selected, skip it
            if(selectedTab != null && button == selectedTab) { continue; }
            //Set to idle colour
            button.background.color = tabIdle;
        }
    }
}
