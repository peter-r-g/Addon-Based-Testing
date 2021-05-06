using System;
using UnityEngine;

public class ModelAsset : Asset
{
    public override bool Load()
    {
        assetType = AssetType.Model;

        try
        {
            CalculateSize();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            return false;
        }
    }
}
