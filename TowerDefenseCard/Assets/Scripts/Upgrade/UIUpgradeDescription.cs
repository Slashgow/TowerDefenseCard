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
#if UNITY_WEBGL
            cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedStringAsync().Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    descriptionText.text = handle.Result;
                }
            };
#endif

#if !UNITY_WEBGL
            descriptionText.text = cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedString();
#endif
        }
        else
        {
#if UNITY_WEBGL
            card.CardData.CardDescription.GetLocalizedStringAsync().Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    descriptionText.text = handle.Result;
                }
            };
#endif

#if !UNITY_WEBGL
            descriptionText.text = card.CardData.CardDescription.GetLocalizedString();
#endif
        }


        GameObject cardUIGameObject = Instantiate(cardUIPrefab, cardUIParent);
        cardUIGameObject.GetComponent<UICardMenu>().SetupUICardMenu(card.CardData, true);
    }
}
