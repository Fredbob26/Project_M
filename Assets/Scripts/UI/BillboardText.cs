using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Transform _cam;

    private void Start()
    {
        if (Camera.main != null)
            _cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cam == null) return;
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.position, Vector3.up);
    }
}