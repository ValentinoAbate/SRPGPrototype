using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableCollections;
using System.Linq;

public class TileUI : MonoBehaviour
{
    public enum Type
    {
        Empty,
        CustWhite,
        CustGreen,
        CustBlue,
        CustRed,
        CustBlocked,
        CustYellow,
    }

    public ObjDict tilePrefabs;
    public ColorDict colors = new ColorDict();
    private Dictionary<Vector2Int, List<GameObject>> tiles = new Dictionary<Vector2Int, List<GameObject>>();

    public bool HasActiveTileUI(Vector2Int p)
    {
        if(tiles.ContainsKey(p))
        {
            var tile = tiles[p];
            if(tile == null)
            {
                ClearTile(p);
                return false;
            }
            return true;
        }
        return false;
    }

    public Entry SpawnTileUI(Vector2Int p, Type t, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        var obj = Instantiate(tilePrefabs[t]);
        var qMesh = obj.GetComponent<QuadMesh>();
        qMesh.SetMesh(v1, v2, v3, v4);
        if(colors.ContainsKey(t))
        {
            var rend = obj.GetComponent<MeshRenderer>().material;
            rend.SetColor("ColorMain", colors[t]);
        }
        if (tiles.ContainsKey(p))
            tiles[p].Add(obj);
        else
            tiles.Add(p, new List<GameObject> { obj });
        return new Entry { obj = obj, pos = p, type = t };
    }

    public void Remove(Entry entry)
    {
        var p = entry.pos;
        if (!tiles.ContainsKey(p))
            return;
        var list = tiles[p];
        var tileInd = list.FindIndex((i) => i == entry.obj);
        if (tileInd == -1)
            return;
        if (list.Count <= 1)
        {
            ClearTile(p);
            return;
        }
        list.RemoveAt(tileInd);
        Destroy(entry.obj);
    }

    private void ClearTile(Vector2Int p)
    {
        foreach (var obj in tiles[p])
            Destroy(obj);
        tiles.Remove(p);
    }

    public struct Entry
    {
        public GameObject obj;
        public Vector2Int pos;
        public Type type;
    }

    [System.Serializable] public class ObjDict : SDictionary<Type, GameObject> { };
    [System.Serializable] public class ColorDict : SDictionary<Type, Color> { };
}
