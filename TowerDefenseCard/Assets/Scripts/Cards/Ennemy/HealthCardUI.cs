using TMPro;
using UnityEngine;

public class HealthCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardHealthText;

    private IDamageable damageable;
    private Card card;
    private void OnEnable()
    {
        card = GetComponentInParent<Card>();
        damageable = card.GetComponent<IDamageable>();
        damageable.OnTakeDamage += Ennemy_OnTakeDamage;
        SetupCard(damageable.MaxHealth);
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= Ennemy_OnTakeDamage;
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
