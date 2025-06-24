using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private LayerMask enemyLayer;

    public void Initialize(Vector3 direction, float speed, float damage, LayerMask enemyLayer)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.enemyLayer = enemyLayer;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Simple collision detection
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, enemyLayer);
        if (hit != null && hit.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }

        // Destroy if out of range (e.g., 10 units)
        if (Vector3.Distance(transform.position, Vector3.zero) > 10f)
        {
            Destroy(gameObject);
        }
    }
}
