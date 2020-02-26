using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

public class SerializationManager : MonoBehaviour
{
    //Function to save data
    public static bool Save(string saveName, object saveData)
    {
        //Create formatter
        BinaryFormatter formatter = GetBinaryFormatter();
        if (!Directory.Exists(Application.dataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }

        //file path
        string path = Application.dataPath + "/Saves/" + saveName + ".ACBSAVE";

        //File handling
        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);
        file.Close();

        //was successful
        return true;
    }

    //Function to load data
    public static object Load(string path)
    {
        //Check if file exists
        if (!File.Exists(path))
        {
            return null;
        }

        //Create formatter
        BinaryFormatter formatter = GetBinaryFormatter();
        //File handling
        FileStream file = File.Open(path, FileMode.Open);
        try
        {
            //If successful
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            //If not successful
            Debug.LogError("Failed to load file at " + path);
            file.Close();
            return null;
        }
    }

    //Creates a binary formatter
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        //Surrogates
        SurrogateSelector selector = new SurrogateSelector();

        //Init surrogates
        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();

        //Add surrogates
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

        //Apply surrogates
        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
