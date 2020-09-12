using UnityEngine;
using UnityEngine.UI;

public class PatternDisplayUI : MonoBehaviour
{
    public GridLayoutGroup layout;
    public RectTransform rectTransform;
    public GameObject emptyIconPrefab;
    public void Show(Pattern p, GameObject iconPrefab, Color color)
    {
        Hide();
        layout.constraintCount = p.Dimensions.x;
        var offsets = p.OffsetsSet;
        for(int x = 0; x < p.Dimensions.x; ++x)
        {
            for(int y = 0; y < p.Dimensions.y; ++y)
            {
                if (offsets.Contains(new Vector2Int(x, y)))
                {
                    var img = Instantiate(iconPrefab, layout.transform).GetComponent<Image>();
                    if (img != null)
                        img.color = color;
                }
                else
                    Instantiate(emptyIconPrefab, layout.transform);
            }
        }
        layout.transform.localPosition = new Vector3(0, (rectTransform.rect.height / 2) - (((layout.cellSize.y + layout.spacing.y) / 2) * p.Dimensions.y), 0);
    }

    public void Hide()
    {
        layout.transform.DestroyAllChildren();
    }
}
