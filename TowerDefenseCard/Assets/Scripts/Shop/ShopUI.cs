using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private TextMeshProUGUI yenCoinText;
    [SerializeField] private Button purchaseButton;

    private void Awake()
    {
        shopManager.OnUpdatePlayerCoin += ShopManager_OnUpdatePlayerCoin;

        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(shopManager.TryPurchaseWeightedCard);
    }

    private void OnDisable()
    {
        shopManager.OnUpdatePlayerCoin -= ShopManager_OnUpdatePlayerCoin;

        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(shopManager.TryPurchaseWeightedCard);
    }

    private void ShopManager_OnUpdatePlayerCoin(int currentPlayerCoin)
    {
        UpdateCurrencyDisplay(currentPlayerCoin);
    }

    private void UpdateCurrencyDisplay(int currentPlayerCoin)
    {
        if (yenCoinText != null)
        {
            yenCoinText.text = $"Yen Coins: {currentPlayerCoin}";
        }
    }
}
