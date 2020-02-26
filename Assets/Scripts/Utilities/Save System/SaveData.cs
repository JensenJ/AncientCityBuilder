using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Reference
    private static SaveData _current;
    //Singleton
    public static SaveData current
    {
        //Getter
        get
        {
            if(_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
        //Setter
        set
        {
            if(value != null)
            {
                _current = value;
            }
        }
    }

    //Values to store
    public Vector3 cameraPosition;
    public float cameraOrthographicSize;
}
