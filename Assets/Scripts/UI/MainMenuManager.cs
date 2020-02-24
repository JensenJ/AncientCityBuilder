using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        //if(Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && isOnMenu == false)
        //{
        //    isOnMenu = true;
        //    if (handleUIEnabledStatus)
        //    {
        //        tabSystemRef.SetActive(true);
        //    }
        //    if (handleUIDisabledStatus)
        //    {
        //        pressAnyKeyRef.SetActive(false);
        //    }
        //    onEnterMenu?.Invoke();
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape) && isOnMenu == true)
        //{
        //    isOnMenu = false;
        //    if (handleUIEnabledStatus)
        //    {
        //        pressAnyKeyRef.SetActive(true);
        //    }
        //    if (handleUIDisabledStatus)
        //    {
        //        tabSystemRef.SetActive(false);
        //    }
        //    onExitMenu?.Invoke();
        //}

        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            if (handleUIEnabledStatus)
            {
                tabSystemRef.SetActive(true);
            }
            if (handleUIDisabledStatus)
            {
                pressAnyKeyRef.SetActive(false);
            }
            onEnterMenu?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (handleUIEnabledStatus)
            {
                pressAnyKeyRef.SetActive(true);
            }
            if (handleUIDisabledStatus)
            {
                tabSystemRef.SetActive(false);
            }
            onExitMenu?.Invoke();
        }

    }
}
