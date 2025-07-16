using UnityEngine;


[RequireComponent(typeof(IDamageable))]
public class HitShaker : Shaker
{
    private IDamageable damageable;
 
    private void Awake() => damageable = GetComponent<IDamageable>();
    private void OnEnable() => damageable.OnTakeDamage += Damageable_OnTakeDamage;
    private void OnDisable() => damageable.OnTakeDamage -= Damageable_OnTakeDamage;
    private void Damageable_OnTakeDamage(float currentHealth) => Shake();
    
}