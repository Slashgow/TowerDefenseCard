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
                Debug.Log($"hit  {target.name} - damageable");
                Vector3 direction = (target.transform.position - transform.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.Initialize(direction, projectileSpeed, Damage, enemyLayer, false, cardDamageorData.AttackArea);
            }
        }
    }
}
