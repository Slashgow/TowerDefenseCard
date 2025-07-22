using UnityEngine;

[RequireComponent(typeof(IDamageable))]
public class Ennemy : Card, ICurrencyDropper
{    
    private int currencyAmountToDrop;
    public int CurrencyAmountToDrop => currencyAmountToDrop;
    private IDamageable damageable;

    private void Awake()
    {
        damageable = GetComponent<IDamageable>();
        damageable.OnDie += Damageable_OnDie;
        currencyAmountToDrop = CardData.Cost;
    }

    private void OnDestroy() => damageable.OnDie -= Damageable_OnDie;

    private void Damageable_OnDie() => DropCurrency();

    public void DropCurrency()
    {
        for (int i = 0; i < CurrencyAmountToDrop; i++)
        {
            GameObject currencyGameObjectInstance = Reseller.Instance.CurrencyPool.GetPrefabFromPool();
            currencyGameObjectInstance.GetComponent<Currency>().Setup(Reseller.Instance.CurrencyPool);
            currencyGameObjectInstance.transform.SetParent(null);
            currencyGameObjectInstance.transform.position = this.transform.position;
        }
        
        ShopManager.Instance.AddPlayerCoin(CurrencyAmountToDrop);
    }
}
