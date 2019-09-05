// ---------------------------------------------------------------------------- 
// Author: Nate Wilson
// Date:   9/5/2019
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System;

namespace MyBox
{
    public class BrowseAssetsAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BrowseAssetsAttribute))]
public class BrowseAssetsDrawer : PropertyDrawer
{
    private bool _checked;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            _checked = false;
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        string name = label.text;
        string path = property.stringValue;

        Rect textfieldBox = new Rect();
        textfieldBox = position;
        textfieldBox.width = position.width * .8f;
        Rect buttonBox = new Rect();
        buttonBox = position;
        buttonBox.x = position.width * .85f;
        buttonBox.width = position.width - buttonBox.x;
        if (!path.Contains(Application.dataPath))
        {
            path = Application.dataPath;
        }
        string truncatedPath = path.Substring(Application.dataPath.Length);
        string filepath = Application.dataPath + EditorGUI.TextField(textfieldBox, name, truncatedPath);
        if (GUI.Button(buttonBox, "Browse"))
        {
            filepath = EditorUtility.SaveFolderPanel(name, path, "Select Folder");
            if (!filepath.Contains(Application.dataPath))
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format("Please choose a folder that is in this Assets Folder. 0}", e));
                }

               property.stringValue = path;
                    return;

            }
        }
        property.stringValue = filepath;
        }
    }   
#endif
}