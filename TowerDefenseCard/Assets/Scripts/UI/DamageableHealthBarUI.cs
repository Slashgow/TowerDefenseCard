using UnityEngine;
using UnityEngine.UI;

public class DamageableHealthBarUI : MonoBehaviour
{
    [SerializeField] private BaseDamageable damageable;
    [SerializeField] private Image fillImage;
    private void OnEnable() => damageable.OnTakeDamage += UpdateHealthBar;
    private void OnDisable() => damageable.OnTakeDamage -= UpdateHealthBar;
    private void Start() => UpdateHealthBar(damageable.CurrentHealth);

    private void UpdateHealthBar(float currentHealth)
    {
        if (fillImage != null)
            fillImage.fillAmount = currentHealth / damageable.MaxHealth;
    }
}
