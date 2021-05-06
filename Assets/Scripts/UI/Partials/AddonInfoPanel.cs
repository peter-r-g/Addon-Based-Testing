using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddonInfoPanel : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField]
    private string addonInfoFormat = "Author: %AUTHOR\nSize: %SIZE\nVersion: %VERSION";

    private GameObject addonIconImage;
    private GameObject addonInfoText;

    private Addon addon;
    private bool shouldRepopulate = true;

    private void Awake()
    {
        RectTransform rootRectTransform = GetComponent<RectTransform>();
        GetComponent<UIElement>().Anchor(AnchorPoint.FILL);

        addonIconImage = UIManager.instance.Create(UIElementType.Image, gameObject);
        addonIconImage.transform.name = "Addon Icon";
        UIElement element = addonIconImage.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -5);
        element.size = new Vector2(rootRectTransform.rect.width/2, rootRectTransform.rect.width/2);

        addonInfoText = UIManager.instance.Create(UIElementType.Text, gameObject);
        addonInfoText.transform.name = "Addon Info Text";
        element = addonInfoText.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.BOTTOM_MIDDLE);
        element.size = new Vector2(rootRectTransform.rect.width-10, rootRectTransform.rect.height/2);
        TMP_Text text = addonInfoText.GetComponent<TMP_Text>();
        text.fontSize = 18;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Top;
    }

    private void Update()
    {
        if (shouldRepopulate)
            Repopulate();
    }

    private void Repopulate()
    {
        if (addon == null)
            return;

        string addonInfo = addonInfoFormat.Replace("%NAME", addon.info.Name).Replace("%AUTHOR", addon.info.Author).Replace("%CONTACT", addon.info.Contact).Replace("%VERSION", addon.info.Version).Replace("%SIZE", UIManager.instance.FormatSize(addon.size));
        addonInfoText.GetComponent<TMP_Text>().text = addonInfo;

        ImageAsset iconAsset = (ImageAsset)AssetManager.instance.GetAsset($"{addon.info.Name}.png");
        if (iconAsset != null)
        {
            Image image = addonIconImage.GetComponent<Image>();
            image.sprite = iconAsset.AsSprite();
            image.color = Color.white;
        }
        else
        {
            Image image = addonIconImage.GetComponent<Image>();
            image.sprite = null;
            image.color = Random.ColorHSV();
        }

        shouldRepopulate = false;
    }

    public void ShowAddonInfo()
    {
        gameObject.SetActive(true);
    }

    public void HideAddonInfo()
    {
        gameObject.SetActive(false);
    }

    public void SetInfoFormat(string newFormat)
    {
        addonInfoFormat = newFormat;
    }

    public void SetAddon(Addon addon)
    {
        transform.name = $"Addon - {addon.info.Name}";
        this.addon = addon;
        shouldRepopulate = true;
    }

    public Addon GetAddon()
    {
        return addon;
    }
}
