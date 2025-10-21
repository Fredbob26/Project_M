using UnityEngine;
using UnityEngine.UI;

public class UITimerBar : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;  // ������ �� ������
    public Image fillImage;            // ������� ������� (���������� �������)

    private float maxTime;

    void Start()
    {
        if (playerHealth != null)
            maxTime = playerHealth.maxLifeTime;
    }

    void Update()
    {
        if (playerHealth == null || fillImage == null) return;

        float fillAmount = Mathf.Clamp01(playerHealth.currentLifeTime / maxTime);
        fillImage.fillAmount = fillAmount;
    }
}