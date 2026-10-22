using UnityEngine;

public class Billboard3D : MonoBehaviour
{
    public bool lockY = true;
    Transform cam;

    void Awake()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main != null) cam = Camera.main.transform;
            if (cam == null) return;
        }
        Vector3 t = transform.position;
        Vector3 c = cam.position;
        if (lockY) c.y = t.y;
        transform.LookAt(c);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + 180f, 0f);
    }
}
