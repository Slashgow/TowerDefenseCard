using TMPro;
using UnityEngine;

public class EnnemyCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardTitleText;
    [SerializeField] private TextMeshProUGUI cardHealthText;

    private Ennemy ennemy;

    private void OnEnable()
    {
        ennemy = GetComponentInParent<Ennemy>();
        ennemy.OnTakeDamage += Ennemy_OnTakeDamage;
        SetupCard(ennemy.CardData.CardName, ennemy.MaxHealth);
    }

    private void OnDisable()
    {
        ennemy.OnTakeDamage -= Ennemy_OnTakeDamage;
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
