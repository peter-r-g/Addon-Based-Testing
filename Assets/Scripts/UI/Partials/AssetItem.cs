using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetItem : MonoBehaviour, IPointerDownHandler
{
    private GameObject assetPreview;
    private GameObject assetPath;

    private Asset asset;
    private bool shouldRepopulate = true;

    private void Awake()
    {
        GetComponent<UIElement>().size = new Vector2(128, 158);

        assetPreview = UIManager.instance.Create(UIElementType.Image, gameObject);
        assetPreview.transform.name = "Asset Preview";
        UIElement element = assetPreview.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.size = new Vector2(128, 128);

        assetPath = UIManager.instance.Create(UIElementType.Text, gameObject);
        assetPath.transform.name = "Asset Path Text";
        element = assetPath.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.BOTTOM_MIDDLE);
        element.size = new Vector2(128, 30);
        TMP_Text text = assetPath.GetComponent<TMP_Text>();
        text.fontSize = 12;
        text.fontStyle = FontStyles.Bold;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
    }

    private void Update()
    {
        if (shouldRepopulate)
            Repopulate();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.instance.assetViewer.SetAsset(asset);
        UIManager.instance.assetViewer.Open();
    }

    private void Repopulate()
    {
        if (asset == null)
            return;

        Image image = assetPreview.GetComponent<Image>();
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

        string itemText;
        if (asset.relativePath.Length > 15)
            itemText = $"{asset.relativePath.Substring(0, 15)}...";
        else
            itemText = asset.relativePath;

        assetPath.GetComponent<TMP_Text>().text = itemText;

        shouldRepopulate = false;
    }

    public void ShouldDraw(bool shouldDraw)
    {
        if (shouldDraw)
        {
            GetComponent<Image>().enabled = true;
            assetPreview.GetComponent<Image>().enabled = true;
            assetPath.GetComponent<TMP_Text>().enabled = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
            assetPreview.GetComponent<Image>().enabled = false;
            assetPath.GetComponent<TMP_Text>().enabled = false;
        }
    }

    public void SetAsset(Asset asset)
    {
        transform.name = $"Asset Item - {asset.relativePath}";
        this.asset = asset;
        shouldRepopulate = true;
    }

    public Asset GetAsset()
    {
        return asset;
    }
}
