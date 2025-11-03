using UnityEngine;

public class LifeTimer : MonoBehaviour
{
    float _max, _cur;

    public void Init(StaticConfig cfg) { _max = cfg.maxSeconds; _cur = cfg.startSeconds; }

    void Update()
    {
        if (!Game.I.GameReady) return;
        _cur -= Time.deltaTime;
        if (_cur <= 0f) { _cur = 0f; Game.I.KillPlayer(); }
    }

    public void AddTime(float v) { _cur = Mathf.Min(_max, _cur + v); }

    public void TakeContactDamage(float v)
    {
        // Щит = игнор урона и БЕЗ флэша
        if (Game.I.powerUps && Game.I.powerUps.ShieldActive) return;

        _cur = Mathf.Max(0f, _cur - v);
        Game.I?.hud?.FlashHit(Game.I.config.hitFlashDuration, Game.I.config.hitFlashColor);
    }

    public float Normalized => Mathf.InverseLerp(0f, _max, _cur);
}
