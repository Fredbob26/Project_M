using UnityEngine;

public class LifeTimer : MonoBehaviour
{
    private float _max;
    private float _cur;

    public void Init(StaticConfig cfg)
    {
        _max = cfg.maxSeconds;
        _cur = cfg.startSeconds;
    }

    private void Update()
    {
        // Безопасные проверки — пока Game/I не готов, ничего не делаем
        var g = Game.I;
        if (g == null || !g.GameReady) return;

        _cur -= Time.deltaTime;
        if (_cur <= 0f)
        {
            _cur = 0f;
            g.KillPlayer();
        }
    }

    public void AddTime(float v) => _cur = Mathf.Min(_max, _cur + v);
    public void TakeContactDamage(float v) => _cur = Mathf.Max(0f, _cur - v);
    public float Normalized => Mathf.InverseLerp(0f, _max, _cur);
}
