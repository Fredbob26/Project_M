using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    [Header("UI Elements")]
    public Text timerText;

    [Header("References")]
    public PlayerHealth playerHealth;

    void Update()
    {
        if (playerHealth == null || timerText == null) return;

        float timeLeft = Mathf.Max(0, playerHealth.currentLifeTime);
        timerText.text = $"Time: {timeLeft:F1}";
    }
}