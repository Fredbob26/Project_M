using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Config/Level Unlocker")]
public class LevelUnlocker : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string sceneName;
        public float surviveSecondsToUnlock; // сколько нужно прожить на предыдущем
    }

    public Entry[] levels;

    private string PrefBest(string scene) => $"bestTime_{scene}";
    private string PrefUnlocked(string scene) => $"unlocked_{scene}";

    public float GetBest(string scene) => PlayerPrefs.GetFloat(PrefBest(scene), 0f);
    public void SetBest(string scene, float seconds)
    {
        if (seconds > GetBest(scene))
        {
            PlayerPrefs.SetFloat(PrefBest(scene), seconds);
            PlayerPrefs.Save();
        }
    }

    public bool IsUnlocked(string scene)
    {
        if (levels.Length == 0) return true;
        if (scene == levels[0].sceneName) return true;
        return PlayerPrefs.GetInt(PrefUnlocked(scene), 0) == 1;
    }

    public void TryUnlockNext(string currentScene, float survivedSeconds)
    {
        int idx = System.Array.FindIndex(levels, e => e.sceneName == currentScene);
        if (idx < 0) return;
        SetBest(currentScene, survivedSeconds);

        if (idx + 1 < levels.Length)
        {
            var next = levels[idx + 1];
            if (survivedSeconds >= next.surviveSecondsToUnlock)
            {
                PlayerPrefs.SetInt(PrefUnlocked(next.sceneName), 1);
                PlayerPrefs.Save();
                Debug.Log($"[LevelUnlocker] Unlocked: {next.sceneName}");
            }
        }
    }
}