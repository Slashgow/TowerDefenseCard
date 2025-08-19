using UnityEngine;
using UnityEngine.UI;

public class UIButtonCardMenu : MonoBehaviour
{
    [SerializeField] private UIInput uIInput;
    [SerializeField] private Button buttonCollectionMenu;
    [SerializeField] private Image notificationImage;
    [SerializeField] private UIPage collectionMenu;

    private void Awake()
    {
        notificationImage.gameObject.SetActive(false);
        buttonCollectionMenu.onClick.AddListener(ShowCollectionMenu);
        uIInput.OnShowPageCollection += UIInput_OnShowPageCollection;
    }

    private void Start()
    {
        CardManager.Instance.OnDiscoverNewCard += CardManager_OnDiscoverNewCard;
    }

    private void OnDestroy()
    {
        if(CardManager.HasInstance)
            CardManager.Instance.OnDiscoverNewCard -= CardManager_OnDiscoverNewCard;

        buttonCollectionMenu.onClick.RemoveListener(ShowCollectionMenu);
        uIInput.OnShowPageCollection -= UIInput_OnShowPageCollection;
    }
    private void ShowCollectionMenu()
    {
        if(!uIInput.IsReceivingInput)
            return;

        notificationImage.gameObject.SetActive(false);
        uIInput.ShowPage(collectionMenu);
    }

    private void UIInput_OnShowPageCollection() => notificationImage.gameObject.SetActive(false);
    private void CardManager_OnDiscoverNewCard() => notificationImage.gameObject.SetActive(true);
}
