using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Pattern))]
public class PatternDrawer : PropertyDrawer
{
    const float labelWidth = 100;
    const float maxCheckWidth = 25;
    const int numFields = 2;
    const int numControlRows = 2;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var dimensionsProp = property.FindPropertyRelative("dimensions");
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        int gridRows = numControlRows + (dimensionsProp.vector2IntValue.y);
        float gridHeight = ((lineHeight + 5) * (gridRows - 1));
        return (lineHeight * (numFields + 1)) + gridHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);


        //Initialize properties
        var offsetsProp = property.FindPropertyRelative("patternOffsets");
        var dimensionsProp = property.FindPropertyRelative("dimensions");
        //Initialize UI variables
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

        #region Actual GUI drawing
        GUI.Label(UIRect, label, EditorUtils.Centered);
        UIRect.y += lineHeight;
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent("Dimensions"));
        EditorGUI.PropertyField(new Rect(UIRect) { x = UIRect.x + labelWidth, width = UIRect.width - labelWidth }, dimensionsProp, GUIContent.none);
        UIRect.y += lineHeight;
        if (GUI.Button(UIRect, new GUIContent("Fill Pattern")))
        {
            var dim = dimensionsProp.vector2IntValue;
            var vals = new bool[dim.x, dim.y];
            for (int x = 0; x < dim.x; ++x)
                for (int y = 0; y < dim.y; ++y)
                    vals[x, y] = true;
            SaveBoolArrayIntoOffsetArray(offsetsProp, vals, dim);
        }
        UIRect.y += lineHeight;
        if (GUI.Button(UIRect, new GUIContent("Clear Pattern")))
        {
            var dim = dimensionsProp.vector2IntValue;
            var vals = new bool[dim.x, dim.y];
            SaveBoolArrayIntoOffsetArray(offsetsProp, vals, dim);
        }
        UIRect.y += lineHeight;
        #region Draw Pattern
        var values = OffsetArrayToBoolArray(offsetsProp, dimensionsProp.vector2IntValue);

        float checkWidth = Mathf.Min(maxCheckWidth, position.width / values.GetLength(1));
        float checkStartX = position.x + ((position.width - checkWidth * values.GetLength(0)) / 2);
        Rect checkRect = new Rect(UIRect) { width = checkWidth };
        for (int y = values.GetLength(1) - 1; y >= 0; --y)
        {
            checkRect.x = checkStartX;
            for (int x = 0; x < values.GetLength(0); ++x)
            {
                values[x, y] = EditorGUI.Toggle(checkRect, values[x, y]);
                checkRect.x += checkWidth;
            }
            checkRect.y += lineHeight + 5;
        }
        UIRect.y = checkRect.y + lineHeight;
        if (GUI.changed)
            SaveBoolArrayIntoOffsetArray(offsetsProp, values, dimensionsProp.vector2IntValue);
        #endregion

        #endregion

        EditorGUI.EndProperty();
    }

    private bool[,] OffsetArrayToBoolArray(SerializedProperty offsetArrayProp, Vector2Int dimensions)
    {
        int rows = dimensions.y;
        int cols = dimensions.x;
        var values = new bool[cols, rows];
        for (int i = 0; i < offsetArrayProp.arraySize; ++i)
        {
            var offsetProp = offsetArrayProp.GetArrayElementAtIndex(i);
            int x = offsetProp.FindPropertyRelative("x").intValue;
            int y = offsetProp.FindPropertyRelative("y").intValue;
            if (x >= 0 && y >= 0 && y < rows && x < cols)
                values[x, y] = true;
        }
        return values;
    }

    private void SaveBoolArrayIntoOffsetArray(SerializedProperty offsetArrayProp, bool[,] values, Vector2Int dimensions)
    {
        offsetArrayProp.ClearArray();

        for (int x = 0; x < values.GetLength(0); ++x)
        {
            for (int y = 0; y < values.GetLength(1); ++y)
            {
                if (values[x, y])
                {
                    offsetArrayProp.InsertArrayElementAtIndex(0);
                    var offsetProp = offsetArrayProp.GetArrayElementAtIndex(0);
                    offsetProp.FindPropertyRelative("x").intValue = x;
                    offsetProp.FindPropertyRelative("y").intValue = y;
                }
            }
        }

    }
}
