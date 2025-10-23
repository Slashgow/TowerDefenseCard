using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class UpgradeSlotSaveData
{
    public bool isEmpty;
    public CardID upgradeCardID;
}

public class UIUpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image backgroundCircle;
    [SerializeField] private Color emptyColor, filledColor;
    [SerializeField] private Vector3 outOffset;
    [SerializeField] private Image outline;

    public bool IsEmpty { get; private set; }
    private CardUpgrade upgradeCard;

    public static event Action<Vector3, CardID> OnHoverEnter;
    public static event Action OnHoverExit;
    public static event Action OnClick;


    private void Awake()
    {
        outline.enabled = false;
        EmptySlot();
    }

    public void FillSlot(CardUpgrade upgradeCard)
    {
        this.upgradeCard = upgradeCard;
        IsEmpty = false;
        backgroundCircle.color = filledColor;
    }

    public void EmptySlot()
    {
        this.upgradeCard = null;
        IsEmpty = true;
        backgroundCircle.color = emptyColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsEmpty)
            return;

        OnClick?.Invoke();
        this.upgradeCard.transform.localScale = Vector3.one;
        this.upgradeCard.transform.position = this.transform.position + outOffset;
        this.upgradeCard.GetComponent<CardMoverUpgrade>().enabled = true;
 
        this.upgradeCard.OnUnstack();
        //this.upgradeCard.gameObject.SetActive(true);
        EmptySlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;

        if(IsEmpty)
            return;

        OnHoverEnter?.Invoke(this.transform.position, this.upgradeCard.CardData.CardID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;

        if(IsEmpty)
            return;

        OnHoverExit?.Invoke();
    }

    public UpgradeSlotSaveData Save()
    {
        return new UpgradeSlotSaveData
        {
            isEmpty = this.IsEmpty,
            upgradeCardID = this.IsEmpty ? CardID.BAMBOO : this.upgradeCard.CardData.CardID
        };
    }

    public void Load(UpgradeSlotSaveData upgradeSlotSaveData)
    {
        this.IsEmpty = upgradeSlotSaveData.isEmpty;

        if (this.IsEmpty)
            EmptySlot();
        else
        {
            CardUpgrade loadedUpgradeCardPrefab = (CardUpgrade) CardManager.Instance.GetCardPrefabByCardID(upgradeSlotSaveData.upgradeCardID);

            if (loadedUpgradeCardPrefab != null)
            {
                CardUpgrade upgradeCardInstance = Instantiate(loadedUpgradeCardPrefab, new Vector3(2500f, 2500f, 2500f), Quaternion.identity);
                Card card = this.GetComponentInParent<Card>();
                upgradeCardInstance.OnStack(card);
                upgradeCardInstance.transform.position = new Vector3(2500f, 2500f, 2500f);
                FillSlot(upgradeCardInstance);
            }
          
            else
            {
                Debug.LogWarning($"Could not find CardUpgrade with ID: {upgradeSlotSaveData.upgradeCardID}");
                EmptySlot();
            }
        }
    }
}
