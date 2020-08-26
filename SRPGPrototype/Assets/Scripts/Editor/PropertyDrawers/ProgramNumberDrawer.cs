using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ProgramNumber))]
public class ProgramNumberDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var type = (ProgramNumber.Type)property.FindPropertyRelative("type").enumValueIndex;
        int numAdditionalControls = 0;
        if(type == ProgramNumber.Type.Constant || type == ProgramNumber.Type.NumberOfInstallsAtrribute 
            || type == ProgramNumber.Type.NumberOfInstallsColor 
            || type == ProgramNumber.Type.NumberOfInstallsRarity)
        {
            numAdditionalControls += 1;
        }
        if (type != ProgramNumber.Type.Constant)
            numAdditionalControls += 5;
        return (EditorGUIUtility.singleLineHeight + 1) * (2 + numAdditionalControls);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);


        //Initialize properties
        var typeProp = property.FindPropertyRelative("type");
        //Initialize UI variables
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

        #region Actual GUI drawing
        GUI.Label(UIRect, label, EditorUtils.Centered);
        UIRect.y += lineHeight;
        EditorGUI.PropertyField(UIRect, typeProp);
        var type = (ProgramNumber.Type)typeProp.enumValueIndex;
        UIRect.y += lineHeight;
        if(type == ProgramNumber.Type.Constant)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("constant"));
            UIRect.y += lineHeight;
        }
        else if(type == ProgramNumber.Type.NumberOfInstallsAtrribute)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("attributes"));
            UIRect.y += lineHeight;
        }
        else if(type == ProgramNumber.Type.NumberOfInstallsColor)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("color"));
            UIRect.y += lineHeight;
        }
        else if(type == ProgramNumber.Type.NumberOfInstallsRarity)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("rarity"));
            UIRect.y += lineHeight;
        }
        // Do min/max/mod/mul code
        if(type != ProgramNumber.Type.Constant)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("min"));
            UIRect.y += lineHeight;
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("max"));
            UIRect.y += lineHeight;
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("baseAmount"));
            UIRect.y += lineHeight;
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("modifier"));
            UIRect.y += lineHeight;
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("multiplier"));
            UIRect.y += lineHeight;
        }

        #endregion

        EditorGUI.EndProperty();
    }
}
