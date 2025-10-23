using UnityEngine;
using TMPro;

public class UIScore : MonoBehaviour
{
    [Header("References")]
    public PlayerScore playerScore; // ������ �� ������

    private TextMeshProUGUI scoreText;

    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (playerScore == null || scoreText == null) return;

        scoreText.text = $"Score: {playerScore.score}";
    }
}