using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JUCL.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pressAnyKeyRef = null;
    [SerializeField] private GameObject tabSystemRef = null;
    [SerializeField] bool handleUIEnabledStatus = true;
    [SerializeField] bool handleUIDisabledStatus = false;

    bool isOnMenu = false;

    public UnityEvent onEnterMenu;
    public UnityEvent onExitMenu;
  

    // Start is called before the first frame update
    void Start()
    {
        pressAnyKeyRef.SetActive(true);
        tabSystemRef.SetActive(false);
        isOnMenu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && isOnMenu == false)
        {
            isOnMenu = true;
            if (handleUIEnabledStatus)
            {
                SetStatusUIElement(tabSystemRef, true);
            }
            if (handleUIDisabledStatus)
            {
                SetStatusUIElement(pressAnyKeyRef, false);
            }
            onEnterMenu?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOnMenu == true)
        {
            isOnMenu = false;
            if (handleUIEnabledStatus)
            {
                SetStatusUIElement(pressAnyKeyRef, true);
            }
            if (handleUIDisabledStatus)
            {
                SetStatusUIElement(tabSystemRef, false);
            }
            onExitMenu?.Invoke();
        }
    }

    void SetStatusUIElement(GameObject UIElement, bool status)
    {
        //Try to get animator
        JUCLUIAnimator animator = UIElement.GetComponent<JUCLUIAnimator>();
        //If animator is present
        if (animator != null)
        {
            //Run disable animation
            if (status == true)
            {
                animator.Show();
            }
            else
            {
                animator.Disable();
            }
        }
        else
        {
            //Disable body window
            UIElement.SetActive(status);
        }
    }
}
