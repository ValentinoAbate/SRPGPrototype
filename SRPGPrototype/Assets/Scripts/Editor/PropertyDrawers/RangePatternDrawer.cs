using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangePattern))]
public class RangePatternDrawer : PropertyDrawer
{
    const float labelWidth = 100;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        var typeProp = property.FindPropertyRelative("patternType");
        var type = (RangePattern.Type)typeProp.enumValueIndex;
        if (type == RangePattern.Type.Generated)
            return lineHeight * 3;
        if (type == RangePattern.Type.Pattern)
            return lineHeight * 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("pattern"));
        // Type doesn't need extra lines
        return lineHeight * 2;
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
        var type = (RangePattern.Type)typeProp.enumValueIndex;

        #region Actual GUI drawing
        GUI.Label(UIRect, label, EditorUtils.Centered);
        UIRect.y += lineHeight;

        EditorGUI.PropertyField(UIRect, typeProp);
        UIRect.y += lineHeight;
        if(type == RangePattern.Type.Generated)
        {
            var generatorProp = property.FindPropertyRelative("generator");
            EditorGUI.PropertyField(UIRect, generatorProp);
            UIRect.y += lineHeight;
        }
        else if(type == RangePattern.Type.Pattern)
        {
            var patternProp = property.FindPropertyRelative("pattern");
            EditorGUI.PropertyField(UIRect, patternProp);
        }

        #endregion

        EditorGUI.EndProperty();
    }
}
