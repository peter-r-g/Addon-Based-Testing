using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameObject rootElement;

    [Header("UI Prefabs")]
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject dropdownPrefab;
    [SerializeField]
    private GameObject emptyPrefab;
    [SerializeField]
    private GameObject imagePrefab;
    [SerializeField]
    private GameObject inputPrefab;
    [SerializeField]
    private GameObject panelPrefab;
    [SerializeField]
    private GameObject textPrefab;
    [SerializeField]
    private GameObject addonViewerPrefab;
    [SerializeField]
    private GameObject assetBrowserPrefab;
    [SerializeField]
    private GameObject debugPanelPrefab;
    [SerializeField]
    private GameObject assetViewerPrefab;

    [HideInInspector]
    public AddonViewer addonViewer;
    [HideInInspector]
    public AssetBrowser assetBrowser;
    [HideInInspector]
    public DebugPanel debugPanel;
    [HideInInspector]
    public AssetViewer assetViewer;

    private Dictionary<UIElementType, GameObject> genericPrefabs;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        genericPrefabs = new Dictionary<UIElementType, GameObject>();
        genericPrefabs.Add(UIElementType.Button, buttonPrefab);
        genericPrefabs.Add(UIElementType.Dropdown, dropdownPrefab);
        genericPrefabs.Add(UIElementType.Empty, emptyPrefab);
        genericPrefabs.Add(UIElementType.Image, imagePrefab);
        genericPrefabs.Add(UIElementType.Input, inputPrefab);
        genericPrefabs.Add(UIElementType.Panel, panelPrefab);
        genericPrefabs.Add(UIElementType.Text, textPrefab);

        GameObject addonViewerObj = Create(addonViewerPrefab);
        addonViewer = addonViewerObj.GetComponent<AddonViewer>();
        GameObject assetBrowserObj = Create(assetBrowserPrefab);
        assetBrowser = assetBrowserObj.GetComponent<AssetBrowser>();
        GameObject debugPanelObj = Create(debugPanelPrefab);
        debugPanel = debugPanelObj.GetComponent<DebugPanel>();
        GameObject assetViewerObj = Create(assetViewerPrefab);
        assetViewer = assetViewerObj.GetComponent<AssetViewer>();

        addonViewer.Close();
        assetBrowser.Close();
        assetViewer.Close();
    }

    public string FormatSize(long bytes)
    {
        double size = bytes;
        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
        short format = 0;
        while (size >= 1024 && format < sizes.Length - 1)
        {
            format++;
            size /= 1024;
        }

        return string.Format("{0:0.##}{1}", size, sizes[format]);
    }

    public GameObject Create(UIElementType element)
    {
        return Create(element, rootElement);
    }

    public GameObject Create(UIElementType element, GameObject parent)
    {
        return Instantiate(genericPrefabs[element], parent.transform);
    }

    public GameObject Create(GameObject prefab)
    {
        return Create(prefab, rootElement);
    }

    public GameObject Create(GameObject prefab, GameObject parent)
    {
        return Instantiate(prefab, parent.transform);
    }
}

public enum UIElementType
{
    Button,
    Dropdown,
    Empty,
    Image,
    Input,
    Panel,
    Text
}
