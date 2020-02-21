using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//Requires an image
[RequireComponent(typeof(Image))]
//Implements mouse cursor events
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    //Tabgroup and image
    public TabGroup tabGroup;
    public Image background;

    //Callback events
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    //When tab selected
    public void OnPointerClick(PointerEventData eventData)
    {
        //Manage on tab group
        tabGroup.OnTabSelected(this);
    }

    //When tab hovered over
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Manage on tab group
        tabGroup.OnTabEnter(this);
    }

    //When tab not hovered over
    public void OnPointerExit(PointerEventData eventData)
    {
        //Manage on tab group
        tabGroup.OnTabExit(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Button setup
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }
    //Selection callback
    public void Select()
    {
        if (onTabSelected != null)
        {
            //Invoke callback
            onTabSelected.Invoke();
        }
    }

    //Deselection callback

    public void Deselect()
    {
        if(onTabDeselected != null)
        {
            //Invoke callback
            onTabDeselected.Invoke();
        }
    }
}
