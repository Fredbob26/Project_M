using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Header("Essence Settings")]
    public float lifeTimeBonus = 5f;    // сколько добавляем времени игроку
    public int scoreValue = 10;         // сколько очков даёт
    public float floatSpeed = 1f;       // эффект покачивания
    public float rotationSpeed = 50f;   // вращение

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Лёгкое покачивание и вращение для визуала
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * 0.2f;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
