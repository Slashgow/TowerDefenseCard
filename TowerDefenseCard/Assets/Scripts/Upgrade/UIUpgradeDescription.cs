using TMPro;
using UnityEngine;

public class UIUpgradeDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Transform cardUIParent;

    public void Init(CardID cardID)
    {
        CardData cardData = CardManager.Instance.GetCardPrefabByCardID(cardID).CardData;

        descriptionText.text = cardData.CardDescription.GetLocalizedString();
        GameObject cardUIGameObject = Instantiate(cardUIPrefab, cardUIParent);
        cardUIGameObject.GetComponent<UICardMenu>().SetupUICardMenu(cardData, true);
    }
}
