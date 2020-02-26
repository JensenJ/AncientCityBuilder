using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

public class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion quaterion = (Quaternion)obj;
        info.AddValue("x", quaterion.x);
        info.AddValue("y", quaterion.y);
        info.AddValue("z", quaterion.z);
        info.AddValue("w", quaterion.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion quaternion = (Quaternion)obj;
        quaternion.x = (float)info.GetValue("x", typeof(float));
        quaternion.y = (float)info.GetValue("y", typeof(float));
        quaternion.z = (float)info.GetValue("z", typeof(float));
        quaternion.w = (float)info.GetValue("w", typeof(float));
        obj = quaternion;
        return obj;
    }
}
