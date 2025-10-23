using UnityEngine;
using System;

public class PlayerScore : MonoBehaviour
{
    public int score = 0;
    public Action<int> onScoreChanged;

    public void AddScore(int value)
    {
        score += value;
        onScoreChanged?.Invoke(score);
    }

    public bool Spend(int value)
    {
        if (score < value) return false;
        score -= value;
        onScoreChanged?.Invoke(score);
        return true;
    }
}