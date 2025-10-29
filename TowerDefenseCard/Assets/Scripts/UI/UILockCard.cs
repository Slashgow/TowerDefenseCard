using UnityEngine;
using UnityEngine.UI;

public class UILockCard : MonoBehaviour
{
    [SerializeField] private Image lockImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color lockedBackgroundColor;
    public Image LockImage => lockImage;
    private CardID cardID;
    public void Setup(CardID cardID)
    {
        this.cardID = cardID;
        CardDiscoveryState cardDiscoveryState = CardManager.Instance.GetCardDiscoveryStateByCardID(cardID);
        if (cardDiscoveryState.isLocked)
        {
            lockImage.gameObject.SetActive(true);

            backgroundImage.color = lockedBackgroundColor;
        }

        else
            lockImage.gameObject.SetActive(false);
    }
}
