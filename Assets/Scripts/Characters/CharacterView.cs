using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    public enum ViewMode2D3D { Sprite2D, Model3D }
    public ViewMode2D3D mode = ViewMode2D3D.Sprite2D;
    public CharacterDef character;
    public Image spriteImage;
    public Transform modelAnchor;
    GameObject modelInstance;

    public void SetCharacter(CharacterDef ch, ViewMode2D3D? forced = null)
    {
        character = ch;
        if (ch == null)
        {
            mode = ViewMode2D3D.Sprite2D;
            if (spriteImage) { spriteImage.gameObject.SetActive(true); spriteImage.sprite = null; }
            if (modelAnchor) modelAnchor.gameObject.SetActive(false);
            ClearModel();
            return;
        }

        ViewMode2D3D m = forced ?? GetMode(ch);
        mode = m;

        if (spriteImage) spriteImage.gameObject.SetActive(m == ViewMode2D3D.Sprite2D);
        if (modelAnchor) modelAnchor.gameObject.SetActive(m == ViewMode2D3D.Model3D);

        if (m == ViewMode2D3D.Sprite2D && spriteImage)
        {
            spriteImage.sprite = PortraitLibrary.GetSprite(ch);
            ClearModel();
        }

        if (m == ViewMode2D3D.Model3D)
        {
            if (spriteImage) spriteImage.sprite = PortraitLibrary.GetSprite(ch);
            LoadModel(ch);
        }
    }

    ViewMode2D3D GetMode(CharacterDef ch)
    {
        if (ch.viewMode == CharacterViewMode.Model3D) return ViewMode2D3D.Model3D;
        if (ch.viewMode == CharacterViewMode.Sprite2D) return ViewMode2D3D.Sprite2D;
        string p = GetPrefabPath(ch);
        if (!string.IsNullOrEmpty(p))
        {
            if (Resources.Load<GameObject>(p) != null) return ViewMode2D3D.Model3D;
        }
        return ViewMode2D3D.Sprite2D;
    }

    string GetPrefabPath(CharacterDef ch)
    {
        if (!string.IsNullOrEmpty(ch.modelResourcePath)) return ch.modelResourcePath;
        if (!string.IsNullOrEmpty(ch.modelPrefab)) return ch.modelPrefab;
        return null;
    }

    void LoadModel(CharacterDef ch)
    {
        if (modelAnchor == null) return;
        ClearModel();
        string p = GetPrefabPath(ch);
        if (string.IsNullOrEmpty(p))
        {
            mode = ViewMode2D3D.Sprite2D;
            if (spriteImage) spriteImage.gameObject.SetActive(true);
            modelAnchor.gameObject.SetActive(false);
            return;
        }
        var prefab = Resources.Load<GameObject>(p);
        if (prefab == null)
        {
            Debug.LogWarning("CharacterView: prefab not found in Resources: " + p);
            mode = ViewMode2D3D.Sprite2D;
            if (spriteImage) spriteImage.gameObject.SetActive(true);
            modelAnchor.gameObject.SetActive(false);
            return;
        }
        modelInstance = Instantiate(prefab, modelAnchor);
        modelInstance.transform.localPosition = ch.modelOffset;
        modelInstance.transform.localRotation = Quaternion.Euler(0f, ch.modelRotationY, 0f);
        modelInstance.transform.localScale = Vector3.one * Mathf.Max(0.0001f, ch.modelScale);
        var bb = modelInstance.GetComponent<Billboard3D>() ?? modelInstance.AddComponent<Billboard3D>();
        bb.enabled = ch.billboard3D;
    }

    void ClearModel()
    {
        if (modelInstance != null)
        {
            Destroy(modelInstance);
            modelInstance = null;
        }
    }
}
