using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class AddonManager : MonoBehaviour
{
    public static AddonManager instance;
    public static string addonsPath = Application.streamingAssetsPath;
    public static string addonInfoFile = "addon.info";

    public List<Addon> addons;
    public bool Ready
    {
        private set;
        get;
    } = false;

    public event EventHandler OnAddonsStartedLoading;
    public event EventHandler OnAddonsLoaded;

    private FileSystemWatcher hotloadWatcher;
    private Thread fullLoadingThread;
    private Thread loadingThread;
    private bool isLoading = false;
    private Stack<Addon> addonLoadQueue;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        addonLoadQueue = new Stack<Addon>();
        hotloadWatcher = new FileSystemWatcher(addonsPath);
        hotloadWatcher.NotifyFilter = NotifyFilters.CreationTime
                                    | NotifyFilters.DirectoryName
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.Size;
        hotloadWatcher.EnableRaisingEvents = true;

        hotloadWatcher.Created += OnCreated;
        hotloadWatcher.Deleted += OnDeleted;
        hotloadWatcher.Changed += OnChanged;
        hotloadWatcher.Renamed += OnRenamed;
    }

    private void Start()
    {
        LoadAddons(false);
    }

    private void Update()
    {
        if (isLoading && !fullLoadingThread.IsAlive)
        {
            isLoading = false;
            OnAddonsLoaded?.Invoke(this, null);
        }
    }

    private void OnApplicationQuit()
    {
        hotloadWatcher.Dispose();
    }

    private void OnCreated(object sender, FileSystemEventArgs args)
    {
        DirectoryInfo info = new DirectoryInfo(args.FullPath);
        if (info.Exists)
            Debug.Log($"{args.FullPath.Replace(addonsPath.Replace("/", "\\"), "")}\\");
        //Debug.Log($"==Created==\n{args.FullPath}\n{AssetManager.instance.AbsolutePathToRelative(args.FullPath)}");
    }

    private void OnDeleted(object sender, FileSystemEventArgs args)
    {
        //Debug.Log($"==Deleted==\n{args.FullPath}\n{AssetManager.instance.AbsolutePathToRelative(args.FullPath)}");
    }

    private void OnChanged(object sender, FileSystemEventArgs args)
    {
        if (args.ChangeType != WatcherChangeTypes.Changed)
            return;

        //Debug.Log($"==Changed==\n{args.FullPath}\n{AssetManager.instance.AbsolutePathToRelative(args.FullPath)}");
    }

    private void OnRenamed(object sender, RenamedEventArgs args)
    {
        //Debug.Log($"==Renamed==\n{args.OldFullPath}\n{AssetManager.instance.AbsolutePathToRelative(args.OldFullPath)}\n{args.FullPath}\n{AssetManager.instance.AbsolutePathToRelative(args.FullPath)}");
    }

    private Addon GetAddon(string path)
    {
        for (int i=0; i<addons.Count; i++)
        {
            if (addons[i].path == path)
                return addons[i];
        }

        return null;
    }

    private void LoadAddons()
    {
        isLoading = true;
        Ready = false;

        string[] addonFolders = Directory.GetDirectories(addonsPath);
        List<Addon> loadedAddons = new List<Addon>();

        for (int i = 0; i < addonFolders.Length; i++)
        {
            try
            {
                string addonInfoText = File.ReadAllText($"{addonFolders[i]}/{addonInfoFile}");
                loadedAddons.Add(new Addon(addonFolders[i], AddonInfo.FromJSON(addonInfoText)));
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                loadedAddons.Add(new Addon(addonFolders[i]));
            }
            finally
            {
                LoadAddon(loadedAddons[i]);
            }
        }

        addons = loadedAddons;
        Ready = true;
    }

    public void LoadAddons(bool inMainThread)
    {
        if (fullLoadingThread != null && fullLoadingThread.IsAlive)
            return;

        OnAddonsStartedLoading?.Invoke(this, null);
        if (inMainThread)
            LoadAddons();
        else
        {
            fullLoadingThread = new Thread(new ThreadStart(LoadAddons));
            fullLoadingThread.Start();
        }
    }
    
    public void LoadAddon(Addon addon)
    {
        if (addon.isLoaded)
            return;

        addon.Load();
    }

    private void LoadAddon(object addon)
    {
        LoadAddon((Addon)addon);
    }

    public void LoadAddon(Addon addon, bool inMainThread)
    {
        if (addon.isLoaded)
            return;

        if (inMainThread)
            LoadAddon(addon);
        else
        {
            loadingThread = new Thread(LoadAddon);
            loadingThread.Start(addon);
        }
    }
}
