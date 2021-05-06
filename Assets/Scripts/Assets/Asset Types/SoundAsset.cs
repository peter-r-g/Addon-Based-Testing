using System;
using System.IO;
using UnityEngine;

public class SoundAsset : Asset
{
    public override bool Load()
    {
        assetType = AssetType.Sound;

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
