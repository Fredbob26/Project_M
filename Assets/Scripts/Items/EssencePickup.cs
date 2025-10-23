using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Header("Essence Settings")]
    public float lifeTimeBonus = 5f;    // ������� ��������� ������� ������
    public int scoreValue = 10;         // ������� ����� ���
    public float floatSpeed = 1f;       // ������ �����������
    public float rotationSpeed = 50f;   // ��������

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // ˸���� ����������� � �������� ��� �������
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * 0.2f;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
