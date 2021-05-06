using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Addon
{
    public AddonInfo info;
    public Asset[] assets;
    public long size = 0;
    public bool isLoaded = false;
    public string path;

    public Addon()
    {
        Setup(string.Empty, new AddonInfo());
    }

    public Addon(string path)
    {
        Setup(path, new AddonInfo());
    }

    public Addon(AddonInfo info)
    {
        Setup(string.Empty, info);
    }

    public Addon(string path, AddonInfo info)
    {
        Setup(path, info);
    }

    private void Setup(string path, AddonInfo info)
    {
        this.path = path;
        info.Validate();
        this.info = info;
    }

    public void Load()
    {
        LoadSize(path);
        assets = LoadDirectory(path).ToArray();
        isLoaded = true;
    }

    private void LoadSize(string path)
    {
        DirectoryInfo info = new DirectoryInfo(path);
        size += info.EnumerateFiles().Sum(file => file.Length);

        DirectoryInfo[] otherDirectories = info.GetDirectories();
        for (int i=0; i<otherDirectories.Length; i++)
            LoadSize(otherDirectories[i].FullName);
    }

    private List<Asset> LoadDirectory(string path)
    {
        List<Asset> loadedAssets = new List<Asset>();

        string[] files = Directory.GetFiles(path);
        for (int i=0; i<files.Length; i++)
        {
            string extension = Path.GetExtension(files[i]);
            if (Path.GetFileName(files[i]) == "addon.info" || extension == ".meta")
                continue;

            Asset asset;
            switch (extension)
            {
                case ".fbx":
                    asset = new ModelAsset()
                    {
                        files = new string[] { files[i] },
                        source = this
                    };

                    if (asset.Load())
                        loadedAssets.Add(asset);
                    else
                        Debug.LogWarning($"Failed to load ModelAsset (does it need to be done in main thread?), Path: {files[i]}");
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                    asset = new ImageAsset
                    {
                        files = new string[] { files[i] },
                        source = this
                    };

                    if (asset.Load())
                        loadedAssets.Add(asset);
                    else
                        Debug.LogWarning($"Failed to load ImageAsset (does it need to be done in main thread?), Path: {files[i]}");
                    break;
                case ".mp3":
                case ".ogg":
                case ".wav":
                    asset = new SoundAsset
                    {
                        files = new string[] { files[i] },
                        source = this
                    };

                    if (asset.Load())
                        loadedAssets.Add(asset);
                    else
                        Debug.LogWarning($"Failed to load SoundAsset, Path: {files[i]}");
                    break;
                case ".txt":
                case ".cs":
                case ".scss":
                case ".html":
                    asset = new TextAsset
                    {
                        files = new string[] { files[i] },
                        source = this
                    };

                    if (asset.Load())
                        loadedAssets.Add(asset);
                    else
                        Debug.LogWarning($"Failed to load TextAsset, Path: {files[i]}");
                    break;
                default:
                    Debug.LogWarning($"Got unknown extension: {extension}");
                    break;
            }
        }

        string[] directories = Directory.GetDirectories(path);
        for (int i=0; i<directories.Length; i++)
            loadedAssets.AddRange(LoadDirectory(directories[i]));

        return loadedAssets;
    }
}
