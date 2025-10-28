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

    // ---------- Player base stats ----------
    public float baseMoveSpeed = 7f;
    public float baseAttackSpeed = 1f;   // множитель выстрелов/сек
    public float baseDamage = 5f;
    [Range(0f, 1f)] public float baseCritChance = 0.05f;
    [Range(0f, 1f)] public float baseRicochetChance = 0f;

    // ---------- Shooting ----------
    public float projectileSpeed = 22f;
    public int projectileDamage = 5;
    public float targetRadius = 20f;

    // ---------- Enemies ----------
    public int enemyBaseHP = 10;
    public float enemyBaseSpeed = 3f;

    // ---------- Power-ups ----------
    // RapidFire → в 2 раза быстрее и 3 выстрела веером
    public float rapidFireDuration = 10f;
    public float rapidFireSpreadDeg = 10f;     // угол отклонения влево/вправо
    public float rapidFireFireRateMul = 2f;    // множитель скорострельности

    public float shieldDuration = 5f;          // не тикает таймер от контакта
    public float freezeDuration = 10f;         // фризит врагов

    // ScoreFrenzy: даёт Х2 очков за ЭССЕНЦИИ (только в счёт, не в валюту)
    public float scoreFrenzyDuration = 10f;
    public float scoreFrenzyMultiplier = 2f;

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
