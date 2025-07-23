using TMPro;
using UnityEngine;

public class HealthCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardHealthText;

    private IDamageable damageable;
    private IHealable healable;
    private Card card;
    private void OnEnable()
    {
        card = GetComponentInParent<Card>();
        damageable = card.GetComponent<IDamageable>();
        healable = card.GetComponent<IHealable>();
        damageable.OnTakeDamage += Ennemy_OnTakeDamage;
        healable.OnHeal += Healable_OnHeal;
        SetupCard(damageable.MaxHealth);
    }

    private void Healable_OnHeal(float currentHealth)
    {
        cardHealthText.text = currentHealth.ToString();
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= Ennemy_OnTakeDamage;
        healable.OnHeal -= Healable_OnHeal;
    }
    private void Ennemy_OnTakeDamage(float currentHealth)
    {
        cardHealthText.text = currentHealth.ToString();
    }

    public void SetupCard(float currentHealth)
    {
        cardHealthText.text = currentHealth.ToString();
    }
}
