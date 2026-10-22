using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// Tiny helper so the whole UI is built from code (no prefab wiring needed).
public static class UIKit
{
    static Font font;
    public static Font DefaultFont
    {
        get
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 24);
            return font;
        }
    }

    public static readonly Color Amber = new Color(0.9f, 0.76f, 0.48f);
    public static readonly Color Body = new Color(0.87f, 0.87f, 0.87f);

    public static Canvas CreateCanvas(Transform parent, string name, int order = 0)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = order;
        var s = go.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1920, 1080); s.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    public static RectTransform Panel(Transform parent, string name, Color color, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.offsetMin = rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        return rt;
    }

    public static Text Label(Transform parent, string text, int size, Color color, TextAnchor anchor, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject("Label", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.offsetMin = new Vector2(8, 4); rt.offsetMax = new Vector2(-8, -4);
        var t = go.GetComponent<Text>();
        t.font = DefaultFont; t.text = text; t.fontSize = size; t.color = color; t.alignment = anchor;
        t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
        t.resizeTextForBestFit = true; t.resizeTextMinSize = 10; t.resizeTextMaxSize = size;
        t.raycastTarget = false; t.supportRichText = true;
        return t;
    }

    public static Button MakeButton(Transform parent, string label, Vector2 aMin, Vector2 aMax, UnityAction onClick, bool interactable = true)
    {
        var rt = Panel(parent, "Button_" + label, new Color(0.18f, 0.18f, 0.22f, 0.95f), aMin, aMax);
        var b = rt.gameObject.AddComponent<Button>();
        b.targetGraphic = rt.GetComponent<Image>();
        var cb = b.colors; cb.highlightedColor = new Color(0.45f, 0.4f, 0.3f); cb.pressedColor = new Color(0.7f, 0.55f, 0.3f);
        cb.disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.8f); b.colors = cb;
        Label(rt, label, 30, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        if (onClick != null) b.onClick.AddListener(onClick);
        b.interactable = interactable;
        return b;
    }

    public static Button SmallButton(Transform parent, string label, Vector2 aMin, Vector2 aMax, UnityAction onClick, bool interactable = true, Color? bg = null)
    {
        var rt = Panel(parent, "Button_" + label, bg ?? new Color(0.18f, 0.18f, 0.22f, 0.95f), aMin, aMax);
        var b = rt.gameObject.AddComponent<Button>();
        b.targetGraphic = rt.GetComponent<Image>();
        var cb = b.colors; cb.highlightedColor = new Color(0.45f, 0.4f, 0.3f); cb.pressedColor = new Color(0.7f, 0.55f, 0.3f);
        cb.disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.8f); b.colors = cb;
        Label(rt, label, 22, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        if (onClick != null) b.onClick.AddListener(onClick);
        b.interactable = interactable;
        return b;
    }

    public static Image Img(Transform parent, string name, Sprite sprite, Color tint, Vector2 aMin, Vector2 aMax)
    {
        var rt = Panel(parent, name, Color.white, aMin, aMax);
        var img = rt.GetComponent<Image>();
        img.sprite = sprite; img.color = tint; img.preserveAspect = true;
        return img;
    }

    public static Image Portrait(Transform parent, CharacterDef c, Vector2 aMin, Vector2 aMax)
    {
        return Img(parent, "Portrait_" + (c != null ? c.id : "none"), PortraitLibrary.Get(c), Color.white, aMin, aMax);
    }

    public static Image Portrait(Transform parent, string name, CharacterDef c, Vector2 aMin, Vector2 aMax)
    {
        return Img(parent, name, PortraitLibrary.Get(c), Color.white, aMin, aMax);
    }

    public static ScrollRect Scroll(Transform parent, string name, Vector2 aMin, Vector2 aMax, out Transform content)
    {
        var viewport = Panel(parent, name, new Color(1f, 1f, 1f, 0.03f), aMin, aMax);
        var mask = viewport.gameObject.AddComponent<Mask>(); mask.showMaskGraphic = true;
        var sr = viewport.gameObject.AddComponent<ScrollRect>();
        sr.horizontal = false; sr.vertical = true; sr.movementType = ScrollRect.MovementType.Clamped;
        sr.scrollSensitivity = 28f;

        var contentGo = new GameObject("Content", typeof(RectTransform));
        contentGo.transform.SetParent(viewport, false);
        var crt = (RectTransform)contentGo.transform;
        crt.anchorMin = new Vector2(0, 1); crt.anchorMax = new Vector2(1, 1); crt.pivot = new Vector2(0.5f, 1);
        crt.offsetMin = Vector2.zero; crt.offsetMax = Vector2.zero; crt.sizeDelta = Vector2.zero;
        var vl = contentGo.AddComponent<VerticalLayoutGroup>();
        vl.spacing = 8; vl.padding = new RectOffset(10, 10, 10, 10);
        vl.childControlHeight = true; vl.childControlWidth = true;
        vl.childForceExpandHeight = false; vl.childForceExpandWidth = true;
        var fit = contentGo.AddComponent<ContentSizeFitter>();
        fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.viewport = viewport; sr.content = crt;
        content = crt;
        return sr;
    }
}
