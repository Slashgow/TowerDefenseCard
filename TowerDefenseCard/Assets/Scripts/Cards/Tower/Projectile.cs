using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private LayerMask enemyLayer;
    private bool isMonoTarget;
    private float attackArea;
    private float attackRange;
    private GameObject impactEffectPrefab;

    public void Initialize(Vector3 direction, float speed, float damage, LayerMask enemyLayer, bool isMonoTarget, float attackArea, GameObject impactEffectPrefab, float attackRange)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.enemyLayer = enemyLayer;
        this.isMonoTarget = isMonoTarget;
        this.attackArea = attackArea;
        this.impactEffectPrefab = impactEffectPrefab;
        this.attackRange = attackRange;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;


        Collider2D singleHit = Physics2D.OverlapCircle(transform.position, 0.1f, enemyLayer);
        if (singleHit != null && singleHit.TryGetComponent<IDamageable>(out var singleDamageable))
        {
            if (isMonoTarget)
            {
                singleDamageable.TakeDamage(damage);
                Instantiate(impactEffectPrefab, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackArea, enemyLayer);
                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        if (hit.TryGetComponent<IDamageable>(out var damageable))
                        {
                            damageable.TakeDamage(damage);
                            Instantiate(impactEffectPrefab, hit.transform.position, Quaternion.identity);
                        }
                    }
                    Destroy(gameObject);
                }
            }
        }
        

        // Destroy if out of range (e.g., 10 units)
        if (Vector3.Distance(transform.position, Vector3.zero) > attackRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackArea);
    }
}
