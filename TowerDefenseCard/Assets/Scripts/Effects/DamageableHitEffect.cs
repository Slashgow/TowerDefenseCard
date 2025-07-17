using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(IDamageable))]
public class DamageableHitEffect : MonoBehaviour
{
    [SerializeField] private List<Effect> effects;

    private IDamageable damageable;

    private void Awake() => damageable = GetComponent<IDamageable>();
    private void OnEnable() => damageable.OnTakeDamage += Damageable_OnTakeDamage;
    private void OnDisable() => damageable.OnTakeDamage -= Damageable_OnTakeDamage;
    private void Damageable_OnTakeDamage(float currentHealth)
    {
        foreach (Effect effect in effects)
        {
            effect.DoEffect();
        }
    }
}
