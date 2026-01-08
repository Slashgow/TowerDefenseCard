using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class DotArea : MonoBehaviour
{
    private float areaRadius;
    private Color warningColor;
    private LayerMask damageableLayer;
    private bool isActive = false;
    private float damage;
    private float tickRate;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D areaCollider;
    private List<IDamageable> damageablesInArea = new List<IDamageable>();
    private Timer damageTimer;
    private Timer pulseTimer;

    public void Initialize(float radius, Color warningColor, LayerMask damageableLayer)
    {
        this.areaRadius = radius;
        this.warningColor = warningColor;
        this.damageableLayer = damageableLayer;

        // Get or add components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 5;
        }

        spriteRenderer.color = warningColor;

        // Add collider for detection
        areaCollider = gameObject.AddComponent<CircleCollider2D>();
        areaCollider.radius = radius / transform.localScale.x; // Adjust for scale
        areaCollider.isTrigger = true;

        // Pulse effect during warning phase
        StartPulseWarning();
    }

    public void Activate(float duration, float damage, float tickRate, Color activeColor)
    {
        this.damage = damage;
        this.tickRate = tickRate;
        this.isActive = true;

        // Change visual to active state
        if (spriteRenderer != null)
        {
            spriteRenderer.color = activeColor;
        }

        pulseTimer?.Cancel();
        transform.localScale = Vector3.one * areaRadius * 2f;
        StartDamageOverTime();
    }

    private void StartPulseWarning()
    {
        float pulseSpeed = 2f;
        float startTime = Time.time;

        pulseTimer = Timer.Register(
            float.MaxValue,
            onComplete: null,
            onUpdate: (timer) =>
            {
                if (isActive) 
                    return;

                float elapsed = Time.time - startTime;
                float pulse = (Mathf.Sin(elapsed * pulseSpeed) + 1f) / 2f;
                Color currentColor = warningColor;
                currentColor.a = Mathf.Lerp(0.3f, 0.8f, pulse);

                if (spriteRenderer != null)
                    spriteRenderer.color = currentColor;

                // Scale pulse
                float scalePulse = Mathf.Lerp(0.95f, 1.05f, pulse);
                transform.localScale = Vector3.one * areaRadius * 2f * scalePulse;
            },
            isLooped: false,
            useRealTime: false
        );
    }

    private void StartDamageOverTime()
    {
        DealDamage();

        damageTimer = Timer.Register(tickRate,onComplete: () => DealDamage(), isLooped: true, useRealTime: false);
    }

    private void DealDamage()
    {
        if (!isActive) 
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, areaRadius, damageableLayer);

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);

                //// Optional: spawn damage effect
                //SpawnDamageEffect(hit.transform.position);
            }
        }
    }

    private void SpawnDamageEffect(Vector3 position)
    {
        // Optional: create small visual feedback when damage is dealt
        GameObject effectObj = new GameObject("DotEffect");
        effectObj.transform.position = position;

        SpriteRenderer effectRenderer = effectObj.AddComponent<SpriteRenderer>();
        effectRenderer.sprite = spriteRenderer?.sprite;
        effectRenderer.color = new Color(1f, 0f, 0f, 0.8f);
        effectRenderer.sortingOrder = 10;
        effectObj.transform.localScale = Vector3.one * 0.5f;

        // Fade out and destroy
        FadeOutEffect(effectObj, 0.3f);
    }

    private void FadeOutEffect(GameObject effect, float duration)
    {
        SpriteRenderer renderer = effect.GetComponent<SpriteRenderer>();
        Color startColor = renderer.color;
        float startTime = Time.time;
        Vector3 startScale = effect.transform.localScale;

        Timer.Register(
            duration,
            onUpdate: (timer) =>
            {
                if (effect == null || renderer == null) return;

                float elapsed = Time.time - startTime;
                float progress = elapsed / duration;

                float alpha = Mathf.Lerp(startColor.a, 0f, progress);
                renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                // Scale up slightly
                effect.transform.localScale = Vector3.Lerp(startScale, Vector3.one * 1.5f, progress);
            },
            onComplete: () =>
            {
                if (effect != null)
                    Destroy(effect);
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && !damageablesInArea.Contains(damageable))
        {
            damageablesInArea.Add(damageable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && damageablesInArea.Contains(damageable))
        {
            damageablesInArea.Remove(damageable);
        }
    }

    private void OnDestroy()
    {
        damageTimer?.Cancel();
        pulseTimer?.Cancel();
        damageablesInArea.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.red : Color.gray;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}