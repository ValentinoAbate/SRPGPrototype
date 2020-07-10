using UnityEditor;
using SerializableCollections.EditorGUIUtils;

[CustomEditor(typeof(TileUI))]
public class TileUIInspector : Editor
{
    private TileUI data;
    private const float labelWidth = 160;
    private void OnEnable()
    {
        data = target as TileUI;
    }
    public override void OnInspectorGUI()
    {
        Undo.RecordObject(data, data.name);
        data.tilePrefabs.DoGUILayout((key) => data.tilePrefabs.KeyGUIFixedWidth(key, labelWidth),
            data.tilePrefabs.ValueGUIObj, data.tilePrefabs.EnumAddGUIVal, "Tile UI Prefabs", true);
    }
}
