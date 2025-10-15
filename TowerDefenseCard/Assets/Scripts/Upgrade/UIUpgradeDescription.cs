using TMPro;
using UnityEngine;

public class UIUpgradeDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Transform cardUIParent;

    public void Init(CardID cardID)
    {
        Card card = CardManager.Instance.GetCardPrefabByCardID(cardID);

        CardUpgrade cardUpgrade = (CardUpgrade)card;

        if(cardUpgrade != null)
        {
            descriptionText.text = cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedString();
        }
        else
        {
            descriptionText.text = card.CardData.CardDescription.GetLocalizedString();
        }

            
        GameObject cardUIGameObject = Instantiate(cardUIPrefab, cardUIParent);
        cardUIGameObject.GetComponent<UICardMenu>().SetupUICardMenu(card.CardData, true);
    }
}
