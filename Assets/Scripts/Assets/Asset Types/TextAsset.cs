using System;
using System.IO;
using UnityEngine;

public class TextAsset : Asset
{
    public string asset;

    public override bool Load()
    {
        assetType = AssetType.Text;

        try
        {
            asset = File.ReadAllText(files[0]);
            AssetManager.instance.AddAsset(files[0], this);
            CalculateSize();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            asset = string.Empty;
            return false;
        }
    }
}
