using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    RTSCameraController playerCamera = null;


    // Start is called before the first frame update
    void Start()
    {
        if(playerCamera == null)
        {
            Debug.LogError("Save Manager: Player Camera is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Saving
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Save();
        }
        //Loading
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Load();
        }
    }

    public void Save()
    {
        print("Saving");
        SerializationManager.Save("Save", SaveData.current);
    }

    public void Load()
    {
        print("Loading");
        SaveData.current = (SaveData)SerializationManager.Load(Application.dataPath + "/Saves/Save.ACBSAVE");
    }
}
