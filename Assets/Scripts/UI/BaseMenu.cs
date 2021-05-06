using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected GameObject header;
    protected GameObject titleText;
    protected GameObject closeButton;
    protected GameObject divider;
    protected GameObject contentContainer;

    private Vector3 dragPosition;
    protected bool dragging = false;

    private void Awake()
    {
        Setup();
    }

    protected virtual void Update()
    {
        if (dragging)
        {
            Vector2 pos = ClampPos(new Vector2(Input.mousePosition.x+dragPosition.x, Input.mousePosition.y+dragPosition.y));
            gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        gameObject.transform.SetAsLastSibling();
        dragPosition = gameObject.transform.position - new Vector3(eventData.position.x, eventData.position.y, 0);
        dragging = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        dragPosition = Vector3.zero;
        dragging = false;
    }

    private Vector2 ClampPos(Vector2 pos)
    {
        Vector2 size = GetSize();
        return new Vector2(Mathf.Clamp(pos.x, size.x / 2, Screen.width - (size.x / 2)), Mathf.Clamp(pos.y, size.y / 2, Screen.height - (size.y / 2)));
    }

    protected virtual void Setup()
    {
        Setup("Menu");
    }

    protected virtual void Setup(string title)
    {
        Setup(title, new Vector2(1, 1));
    }

    protected virtual void Setup(string title, Vector2 menuSize)
    {
        RectTransform rootRectTransform = GetComponent<RectTransform>();
        transform.name = title;

        UIElement element = GetComponent<UIElement>();
        element.Anchor(AnchorPoint.MIDDLE_CENTER);
        element.size = menuSize;
        Color panelColor = Color.grey;
        panelColor.a = 200;
        GetComponent<Image>().color = panelColor;

        header = Create(UIElementType.Empty, gameObject);
        header.transform.name = "Header";
        element = header.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.size = new Vector2(rootRectTransform.rect.width, 40);

        titleText = Create(UIElementType.Text, header);
        titleText.transform.name = "Title";
        element = titleText.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_LEFT);
        element.anchorPos = new Vector2(5, -5);
        element.size = new Vector2(rootRectTransform.rect.width - 40, 30);
        TMP_Text text = titleText.GetComponent<TMP_Text>();
        text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        text.verticalAlignment = VerticalAlignmentOptions.Top;
        text.fontSize = 24;
        text.text = title;

        closeButton = Create(UIElementType.Button, header);
        closeButton.transform.name = "Close";
        element = closeButton.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_RIGHT);
        element.anchorPos = new Vector2(-5, -5);
        element.size = new Vector2(30, 30);
        closeButton.GetComponentInChildren<TMP_Text>().text = "X";
        Button button = closeButton.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.red;
        button.colors = colors;
        button.onClick.AddListener(Close);

        divider = Create(UIElementType.Image, header);
        divider.transform.name = "Divider";
        element = divider.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -40);
        element.size = new Vector2(rootRectTransform.rect.width - 4, 5);
        divider.GetComponent<Image>().color = Color.grey;

        contentContainer = Create(UIElementType.Empty, gameObject);
        contentContainer.transform.name = "Content";
        element = contentContainer.GetComponent<UIElement>();
        element.Anchor(AnchorPoint.TOP_MIDDLE);
        element.anchorPos = new Vector2(0, -45);
        element.size = new Vector2(rootRectTransform.rect.width, rootRectTransform.rect.height-40);
    }
    
    protected virtual GameObject Create(UIElementType element)
    {
        return Create(element, contentContainer);
    }

    protected virtual GameObject Create(UIElementType element, GameObject parent)
    {
        return UIManager.instance.Create(element, parent);
    }

    protected virtual GameObject Create(GameObject prefab)
    {
        return Create(prefab, contentContainer);
    }

    protected virtual GameObject Create(GameObject prefab, GameObject parent)
    {
        return UIManager.instance.Create(prefab, parent);
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Close();
        else
            Open();
    }

    public virtual void Open()
    {
        if (gameObject.activeSelf)
            return;

        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
    }

    public void SetTitle(string text)
    {
        titleText.GetComponent<TMP_Text>().text = text;
    }

    public string GetTitle()
    {
        return titleText.GetComponent<TMP_Text>().text;
    }

    public void ShowCloseButton(bool shouldShow)
    {
        if (shouldShow)
            closeButton.SetActive(true);
        else
            closeButton.SetActive(false);
    }

    public void SetSize(Vector2 size)
    {
        GetComponent<UIElement>().size = size;
        OnSizeChanged(size);
    }

    public Vector2 GetSize()
    {
        return GetComponent<UIElement>().size;
    }

    public void SetPos(Vector2 pos)
    {
        Vector2 clampedPos = ClampPos(pos);
        transform.position = new Vector3(clampedPos.x, clampedPos.y, 0);
        OnPosChanged(clampedPos);
    }

    public Vector2 GetPos()
    {
        Vector3 pos = transform.position;
        return new Vector2(pos.x, pos.y);
    }

    public void Center()
    {
        Vector2 size = GetSize();
        SetPos(new Vector2((Screen.width/2) - (size.x/2), (Screen.height/2) - (size.y/2)));
    }

    public void CenterHorizontal()
    {
        SetPos(new Vector2((Screen.width/2) - (GetSize().x/2), GetPos().y));
    }

    public void CenterVertical()
    {
        SetPos(new Vector2(GetPos().x, (Screen.height/2) - (GetSize().y/2)));
    }

    protected virtual void OnSizeChanged(Vector2 newSize)
    {
        UIElement element = header.GetComponent<UIElement>();
        element.size = new Vector2(newSize.x, element.size.y);
    }

    protected virtual void OnPosChanged(Vector2 newPos) {}
}
