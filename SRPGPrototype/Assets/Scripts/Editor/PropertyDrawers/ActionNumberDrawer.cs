using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ActionNumber))]
public class ActionNumberDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var type = (ActionNumber.Type)property.FindPropertyRelative("type").enumValueIndex;
        int numAdditionalControls = 0;
        float addedHeight = 0;
        if(type == ActionNumber.Type.Constant)
        {
            numAdditionalControls += 1;
        }
        else if(type == ActionNumber.Type.UserStat || type == ActionNumber.Type.TargetStat)
        {
            addedHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("unitNumber"));
        }
        else
        {
            numAdditionalControls += 6;
        }
        return (EditorGUIUtility.singleLineHeight + 1) * (2 + numAdditionalControls) + addedHeight;
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
        var type = (ActionNumber.Type)typeProp.enumValueIndex;
        UIRect.y += lineHeight;
        if(type == ActionNumber.Type.Constant)
        {
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("constant"));
            UIRect.y += lineHeight;
        }
        else if(type == ActionNumber.Type.UserStat || type == ActionNumber.Type.TargetStat)
        {
            var unitNumProp = property.FindPropertyRelative("unitNumber");
            EditorGUI.PropertyField(UIRect, unitNumProp);
            UIRect.y += EditorGUI.GetPropertyHeight(unitNumProp);
        }
        else // Do min/max/mod/mul code
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
            EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("negateNumber"));
            UIRect.y += lineHeight;
        }

        #endregion

        EditorGUI.EndProperty();
    }
}
