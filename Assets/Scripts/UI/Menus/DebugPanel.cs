using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : BaseMenu
{
    private GameObject loadedAddonsText;
    private GameObject viewAddonsButton;
    private GameObject assetBrowserButton;
    private GameObject reloadAddonsButton;

    protected override void Setup()
    {
        base.Setup("Debug", new Vector2(300, 190));
        RectTransform rootRectTransform = GetComponent<RectTransform>();

        loadedAddonsText = Create(UIElementType.Text);
        loadedAddonsText.transform.name = "Loaded Addons Text";
        UIElement element = loadedAddonsText.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -5);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        TMP_Text text = loadedAddonsText.GetComponent<TMP_Text>();
        text.fontSize = 24;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Top;

        viewAddonsButton = Create(UIElementType.Button);
        viewAddonsButton.transform.name = "View Addons Button";
        element = viewAddonsButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -40);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        viewAddonsButton.GetComponentInChildren<TMP_Text>().text = "View Addons";
        viewAddonsButton.GetComponent<Button>().onClick.AddListener(ViewAddonsButtonClicked);

        assetBrowserButton = Create(UIElementType.Button);
        assetBrowserButton.transform.name = "Asset Browser Button";
        element = assetBrowserButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -75);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        assetBrowserButton.GetComponentInChildren<TMP_Text>().text = "Asset Browser";
        assetBrowserButton.GetComponent<Button>().onClick.AddListener(AssetBrowserButtonClicked);

        reloadAddonsButton = Create(UIElementType.Button);
        reloadAddonsButton.transform.name = "Reload Addons Button";
        element = reloadAddonsButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -110);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        reloadAddonsButton.GetComponentInChildren<TMP_Text>().text = "Reload Addons";
        reloadAddonsButton.GetComponent<Button>().onClick.AddListener(ReloadAddonsButtonClicked);

        ShowCloseButton(false);
        AddonManager.instance.OnAddonsStartedLoading += OnAddonsStartedLoading;
        AddonManager.instance.OnAddonsLoaded += OnAddonsLoaded;
    }

    private void OnAddonsStartedLoading(object sender, EventArgs args)
    {
        loadedAddonsText.GetComponent<TMP_Text>().text = "Addons are loading...";

        viewAddonsButton.GetComponent<Button>().enabled = false;
        assetBrowserButton.GetComponent<Button>().enabled = false;
        reloadAddonsButton.GetComponent<Button>().enabled = false;
    }

    private void OnAddonsLoaded(object sender, EventArgs args)
    {
        loadedAddonsText.GetComponent<TMP_Text>().text = $"Loaded Addons: {AddonManager.instance.addons.Count}";

        viewAddonsButton.GetComponent<Button>().enabled = true;
        assetBrowserButton.GetComponent<Button>().enabled = true;
        reloadAddonsButton.GetComponent<Button>().enabled = true;
    }

    private void ViewAddonsButtonClicked()
    {
        UIManager.instance.addonViewer.Toggle();
    }

    private void AssetBrowserButtonClicked()
    {
        UIManager.instance.assetBrowser.FilterForAddon(null);
        UIManager.instance.assetBrowser.Toggle();
    }

    private void ReloadAddonsButtonClicked()
    {
        UIManager.instance.addonViewer.Close();
        UIManager.instance.assetBrowser.Close();
        UIManager.instance.assetViewer.Close();
        AddonManager.instance.LoadAddons(false);
    }
}
