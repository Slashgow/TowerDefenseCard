using TMPro;
using UnityEngine;

public class EnnemyCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardTitleText;
    [SerializeField] private TextMeshProUGUI cardHealthText;

    private IDamageable damageable;
    private Card card;
    private void OnEnable()
    {
        card = GetComponentInParent<Card>();
        damageable = card.GetComponent<IDamageable>();
        damageable.OnTakeDamage += Ennemy_OnTakeDamage;
        SetupCard(card.CardData.CardName, damageable.MaxHealth);
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= Ennemy_OnTakeDamage;
    }
    private void Ennemy_OnTakeDamage(float currentHealth)
    {
        cardHealthText.text = currentHealth.ToString();
    }

    public void SetupCard(string cardTitle, float currentHealth)
    {
        cardTitleText.text = cardTitle;
        cardHealthText.text = currentHealth.ToString();
    }
}
