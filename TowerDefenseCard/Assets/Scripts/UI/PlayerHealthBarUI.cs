using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;
    private void OnEnable()
    {
        playerHealth.OnPlayerTakeDamageUnity.AddListener(UpdateHealthBar);
    }

    private void Start()
    {
        UpdateHealthBar();
    }
    private void OnDisable()
    {
        playerHealth.OnPlayerTakeDamageUnity.RemoveListener(UpdateHealthBar);
    }
    private void UpdateHealthBar()
    {
        if (fillImage != null)
            fillImage.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }
}
