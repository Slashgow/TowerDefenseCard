using TMPro;
using UnityEngine;

public class UICardCurrencyCollecter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentAmountText;
    [SerializeField] private CardCurrencyCollecter currencyCollecter;

    private void OnEnable() => currencyCollecter.OnUpdateCurrentAmount += CurrencyCollecter_OnUpdateCurrentAmount;
    private void OnDisable() => currencyCollecter.OnUpdateCurrentAmount -= CurrencyCollecter_OnUpdateCurrentAmount;

    private void Start()
    {
        currentAmountText.text = currencyCollecter.CurrentAmount.ToString();
    }
    private void CurrencyCollecter_OnUpdateCurrentAmount(int currentAmount)
    {
        currentAmountText.text = currentAmount.ToString();
    }
}
