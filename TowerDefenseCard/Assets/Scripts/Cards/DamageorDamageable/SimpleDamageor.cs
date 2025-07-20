using System;
using UnityEngine;

public class SimpleDamageor : BaseDamageor
{
    [SerializeField] private bool isMonoTarget = true;

    [Header("Projectile")]
    [SerializeField] private bool useProjectile = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0, 10)] private int numberOfProjectilePerAttack = 1;
    [SerializeField, Range(0f, 20f)] private float projectileSpeed = 5f;

    protected override void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);
        if (hits.Length > 0)
        {
            if (useProjectile)
                SetupProjectiles(hits);
            else
                AttackImmediately(hits);
        }
    }

    private void AttackImmediately(Collider2D[] hits)
    {
        if (isMonoTarget)
        {
            if (hits[0].TryGetComponent<IDamageable>(out var damageable))
                HitDamageable(hits[0], damageable);
        }
        else
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(hits[0].transform.position, AttackArea, enemyLayer);

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].TryGetComponent<IDamageable>(out var damageable))
                    HitDamageable(targets[i], damageable);
            }
        }
    }

    private void HitDamageable(Collider2D hit, IDamageable damageable)
    {
        ApplyDoT(hit.gameObject);
        damageable.TakeDamage(Damage);
        Instantiate(impactEffectPrefab, hit.transform.position, Quaternion.identity);
    }


    private void SetupProjectiles(Collider2D[] hits)
    {
        for (int i = 0; i < numberOfProjectilePerAttack; i++)
        {
            if (i >= hits.Length)
                return;

            Collider2D target = hits[i];
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                ApplyDoT(hits[0].gameObject);

                Vector3 direction = (target.transform.position - transform.position).normalized;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust -90f for 2D up vector

                GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.Initialize(direction, projectileSpeed, Damage, enemyLayer, isMonoTarget, AttackArea, impactEffectPrefab, AttackRange);
            }
        }
    }
}
