using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 5f;     // страховка, чтобы пули не жили вечно
    [SerializeField] private bool pierceOne = false;  // позже можно апгрейдом
    private int damage = 1;

    private Transform target;
    private Vector3 travelDir;  // последнее направление
    private int piercesLeft = 0;

    public void Init(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        this.travelDir = (target != null) ? (target.position - transform.position).normalized
                                          : transform.forward;
        piercesLeft = pierceOne ? 1 : 0;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f) { Destroy(gameObject); return; }

        // если цель ещЄ жива Ч обновл€ем направление к ней (homing-lite)
        if (target != null)
        {
            travelDir = (target.position - transform.position).normalized;
        }
        // иначе летим по последнему направлению

        transform.position += travelDir * speed * Time.deltaTime;
        transform.forward = travelDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        // бьЄм ЋёЅќ√ќ врага, а не только исходную цель
        var hp = other.GetComponent<EnemyHealth>();
        if (hp == null) return;

        hp.TakeDamage(damage);

        if (piercesLeft > 0)
        {
            piercesLeft--;
            return; // пробиваем и летим дальше
        }

        Destroy(gameObject);
    }
}