using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddonViewer : BaseMenu
{
    [Header("UI Element Prefabs - AddonViewer")]
    [SerializeField]
    private GameObject addonInfoPanelPrefab;

    private GameObject addonDropdown;
    private GameObject switchViewButton;
    private GameObject addonContainer;
    private GameObject addonInfoPanel;

    private Addon chosenAddon;

    protected override void Setup()
    {
        base.Setup("Addon Viewer", new Vector2(400, 500));
        RectTransform rootRectTransform = GetComponent<RectTransform>();

        addonContainer = Create(UIElementType.Empty);
        addonContainer.transform.name = "Addon Container";
        UIElement element = addonContainer.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.MIDDLE_CENTER);
        element.anchorPos = new Vector2(0, -5);
        element.size = new Vector2(rootRectTransform.rect.width - 10, rootRectTransform.rect.height - 100);

        addonDropdown = Create(UIElementType.Dropdown);
        addonDropdown.transform.name = "Addon Dropdown";
        element = addonDropdown.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -5);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        addonDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnAddonChosen);

        switchViewButton = Create(UIElementType.Button);
        switchViewButton.transform.name = "View Assets Button";
        element = switchViewButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.BOTTOM_MIDDLE);
        element.anchorPos = new Vector2(0, 10);
        element.size = new Vector2(rootRectTransform.rect.width-10, 30);
        switchViewButton.GetComponentInChildren<TMP_Text>().text = "View Assets";
        switchViewButton.GetComponent<Button>().onClick.AddListener(SwitchViewButtonClicked);

        addonInfoPanel = Create(addonInfoPanelPrefab, addonContainer);
    }

    private void PopulateAddonDropdown()
    {
        TMP_Dropdown dropdownMenu = addonDropdown.GetComponent<TMP_Dropdown>();
        dropdownMenu.options.Clear();
        for (int i=0; i<AddonManager.instance.addons.Count; i++)
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(AddonManager.instance.addons[i].info.Name));
    }

    private void OnAddonChosen(int chosenIndex)
    {
        chosenAddon = AddonManager.instance.addons[chosenIndex];
        AddonInfoPanel infoPanel = addonInfoPanel.GetComponent<AddonInfoPanel>();
        infoPanel.SetAddon(chosenAddon);
        infoPanel.ShowAddonInfo();
    }

    private void SwitchViewButtonClicked()
    {
        UIManager.instance.assetBrowser.FilterForAddon(chosenAddon);
        UIManager.instance.assetBrowser.Open();
    }

    public override void Open()
    {
        if (gameObject.activeSelf)
            return;

        base.Open();
        PopulateAddonDropdown();

        if (chosenAddon == null && AddonManager.instance.addons.Count > 0)
            OnAddonChosen(0);
    }
}
