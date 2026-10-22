using UnityEngine;
using UnityEngine.EventSystems;

/// Works with mouse (PC/Mac/Linux) and touch (Android/iOS): legacy Input emulates
/// mouse button 0 from the first finger.
public class ClickDetector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactableLayer = ~0;
    [SerializeField] private float maxDistance = 100f;

    private void Awake() { if (cam == null) cam = GetComponent<Camera>(); if (cam == null) cam = Camera.main; }

    private bool OverUI()
    {
        var es = EventSystem.current;
        if (es == null) return false;
        if (Input.touchCount > 0) return es.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return es.IsPointerOverGameObject();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || cam == null || OverUI()) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            var it = hit.collider.GetComponent<IInteractable>() ?? hit.collider.GetComponentInParent<IInteractable>();
            it?.OnInteract();
        }
    }
}
