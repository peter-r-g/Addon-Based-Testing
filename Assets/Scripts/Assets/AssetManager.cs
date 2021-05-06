using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance;
    public Thread mainThread;

    public Dictionary<string, Asset> assets;
    private Stack<Asset> mainLoadedAssets;
    private Dictionary<Asset, bool> triedLoadingAssets;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        mainThread = Thread.CurrentThread;
        instance = this;
        assets = new Dictionary<string, Asset>();
        mainLoadedAssets = new Stack<Asset>();
        triedLoadingAssets = new Dictionary<Asset, bool>();
    }

    private void Update()
    {
        if (AddonManager.instance.Ready && mainLoadedAssets.Count > 0)
        {
            Asset asset = mainLoadedAssets.Pop();
            if (asset.Load())
            {
                Asset[] oldAssets = asset.source.assets;
                Asset[] newAssets = new Asset[oldAssets.Length+1];
                oldAssets.CopyTo(newAssets, 0);
                newAssets[oldAssets.Length] = asset;
                asset.source.assets = newAssets;
            }
            else
            {
                if (triedLoadingAssets.ContainsKey(asset))
                {
                    Debug.LogError($"{asset.assetType} failed to load too many times, Path: {asset.files[0]}");
                    triedLoadingAssets.Remove(asset);
                }
                else
                {
                    mainLoadedAssets.Push(asset);
                    triedLoadingAssets.Add(asset, true);
                }
            }
        }
    }

    public Asset GetAsset(string path)
    {
        try
        {
            return assets[path];
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            return null;
        }
    }

    public string GetRootFileByAsset(Asset asset)
    {
        foreach (KeyValuePair<string, Asset> pair in assets)
        {
            if (pair.Value == asset)
                return pair.Key;
        }

        return string.Empty;
    }

    public void AddAsset(string path, Asset asset)
    {
        string relativePath = AbsolutePathToRelative(path);
        asset.relativePath = relativePath;

        if (assets.ContainsKey(relativePath))
            assets[relativePath] = asset;
        else
            assets.Add(relativePath, asset);
    }

    public void QueueAssetInMain(Asset asset)
    {
        mainLoadedAssets.Push(asset);
    }

    public string RelativePathToAbsolute(string path)
    {
        assets.TryGetValue(path, out Asset asset);

        if (asset != null)
            return asset.files[0];
        else
            return $"{Application.streamingAssetsPath}/_UNKNOWN_/{path}";
    }

    public string AbsolutePathToRelative(string path)
    {
        string[] pieces = path.Split(new string[] { Application.streamingAssetsPath }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split(new string[] { "\\" }, System.StringSplitOptions.RemoveEmptyEntries);
        string finishedPath = string.Empty;
        for (int i = 1; i < pieces.Length; i++)
            finishedPath += $"{pieces[i]}/";

        return finishedPath.Trim('/');
    }
}
