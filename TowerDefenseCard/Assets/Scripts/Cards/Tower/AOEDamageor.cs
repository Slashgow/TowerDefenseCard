using UnityEngine;

public class AOEDamageor : CardBaseDamageor
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0f, 20f)] private float projectileSpeed = 5f;

    protected override void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);
        if (hits.Length > 0)
        {
            Collider2D target = hits[0]; // Simplest: target the first enemy in range
            if (target.TryGetComponent<IDamageable>(out var damageable)) // Only targets IDamageable (enemies)
            {
                ApplyDoT(hits[0].gameObject);

                Debug.Log($"hit  {target.name} - damageable");
                Vector3 direction = (target.transform.position - transform.position).normalized;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust -90f for 2D up vector

                GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.Initialize(direction, projectileSpeed, Damage, enemyLayer, false, AttackArea, impactEffectPrefab);
            }
        }
    }
}
