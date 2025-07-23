using System;
using UnityEngine;
public class UIDamageable : UISpawner
{
    [SerializeField] Logger logger;

    private void Awake()
    {
        BaseDamageable.OnAnyDamageableTakeDamage += IDamageable_OnAnyDamageableTakeDamage;
        BaseDamageable.OnAnyHealableHealed += IHealble_OnAnyHealableHealed;
    }

    private void OnDestroy()
    {
        BaseDamageable.OnAnyDamageableTakeDamage -= IDamageable_OnAnyDamageableTakeDamage;
        BaseDamageable.OnAnyHealableHealed -= IHealble_OnAnyHealableHealed;
    }

    private void IHealble_OnAnyHealableHealed(float healAmount, Vector3 worldPosition)
    {
        logger.Log($" Heal {healAmount}", this);

        GameObject instance = GetSpawnTextAbove(worldPosition, healAmount.ToString(), healColor);
        instance.GetComponent<UIDamage>().Setup(uiPrefabPool);
    }
    private void IDamageable_OnAnyDamageableTakeDamage(float damage, Vector3 worldPosition)
    {
        logger.Log($"Take {damage} damage", this);

        GameObject instance = GetSpawnTextAbove(worldPosition, damage.ToString(), damageColor);
        instance.GetComponent<UIDamage>().Setup(uiPrefabPool);
    }
}
