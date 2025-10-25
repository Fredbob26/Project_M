using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text scoreText;
    public TMP_Text detailsText; // можно оставить пустым в инспекторе, если не нужно
    public Button restartButton;
    public Button quitButton;

    private void Start()
    {
        if (restartButton) restartButton.onClick.AddListener(() => Game.I.Restart());
        if (quitButton) quitButton.onClick.AddListener(() => Game.I.Quit());
    }

    public void Show(int finalScore, float secondsAlive, int essencePicked, int enemiesKilled)
    {
        if (panel) panel.SetActive(true);

        if (scoreText)
            scoreText.text = $"Final Score: {finalScore}";

        if (detailsText)
            detailsText.text = $"Time: {secondsAlive:F1}s\nEssence Picked: {essencePicked}\nEnemies Killed: {enemiesKilled}";
    }

    public void Hide()
    {
        if (panel) panel.SetActive(false);
    }
}