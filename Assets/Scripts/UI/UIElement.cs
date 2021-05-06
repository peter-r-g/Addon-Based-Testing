using UnityEngine;

public class UIElement : MonoBehaviour
{
    public Vector2 anchorPos
    {
        set
        {
            GetComponent<RectTransform>().anchoredPosition = value;
        }
        get => GetComponent<RectTransform>().anchoredPosition;
    }

    public Vector2 size
    {
        set
        {
            GetComponent<RectTransform>().sizeDelta = value;
        }
        get => GetComponent<RectTransform>().sizeDelta;
    }

    public void Anchor(AnchorPoint point)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (point == AnchorPoint.FILL)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
        else
        {
            Vector2 anchorPoint = new Vector2();

            switch (point)
            {
                case AnchorPoint.TOP_LEFT:
                    anchorPoint = new Vector2(0, 1);
                    break;
                case AnchorPoint.TOP_MIDDLE:
                    anchorPoint = new Vector2(0.5f, 1);
                    break;
                case AnchorPoint.TOP_RIGHT:
                    anchorPoint = new Vector2(1, 1);
                    break;
                case AnchorPoint.MIDDLE_LEFT:
                    anchorPoint = new Vector2(0, 0.5f);
                    break;
                case AnchorPoint.MIDDLE_CENTER:
                    anchorPoint = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPoint.MIDDLE_RIGHT:
                    anchorPoint = new Vector2(1, 0.5f);
                    break;
                case AnchorPoint.BOTTOM_LEFT:
                    anchorPoint = new Vector2(0, 0);
                    break;
                case AnchorPoint.BOTTOM_MIDDLE:
                    anchorPoint = new Vector2(0.5f, 0);
                    break;
                case AnchorPoint.BOTTOM_RIGHT:
                    anchorPoint = new Vector2(1, 0);
                    break;
            }

            rectTransform.anchorMin = anchorPoint;
            rectTransform.anchorMax = anchorPoint;
            rectTransform.pivot = anchorPoint;
        }
    }
}

public enum AnchorPoint
{
    TOP_LEFT,
    TOP_MIDDLE,
    TOP_RIGHT,
    MIDDLE_LEFT,
    MIDDLE_CENTER,
    MIDDLE_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_MIDDLE,
    BOTTOM_RIGHT,
    FILL
}
