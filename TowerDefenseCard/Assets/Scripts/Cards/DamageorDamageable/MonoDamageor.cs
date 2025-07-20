using System;
using UnityEngine;

public class MonoDamageor : BaseDamageor
{
    [Header("Projectile")]
    [SerializeField] private bool useProjectile = false;
    [SerializeField, Range(0, 10)] private int numberOfProjectilePerAttack = 1;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0f,20f)] private float projectileSpeed = 5f;

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
        Collider2D target = hits[0];
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            ApplyDoT(hits[0].gameObject);
            damageable.TakeDamage(Damage);
            Instantiate(impactEffectPrefab, target.transform.position, Quaternion.identity);
        }
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
                projectileScript.Initialize(direction, projectileSpeed, Damage, enemyLayer, true, AttackArea, impactEffectPrefab, AttackRange);
            }
        }
    }
}
