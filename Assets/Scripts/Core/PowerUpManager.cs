using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private float _rapidUntil = 0f;
    private float _shieldUntil = 0f;
    private float _freezeUntil = 0f;
    private float _frenzyUntil = 0f;

    private StaticConfig Cfg => Game.I ? Game.I.config : null;

    public bool RapidFireActive => Time.time < _rapidUntil;
    public bool ShieldActive => Time.time < _shieldUntil;
    public bool FreezeActive => Time.time < _freezeUntil;
    public bool ScoreFrenzyActive => Time.time < _frenzyUntil;

    public float ScoreFrenzyMultiplier =>
        (ScoreFrenzyActive && Cfg) ? Mathf.Max(1f, Cfg.scoreFrenzyMultiplier) : 1f;

    public float RapidFireRateMul =>
        (RapidFireActive && Cfg) ? Mathf.Max(1f, Cfg.rapidFireFireRateMul) : 1f;

    public void Activate(PowerUpType type)
    {
        if (Cfg == null) return;

        switch (type)
        {
            case PowerUpType.RapidFire:
                _rapidUntil = Time.time + Cfg.rapidFireDuration;
                break;
            case PowerUpType.Shield:
                _shieldUntil = Time.time + Cfg.shieldDuration;
                break;
            case PowerUpType.Freeze:
                _freezeUntil = Time.time + Cfg.freezeDuration;
                break;
            case PowerUpType.ScoreFrenzy:
                _frenzyUntil = Time.time + Cfg.scoreFrenzyDuration;
                break;
        }
    }
}
