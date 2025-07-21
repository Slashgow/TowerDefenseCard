using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCurrencyCollecter : Card, IPointerUpHandler, IDropHandler
{
    [SerializeField] private Logger logger;
    [SerializeField, Range(0, 100)] private int maxCurrencyAmount = 50;
    [SerializeField, Range(0, 30)] private int spawnAmount = 10;
    [SerializeField] private Vector3 spawnOffset = Vector3.down * 3;

    public event Action<int> OnUpdateCurrentAmount;

    public int CurrentAmount { get; private set; }

    private void Awake()
    {
        CurrentAmount = 0;
    }

    public void CollectCurrency(Currency[] currencies)
    {
        if(CurrentAmount + currencies.Length > maxCurrencyAmount)
        {
            currencies[maxCurrencyAmount - CurrentAmount].transform.SetParent(null);
        }

        foreach (var currency in currencies)
        {
            if (CurrentAmount >= maxCurrencyAmount)
                return;

            CurrentAmount++;
            OnUpdateCurrentAmount?.Invoke(CurrentAmount);
            currency.Pool.AddToPool(currency.gameObject);
        }
    }

    public void DropCurrency()
    {
        float currentAmountOnDrop = CurrentAmount;
        if(CurrentAmount < spawnAmount)
        {
            for (int i = 0; i< currentAmountOnDrop; i++)
            {
                SpawnCurrency();
            }
        }
        else
        {
            for(int i = 0;i< spawnAmount; i++)
            {
                SpawnCurrency();
            }
        }
    }

    private void SpawnCurrency()
    {
        GameObject currencyGameObjectInstance = Reseller.Instance.CurrencyPool.GetPrefabFromPool(this.transform.position + spawnOffset);
        currencyGameObjectInstance.GetComponent<Currency>().Setup(Reseller.Instance.CurrencyPool);
        CurrentAmount--;
        CurrentAmount = Mathf.Clamp(CurrentAmount, 0, maxCurrencyAmount);
        OnUpdateCurrentAmount?.Invoke(CurrentAmount);
    }

    public void OnPointerUp(PointerEventData eventData) => DropCurrency();

    public void OnDrop(PointerEventData eventData)
    {
        Currency[] currencies = eventData.pointerDrag.GetComponentsInChildren<Currency>();

        if (currencies.Length <= 0)
            return;

        CollectCurrency(currencies);
    }
}
