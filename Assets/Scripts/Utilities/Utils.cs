using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Utils
{
    //Function to create world text with default settings
    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), 
        int fontsize = 40, Color? color = null, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center)
    {
        if(color == null)
        {
            color = Color.white;
        }
        return CreateWorldText(parent, text, localPosition, fontsize, (Color)color, textAlignment);
    }

    //Function to create world text where settings have to be defined.
    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition,
        int fontsize, Color color, TextAlignmentOptions textAlignment)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.Rotate(new Vector3(90, -90, 0));
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontsize;
        textMesh.color = color;
        return textMesh;
    }

    //Function to return the raycast hit data from the current mouse position, only on layers specified
    public static RaycastHit GetMousePositionRaycastData(Camera cam, LayerMask layerToCheck)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerToCheck);
        return hit;
    }
}
