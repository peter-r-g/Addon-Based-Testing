using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetViewer : BaseMenu
{
    private GameObject openFileButton;
    private GameObject assetPreviewImage;
    private GameObject assetInfoText;

    [SerializeField]
    private string assetInfoFormat = "Source: %SOURCE\nRelative Path: %RELATIVE_PATH\nAbsolute Path: %ABSOLUTE_PATH\nSize: %SIZE";
    private Asset asset;

    protected override void Setup()
    {
        base.Setup("Asset Viewer", new Vector2(500, 400));
        RectTransform rootRectTransform = GetComponent<RectTransform>();

        openFileButton = Create(UIElementType.Button, header);
        UIElement element = openFileButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_RIGHT);
        element.anchorPos = new Vector2(-35, -5);
        element.size = new Vector2(70, 30);
        openFileButton.GetComponentInChildren<TMP_Text>().text = "Open";
        openFileButton.GetComponent<Button>().onClick.AddListener(OpenFileButtonClicked);

        assetPreviewImage = Create(UIElementType.Image);
        element = assetPreviewImage.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -5);
        element.size = new Vector2(256, 256);

        assetInfoText = Create(UIElementType.Text);
        element = assetInfoText.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.BOTTOM_MIDDLE);
        element.anchorPos = new Vector2(0, 10);
        element.size = new Vector2(rootRectTransform.rect.width-10, 90);
        TMP_Text text = assetInfoText.GetComponent<TMP_Text>();
        text.fontSize = 12;
        text.fontStyle = FontStyles.Bold;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Bottom;
    }

    private void OpenFileButtonClicked()
    {
        if (asset != null)
            Process.Start(asset.files[0]);
    }

    private void UpdateInfoText()
    {
        string assetInfo = assetInfoFormat.Replace("%SOURCE", asset.source.info.Name).Replace("%RELATIVE_PATH", asset.relativePath).Replace("%ABSOLUTE_PATH", asset.files[0]).Replace("%SIZE", UIManager.instance.FormatSize(asset.size));
        assetInfoText.GetComponent<TMP_Text>().text = assetInfo;
    }

    public void SetAssetInfoFormat(string format)
    {
        assetInfoFormat = format;
        UpdateInfoText();
    }

    public void SetAsset(Asset asset)
    {
        SetTitle(Path.GetFileName(asset.relativePath));
        this.asset = asset;

        Image image = assetPreviewImage.GetComponent<Image>();
        if (asset.assetType == AssetType.Image)
        {
            image.sprite = ((ImageAsset)asset).AsSprite();
            image.color = Color.white;
        }
        else
        {
            Asset iconAsset = AssetManager.instance.GetAsset($"icon/{Path.GetExtension(asset.relativePath)}.png");
            if (iconAsset != null && iconAsset.assetType == AssetType.Image)
            {
                image.sprite = ((ImageAsset)iconAsset).AsSprite();
                image.color = Color.white;
            }
            else
            {
                image.sprite = null;
                image.color = Random.ColorHSV();
            }
        }

        UpdateInfoText();
    }

    public Asset GetAsset()
    {
        return asset;
    }
}
