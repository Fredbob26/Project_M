// Assets/Scripts/Core/PowerUpManager.cs
using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Visuals (optional)")]
    public FreezeOverlay freezeOverlay;        // фриз-экран (может быть пустым)

    [Tooltip("Префаб щита. Инстансится под игрока в рантайме.")]
    public GameObject shieldPrefab;            // ВАЖНО: сюда кидаем ПРЕФАБ из Project, а не объект со сцены

    [Header("Debug")]
    public bool logPowerUps = false;           // только для логов, можно выключить

    // таймеры эффектов (Time.time)
    float _rapidUntil, _frenzyUntil, _shieldUntil, _freezeUntil;
    bool _paused;

    // рантайм-ссылки
    GameObject _shieldInstance;
    PlayerStats _stats;
    AutoShooter _shooter;

    StaticConfig Cfg => Game.I ? Game.I.config : null;

    void Start()
    {
        if (Game.I && Game.I.player)
        {
            _stats = Game.I.player.GetComponent<PlayerStats>();
            _shooter = Game.I.player.GetComponent<AutoShooter>();
        }
    }

    void Update()
    {
        if (_paused) return;

        float now = Time.time;

        bool wasFreeze = now < _freezeUntil;
        bool wasFrenzy = now < _frenzyUntil;

        // авто-обнуление таймеров
        if (now > _rapidUntil) _rapidUntil = 0f;
        if (now > _frenzyUntil) _frenzyUntil = 0f;
        if (now > _shieldUntil) _shieldUntil = 0f;
        if (now > _freezeUntil) _freezeUntil = 0f;

        bool isFreeze = now < _freezeUntil;
        bool isShield = now < _shieldUntil;
        bool isFrenzy = now < _frenzyUntil;

        // --- Freeze overlay ---
        if (freezeOverlay)
        {
            if (isFreeze && !wasFreeze)
                freezeOverlay.Show(Mathf.Max(0.01f, _freezeUntil - now));
            if (!isFreeze && wasFreeze)
                freezeOverlay.Hide();
        }

        // --- Shield visual (инстанс из префаба) ---
        var shield = EnsureShieldInstance();
        if (shield && shield.activeSelf != isShield)
            shield.SetActive(isShield);

        // --- Окончание удвоения — гасим пульс ESSENCE ---
        if (!isFrenzy && wasFrenzy)
            Game.I?.hud?.PulseEssenceStop();
    }

    // =====================================================================================
    //  ПУБЛИЧНОЕ API
    // =====================================================================================

    public void SetPaused(bool paused)
    {
        _paused = paused;
    }

    public void Activate(PowerUpType type, float duration)
    {
        float d = Mathf.Max(0.01f, duration);
        float until = Time.time + d;

        if (logPowerUps)
            Debug.Log($"[PUM] Activate {type} for {d:0.00}s");

        switch (type)
        {
            case PowerUpType.RapidFire:
                _rapidUntil = Mathf.Max(_rapidUntil, until);
                break;

            case PowerUpType.ScoreFrenzy:
                _frenzyUntil = Mathf.Max(_frenzyUntil, until);
                // пульс ESSENCE на всё время действия
                Game.I?.hud?.PulseEssenceStart(d);
                break;

            case PowerUpType.Shield:
                _shieldUntil = Mathf.Max(_shieldUntil, until);
                break;

            case PowerUpType.Freeze:
                _freezeUntil = Mathf.Max(_freezeUntil, until);
                if (freezeOverlay) freezeOverlay.Show(d);
                break;

            case PowerUpType.FireRing:
                // мгновенный эффект — запускаем корутину и всё
                StartCoroutine(FireRingRoutine());
                break;
        }
    }

    public void ActivateRandom(float duration)
    {
        // Равновероятно 5 типов, FireRing тоже в рулетке
        float r = Random.value;
        PowerUpType t =
            r < 0.20f ? PowerUpType.RapidFire :
            r < 0.40f ? PowerUpType.ScoreFrenzy :
            r < 0.60f ? PowerUpType.Shield :
            r < 0.80f ? PowerUpType.Freeze :
                        PowerUpType.FireRing;

        if (logPowerUps)
            Debug.Log($"[PUM] Random picked: {t}");

        Activate(t, duration);
    }

    // --- статус баффов ---
    public bool IsActive(PowerUpType type)
    {
        float now = Time.time;
        return type switch
        {
            PowerUpType.RapidFire => now < _rapidUntil,
            PowerUpType.ScoreFrenzy => now < _frenzyUntil,
            PowerUpType.Shield => now < _shieldUntil,
            PowerUpType.Freeze => now < _freezeUntil,
            PowerUpType.FireRing => false,      // мгновенный
            _ => false
        };
    }

    public bool RapidFireActive => IsActive(PowerUpType.RapidFire);
    public bool ScoreFrenzyActive => IsActive(PowerUpType.ScoreFrenzy);
    public bool ShieldActive => IsActive(PowerUpType.Shield);
    public bool FreezeActive => IsActive(PowerUpType.Freeze);

    // --- мультипликаторы для стрельбы / лута ---
    public float GetFireRateMul()
    {
        float mul = Cfg ? Cfg.rapidFireRateMul : 2f;
        return RapidFireActive ? mul : 1f;
    }

    public float GetEssenceMul()
    {
        float mul = Cfg ? Cfg.scoreFrenzyMultiplier : 2f;
        return ScoreFrenzyActive ? mul : 1f;
    }

    public float GetDamageMul() => 1f; // пока без баффа на урон

    // =====================================================================================
    //  ВНУТРЕННЕЕ
    // =====================================================================================

    // один раз создаём щит из префаба и цепляем к игроку
    GameObject EnsureShieldInstance()
    {
        if (_shieldInstance != null) return _shieldInstance;
        if (shieldPrefab == null) return null;
        if (Game.I == null || Game.I.player == null) return null;

        _shieldInstance = Instantiate(shieldPrefab, Game.I.player.transform);
        _shieldInstance.transform.localPosition = Vector3.zero;
        _shieldInstance.transform.localRotation = Quaternion.identity;
        _shieldInstance.SetActive(false);
        return _shieldInstance;
    }

    IEnumerator FireRingRoutine()
    {
        var cfg = Cfg;
        if (cfg == null || Game.I == null || Game.I.player == null) yield break;
        if (_shooter == null || _shooter.projectilePrefab == null) yield break;

        int bursts = Mathf.Max(1, cfg.ringBursts);
        int perBurst = Mathf.Max(1, cfg.ringProjectilesPerBurst);
        float interval = Mathf.Max(0f, cfg.ringBurstInterval);
        float projSpeed = Mathf.Max(0.1f, cfg.ringProjectileSpeed);
        float projDamage = Mathf.Max(0.1f, cfg.ringDamage);

        Transform center = Game.I.player.transform;

        for (int b = 0; b < bursts; b++)
        {
            float step = 360f / perBurst;

            for (int i = 0; i < perBurst; i++)
            {
                float angleRad = (step * i) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
                Vector3 spawnPos = center.position + dir * 0.5f;

                var go = Instantiate(_shooter.projectilePrefab,
                                     spawnPos,
                                     Quaternion.LookRotation(dir, Vector3.up));

                var proj = go.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.speed = projSpeed;
                    proj.Init(spawnPos + dir * 10f, projDamage);
                }
            }

            if (interval > 0f && b < bursts - 1)
                yield return new WaitForSeconds(interval);
        }
    }
}
