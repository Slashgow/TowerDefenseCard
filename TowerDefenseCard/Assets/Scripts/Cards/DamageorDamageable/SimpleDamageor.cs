using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDamageor : BaseDamageor
{
    [SerializeField] private bool isMonoTarget = true;
    [SerializeField, Range(0,10)] private int maxNumberOfTargets = 1;

    [Header("Projectile")]
    [SerializeField] private bool useProjectile = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0, 10)] private int numberOfProjectilePerAttack = 1;
    [SerializeField, Range(0f, 40f)] private float projectileSpeed = 5f;

    public struct DamageableTarget
    {
        public Collider2D collider;
        public IDamageable damageable;

        public DamageableTarget(Collider2D collider, IDamageable damageable)
        {
            this.collider = collider;
            this.damageable = damageable;
        }
    }

    protected override void Attack()
    {
        if (!CanAttack)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);
        List<DamageableTarget> targets = CardUtility.GetDamageableTargets(hits);
        if (targets.Count > 0)
        {
            OnLaunchAttack();

            if (useProjectile)
                SetupProjectiles(targets);
            else
                AttackImmediately(targets);
        }
    }

    private void AttackImmediately(List<DamageableTarget> targets)
    {
        if (isMonoTarget)
        {
            HitDamageable(targets[0].collider, targets[0].damageable);
        }
        else
        {
            int targetCount = Mathf.Min(targets.Count, maxNumberOfTargets);

            for (int i = 0; i < targetCount; i++)
            {
                HitDamageable(targets[i].collider, targets[i].damageable);
            }
        }
    }

    private void HitDamageable(Collider2D hit, IDamageable damageable)
    {
        ApplyDoT(hit.gameObject);
        OnAttack(hit.GetComponent<BaseDamageable>());
        damageable.TakeDamage(Damage);
        Instantiate(impactEffectPrefab, hit.transform.position, Quaternion.identity);
    }


    private void SetupProjectiles(List<DamageableTarget> targets)
    {
        int projectileCount = Mathf.Min(numberOfProjectilePerAttack, targets.Count);

        for (int i = 0; i < projectileCount; i++)
        {
            DamageableTarget target = targets[i];
            ApplyDoT(target.collider.gameObject);

            Vector3 direction = (target.collider.transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust -90f for 2D up vector

            GameObject projectileGameObjectInstance = Instantiate(projectilePrefab, transform.position, rotation);
            Projectile projectileInstance = projectileGameObjectInstance.GetComponent<Projectile>();
            projectileInstance.OnProjectileHit -= ProjectileInstance_OnProjectileHit;
            projectileInstance.OnProjectileHit += ProjectileInstance_OnProjectileHit;
            projectileInstance.Initialize(direction, projectileSpeed, Damage, enemyLayer, isMonoTarget, AttackArea, impactEffectPrefab, AttackRange);
        }
    }

    private void ProjectileInstance_OnProjectileHit(BaseDamageable baseDamageable)
    {
        OnAttack(baseDamageable);
    }
}
