using UnityEngine;
public class UIDamageable : UISpawner
{
    [SerializeField] Logger logger;

    private void Awake() => BaseDamageable.OnAnyDamageableTakeDamage += IDamageable_OnAnyDamageableTakeDamage;
    private void OnDestroy() => BaseDamageable.OnAnyDamageableTakeDamage -= IDamageable_OnAnyDamageableTakeDamage;

    private void IDamageable_OnAnyDamageableTakeDamage(float damage, Vector3 worldPosition)
    {
        logger.Log($"Take {damage} damage", this);

        GameObject instance = GetSpawnTextAbove(worldPosition, damage.ToString());
        instance.GetComponent<UIDamage>().Setup(uiPrefabPool);
    }
}
