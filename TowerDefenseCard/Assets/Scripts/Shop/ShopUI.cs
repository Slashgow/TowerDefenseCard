using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI yenCoinText;

    private void OnEnable()
    {
        ShopManager.Instance.OnUpdatePlayerCoin += ShopManager_OnUpdatePlayerCoin;
    }

    private void OnDisable()
    {
        ShopManager.Instance.OnUpdatePlayerCoin -= ShopManager_OnUpdatePlayerCoin;
    }

    private void ShopManager_OnUpdatePlayerCoin(int currentPlayerCoin)
    {
        UpdateCurrencyDisplay(currentPlayerCoin);
    }

    private void UpdateCurrencyDisplay(int currentPlayerCoin)
    {
        if (yenCoinText != null)
        {
            yenCoinText.text = $"{currentPlayerCoin}";
        }
    }
}
