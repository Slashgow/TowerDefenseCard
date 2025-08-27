using System;
using UnityEngine.Events;

public class PlayerHealth : BaseDamageable, ILoadable, ISavable
{
    public static event Action OnPlayerDie;
    public static event Action OnPlayerHit;
    public UnityEvent OnPlayerTakeDamageUnity;

    protected override void Awake()
    {

    }
    public override void TakeDamage(float damage)
    {
        OnPlayerTakeDamageUnity?.Invoke();
        OnPlayerHit?.Invoke();
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
        OnPlayerDie?.Invoke();
    }

    public void Load(GameSaveData gameSaveData)
    {
        currentHealth = gameSaveData.currentPlayerHealth;
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.currentPlayerHealth = currentHealth;
    }
}
