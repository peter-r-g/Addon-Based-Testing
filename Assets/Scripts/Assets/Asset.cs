using System.IO;

public class Asset
{
    public AssetType assetType;
    public string[] files;
    public string relativePath;
    public Addon source;
    public long size = 0;

    public virtual bool Load()
    {
        assetType = AssetType.Unknown;
        return false;
    }

    public void CalculateSize()
    {
        size = 0;
        for (int i=0; i<files.Length; i++)
            size += new FileInfo(files[i]).Length;
    }
}

public enum AssetType
{
    Unknown,
    Model,
    Image,
    Sound,
    Text
}