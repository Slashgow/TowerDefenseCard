using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Currency : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI amountText;
    public int Amount {  get; private set; }

    public void Init(int amount)
    {
        Amount = amount;
        amountText.text = Amount.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ShopManager.Instance.AddPlayerCoin(Amount);
        Destroy(this.gameObject);
    }
}
