using UnityEngine;

[RequireComponent(typeof(IDamageable))]
public class Ennemy : Card, ICurrencyDropper
{
    [SerializeField] private Currency currencyPrefab;
    
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
        GameObject currencyGameObjectInstance = Instantiate(currencyPrefab.gameObject, this.transform.position, Quaternion.identity);
        currencyGameObjectInstance.GetComponent<Currency>().Init(currencyAmountToDrop);
    }
}
