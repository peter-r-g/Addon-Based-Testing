using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetBrowser : BaseMenu
{
    [Header("UI Element Prefabs - AssetBrowser")]
    [SerializeField]
    private GameObject assetContainerPrefab;
    [SerializeField]
    private GameObject assetItemPrefab;

    private GameObject addonFilterDropdown;
    private GameObject assetFilterDropdown;
    private GameObject pathFilterInput;
    private GameObject assetContainer;
    private GameObject assetContainerContent;
    private List<GameObject> assetItems;
    private int assetItemIndex;
    private Stack<Asset> assetItemQueue;

    private string pathFilter = string.Empty;
    private AssetType typeFilter = AssetType.Unknown;
    private Addon addonFilter;
    private bool shouldRepopulate = true;

    protected override void Setup()
    {
        base.Setup("Asset Browser", new Vector2(782, 500));
        RectTransform rootRectTransform = GetComponent<RectTransform>();

        addonFilterDropdown = Create(UIElementType.Dropdown);
        addonFilterDropdown.transform.name = "Addon Filter";
        UIElement element = addonFilterDropdown.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_LEFT);
        element.anchorPos = new Vector2(5, -5);
        addonFilterDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnAddonSelected);

        assetFilterDropdown = Create(UIElementType.Dropdown);
        assetFilterDropdown.transform.name = "Asset Filter";
        element = assetFilterDropdown.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -5);
        assetFilterDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnAssetTypeSelected);

        pathFilterInput = Create(UIElementType.Input);
        pathFilterInput.transform.name = "Path Filter";
        element = pathFilterInput.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_RIGHT);
        element.anchorPos = new Vector2(-5, -5);
        pathFilterInput.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Enter path...";
        pathFilterInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(OnPathFilterEdited);

        assetContainer = Create(assetContainerPrefab);
        assetContainer.transform.name = "Asset Container";
        element = assetContainer.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_LEFT);
        element.anchorPos = new Vector2(2, -37);
        element.size = new Vector2(rootRectTransform.rect.width-4, rootRectTransform.rect.height-85);

        assetContainerContent = assetContainer.GetComponent<ScrollRect>().content.gameObject;
        assetContainerContent.transform.name = "Assets";
        element = assetContainerContent.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_LEFT);
        element.size = assetContainer.GetComponent<UIElement>().size;
        GridLayoutGroup gridLayout = assetContainerContent.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(128, 158);
        gridLayout.spacing = new Vector2(2, 2);

        assetItems = new List<GameObject>();
        assetItemQueue = new Stack<Asset>();
        AddonManager.instance.OnAddonsLoaded += OnAddonsLoaded;
    }

    protected override void Update()
    {
        base.Update();

        if (shouldRepopulate && AddonManager.instance.Ready)
            Repopulate();

        if (assetItemQueue.Count > 0)
            AddAsset(assetItemQueue.Pop());
    }

    private void OnAddonsLoaded(object sender, EventArgs args)
    {
        RepopulateAddonFilter();
        RepopulateAssetTypeFilter();
        FilterForAddon(null);
        FilterForAssetType(AssetType.Unknown);
        FilterForPath(string.Empty);
    }

    private void Repopulate()
    {
        assetItemIndex = 0;
        assetItemQueue.Clear();

        List<Asset> assets = Filter();
        for (int i=0; i<assets.Count; i++)
            assetItemQueue.Push(assets[i]);

        for (int i=0; i<assetItems.Count; i++)
            assetItems[i].SetActive(i < assetItemQueue.Count);

        shouldRepopulate = false;
    }

    private void RepopulateAddonFilter()
    {
        TMP_Dropdown dropdown = addonFilterDropdown.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData("All"));
        for (int i = 0; i < AddonManager.instance.addons.Count; i++)
            dropdown.options.Add(new TMP_Dropdown.OptionData(AddonManager.instance.addons[i].info.Name));
        dropdown.RefreshShownValue();
    }

    private void RepopulateAssetTypeFilter()
    {
        TMP_Dropdown dropdown = assetFilterDropdown.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData("All"));
        Array assetTypes = Enum.GetValues(typeof(AssetType));
        for (int i = 1; i < assetTypes.Length; i++)
        {
            AssetType assetType = (AssetType)assetTypes.GetValue(i);
            dropdown.options.Add(new TMP_Dropdown.OptionData(assetType.ToString()));
        }
        dropdown.RefreshShownValue();
    }

    private List<Asset> Filter()
    {
        List<Asset> filteredAssets = new List<Asset>();

        foreach (KeyValuePair<string, Asset> asset in AssetManager.instance.assets)
        {
            if (addonFilter != null && asset.Value.source != addonFilter)
                continue;

            if (typeFilter != AssetType.Unknown && asset.Value.assetType != typeFilter)
                continue;

            if (pathFilter != string.Empty && !asset.Key.ToLower().Contains(pathFilter.ToLower()))
                continue;

            filteredAssets.Add(asset.Value);
        }

        return filteredAssets;
    }

    private void AddAsset(Asset asset)
    {
        if (assetItemIndex >= assetItems.Count)
        {
            GameObject assetItem = Create(assetItemPrefab, assetContainerContent);
            assetItem.GetComponent<AssetItem>().SetAsset(asset);
            assetItems.Add(assetItem);
        }
        else
            assetItems[assetItemIndex].GetComponent<AssetItem>().SetAsset(asset);

        assetItemIndex++;
    }

    private void OnAddonSelected(int index)
    {
        if (index == 0)
            FilterForAddon(null);
        else
            FilterForAddon(AddonManager.instance.addons[index-1]);
    }

    private void OnAssetTypeSelected(int index)
    {
        if (index == 0)
            FilterForAssetType(AssetType.Unknown);
        else
            FilterForAssetType((AssetType)Enum.GetValues(typeof(AssetType)).GetValue(index));
    }

    private void OnPathFilterEdited(string path)
    {
        pathFilter = path;
        shouldRepopulate = true;
    }

    public void FilterForAddon(Addon addon)
    {
        TMP_Dropdown dropdown = addonFilterDropdown.GetComponent<TMP_Dropdown>();
        if (addon != null)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == addon.info.Name)
                {
                    dropdown.value = i;
                    break;
                }
            }
        }

        addonFilter = addon;
        shouldRepopulate = true;
    }

    public void FilterForAssetType(AssetType type)
    {
        TMP_Dropdown dropdown = assetFilterDropdown.GetComponent<TMP_Dropdown>();
        for (int i=0; i<dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == type.ToString())
            {
                dropdown.value = i;
                break;
            }
        }

        typeFilter = type;
        shouldRepopulate = true;
    }

    public void FilterForPath(string relativePath)
    {
        pathFilterInput.GetComponent<TMP_InputField>().text = relativePath;
        pathFilter = relativePath;
        shouldRepopulate = true;
    }
}
