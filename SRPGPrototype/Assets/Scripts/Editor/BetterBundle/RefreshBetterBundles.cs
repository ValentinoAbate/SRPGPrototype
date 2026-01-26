using UnityEditor;

public static class RefreshBetterBundles
{
    public const string path = "Assets/ScriptableObjects/Bundles";

    [MenuItem("Tools/Asset/Refresh BetterBundles", priority = -999)]
    static void DoRefreshBetterBundles()
    {
        BetterBundleLoaderUtils.LoadBundles(path);
    }
}
