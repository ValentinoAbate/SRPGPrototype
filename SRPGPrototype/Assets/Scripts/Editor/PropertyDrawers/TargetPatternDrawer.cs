using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TargetPattern))]
public class TargetPatternDrawer : PropertyDrawer
{
    const float labelWidth = 100;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        var typeProp = property.FindPropertyRelative("patternType");
        var type = (TargetPattern.Type)typeProp.enumValueIndex;
        if (type == TargetPattern.Type.Simple || type == TargetPattern.Type.Self)
            return lineHeight * 2;
        if (type == TargetPattern.Type.Generated)
            return lineHeight * 3;
        // Type is pattern
        return lineHeight * 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("pattern"));
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);


        //Initialize properties
        var typeProp = property.FindPropertyRelative("patternType");
        //Initialize UI variables
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
        // Initialize local vars
        var type = (TargetPattern.Type)typeProp.enumValueIndex;

        #region Actual GUI drawing
        GUI.Label(UIRect, label, EditorUtils.Centered);
        UIRect.y += lineHeight;
        //EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent("Pattern Type"));
        //EditorGUI.PropertyField(new Rect(UIRect) { x = UIRect.x + labelWidth, width = UIRect.width - labelWidth }, typeProp, GUIContent.None);
        EditorGUI.PropertyField(UIRect, typeProp);
        UIRect.y += lineHeight;
        if(type == TargetPattern.Type.Generated)
        {
            var generatorProp = property.FindPropertyRelative("generator");
            EditorGUI.PropertyField(UIRect, generatorProp);
            UIRect.y += lineHeight;
        }
        else if(type == TargetPattern.Type.Pattern)
        {
            var patternProp = property.FindPropertyRelative("pattern");
            EditorGUI.PropertyField(UIRect, patternProp);
        }

        #endregion

        EditorGUI.EndProperty();
    }
}
