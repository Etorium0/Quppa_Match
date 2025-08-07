using System.Collections;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles : Editor
{
    [MenuItem("Assets/Build Asset Bundles To AssetBundles")]
    static void ExportBundleToAssetBundles()
    {

#if UNITY_ANDROID
        string bundlePathAndroid = "/../AssetBundles/Android";
        string targetPath = Application.dataPath + bundlePathAndroid;
        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.None, BuildTarget.Android);
#elif UNITY_IOS
        string bundlePathiOS = "Assets/AssetBundles/iOS";
        BuildPipeline.BuildAssetBundles(bundlePathiOS, BuildAssetBundleOptions.None, BuildTarget.iOS);
#endif
    }

    [MenuItem("Assets/Build Asset Bundles To StreamingAssets")]
    static void ExportBundleToStreamingAssets()
    {
        string bundlePath = "Assets/StreamingAssets";
        BuildPipeline.BuildAssetBundles(bundlePath, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Assets/Delete All AssetBundles In Cache")]
    static void DeleteAllAssetBundlesInCache()
    {
        if (UnityEngine.Caching.ClearCache())
        {
            UnityEngine.Debug.Log("Successfully cleaned the cache.");
        }
        else
        {
            UnityEngine.Debug.Log("Cache is being used.");
        }
    }

}