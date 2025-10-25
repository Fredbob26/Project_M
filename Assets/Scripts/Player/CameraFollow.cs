using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 12f, -6f);
    public Vector3 fixedEuler = new Vector3(35f, 0f, 0f);

    private void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    private void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position + offset; // только позиция
        transform.rotation = Quaternion.Euler(fixedEuler); // фиксированная ориентация
    }
}