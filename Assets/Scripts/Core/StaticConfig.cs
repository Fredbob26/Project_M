using UnityEngine;

[CreateAssetMenu(fileName = "StaticConfig", menuName = "Config/StaticConfig")]
public class StaticConfig : ScriptableObject
{
    // ---------- Life / Timer ----------
    public float startSeconds = 60f;
    public float maxSeconds = 60f;
    public float essenceSeconds = 2f;
    public float contactDamageSeconds = 3f;
    public float contactDamageCooldown = 0.6f;

    // ---------- Player: base stats for PlayerStats.ResetFromConfig ----------
    public float baseMoveSpeed = 7f;           // базовая скорость игрока
    public float baseAttackSpeed = 1f;         // базовый множитель скорострельности (1 = без буста)
    public float baseDamage = 5f;              // базовый урон
    [Range(0f, 1f)] public float baseCritChance = 0.05f;
    [Range(0f, 1f)] public float baseRicochetChance = 0f;

    // ---------- Shooting config (используются AutoShooter/Projectile) ----------
    public float attackInterval = 0.45f;       // базовый интервал выстрела (сек)
    public float projectileSpeed = 22f;        // скорость пули
    public int projectileDamage = 5;           // базовый урон пули (до добавок PlayerStats)
    public float targetRadius = 20f;           // радиус поиска цели

    // ---------- Enemy ----------
    public int enemyBaseHP = 10;
    public float enemyBaseSpeed = 3f;

    // ---------- Arena ----------
    private BoxCollider _arenaBounds;
    public BoxCollider ArenaBounds
    {
        get
        {
            if (_arenaBounds == null)
            {
                var go = GameObject.FindGameObjectWithTag("LevelBounds");
                if (go) _arenaBounds = go.GetComponent<BoxCollider>();
            }
            return _arenaBounds;
        }
    }
}