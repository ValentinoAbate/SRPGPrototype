using UnityEngine;
using UnityEngine.UI;

public class ShellPatternDisplayUI : MonoBehaviour
{
    public GridLayoutGroup layout;
    public RectTransform rectTransform;
    public GameObject blockedIconPrefab;
    public GameObject emptyIconPrefab;
    public GameObject programIconPrefab;
    public GameObject programIconPrefabFixed;
    public void Show(Shell s)
    {
        var p = s.CustArea;
        Hide();
        layout.constraintCount = p.Dimensions.x;
        var offsets = p.OffsetsSet;
        var installMap = s.InstallMap ?? GenerateIntallMapFromShellAsset(s);
        for(int x = 0; x < p.Dimensions.x; ++x)
        {
            for(int y = 0; y < p.Dimensions.y; ++y)
            {
                // Pattern is blocked
                if(!offsets.Contains(new Vector2Int(x, y)))
                {
                    Instantiate(blockedIconPrefab, layout.transform);
                }
                else if (installMap[x, y] != null)
                {
                    var prog = installMap[x, y];
                    var prefab = prog.attributes.HasFlag(Program.Attributes.Fixed) ? programIconPrefabFixed : programIconPrefab;
                    var img = Instantiate(prefab, layout.transform).GetComponent<Image>();
                    if (img != null)
                        img.color = prog.ColorValue;
                }
                else
                {
                    Instantiate(emptyIconPrefab, layout.transform);
                }
            }
        }
        layout.transform.localPosition = new Vector3(0, (rectTransform.rect.height / 2) - (((layout.cellSize.y + layout.spacing.y) / 2) * p.Dimensions.y), 0);
    }

    private Program[,] GenerateIntallMapFromShellAsset(Shell s)
    {
        var map = new Program[s.CustArea.Dimensions.x, s.CustArea.Dimensions.y];
        foreach(var install in s.preInstalledPrograms)
        {
            var positions = install.program.shape.OffsetsShifted(install.location, false);
            foreach (var pos in positions)
                map[pos.x, pos.y] = install.program;
        }
        return map;
    }

    public void Hide()
    {
        layout.transform.DestroyAllChildren();
    }
}
