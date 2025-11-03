using UnityEngine;

[CreateAssetMenu(fileName = "StaticConfig", menuName = "Config/StaticConfig")]
public class StaticConfig : ScriptableObject
{
    // ---------- Life / Timer ----------
    [Header("Life / Timer")]
    public float startSeconds = 60f;
    public float maxSeconds = 60f;
    public float essenceSeconds = 2f;
    public float contactDamageSeconds = 3f;
    public float contactDamageCooldown = 0.6f;

    [Header("Hit flash (HUD)")]
    public float hitFlashDuration = 0.25f;
    public Color hitFlashColor = new Color(1f, 0f, 0f, 0.35f);

    // ---------- Player base ----------
    [Header("Player base")]
    public float baseMoveSpeed = 7f;
    public float baseAttackSpeed = 1f;
    public float baseDamage = 5f;
    [Range(0f, 1f)] public float baseCritChance = 0.05f;
    [Range(0f, 1f)] public float baseRicochetChance = 0f;

    // ---------- Shooting ----------
    [Header("Shooting")]
    public float attackInterval = 0.45f;
    public float projectileSpeed = 22f;
    public int projectileDamage = 5;
    public float targetRadius = 20f;

    // ---------- Enemy base ----------
    [Header("Enemy base")]
    public int enemyBaseHP = 10;
    public float enemyBaseSpeed = 3.5f;

    [Header("Enemy type multipliers")]
    public float tankHpMul = 3f;
    public float tankSpeedMul = 0.6f;
    public float bonusHpMul = 0.8f;
    public float bonusSpeedMul = 1.2f;

    [Header("Boss")]
    public int bossBaseHP = 300;
    public int bossHpGrowth = 50;
    public float bossSpeed = 2.5f;
    public int bossEssenceReward = 10;

    // ---------- PowerUps ----------
    [Header("PowerUps: RapidFire")]
    public float rapidFireDuration = 10f;
    public float rapidFireRateMul = 2f;
    public float rapidSpreadDeg = 12f;

    [Header("PowerUps: ScoreFrenzy")]
    public float scoreFrenzyDuration = 10f;
    public float scoreFrenzyMultiplier = 2f;

    [Header("PowerUps: Shield")]
    public float shieldDuration = 5f;

    [Header("PowerUps: Freeze")]
    public float freezeDuration = 10f;

    [Header("PowerUps: FireRing (bursts)")]
    public bool includeFireRingInRandom = true;
    [Min(1)] public int ringBursts = 3;
    [Min(1)] public int ringProjectilesPerBurst = 36;
    [Min(0f)] public float ringBurstInterval = 0.15f;
    [Min(0.1f)] public float ringProjectileSpeed = 18f;
    [Min(1f)] public float ringDamage = 5f;

    // ---------- Masks ----------
    [Header("Masks")]
    public LayerMask losBlockMask;
    public LayerMask obstacleMask;

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
