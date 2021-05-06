using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class ImageAsset : Asset
{
    public Texture2D asset;
    private Sprite assetSprite;

    public override bool Load()
    {
        assetType = AssetType.Image;

        if (!AssetManager.instance.mainThread.Equals(Thread.CurrentThread))
        {
            AssetManager.instance.QueueAssetInMain(this);
            return false;
        }

        try
        {
            asset = new Texture2D(0, 0);
            asset.LoadImage(File.ReadAllBytes(files[0]));
            AssetManager.instance.AddAsset(files[0], this);
            CalculateSize();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            asset = null;
            return false;
        }
    }

    public Sprite AsSprite()
    {
        if (assetSprite != null)
            return assetSprite;

        assetSprite = Sprite.Create(asset, new Rect(0, 0, asset.width, asset.height), new Vector2(0.5f, 0.5f));
        return assetSprite;
    }
}
