using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UIPageCardsDiscovered : UIPage
{
    [SerializeField] private GameObject uiCardMenuPrefab;
    public GameObject UICardMenuPrefab => uiCardMenuPrefab;

    [SerializeField] private Transform contentScrollView;
    [SerializeField] private TextMeshProUGUI cardDiscoverCounterText;
    [SerializeField] private LocalizedString cardDiscoverLocalizedString;

    public event Action<Card> OnSelectCard;
    public override void Show()
    {
        base.Show();
        CardUtility.DestroyAllChildren(contentScrollView);

        List<Card> cardDiscovered = CardManager.Instance.GetAllCardsDiscovered();

        cardDiscoverCounterText.text = $"{cardDiscoverLocalizedString.GetLocalizedString()} : {cardDiscovered.Count}/{CardManager.Instance.AllDiscoverableCards}";

        foreach (Card card in cardDiscovered)
        {
            GameObject uiCardMenuGameObject = Instantiate(uiCardMenuPrefab, contentScrollView);
            UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
            uICardMenu.SetupUICardMenu(card.CardData, false);

            UICardOutlineSelector uICardOutline = uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutlineSelector>();

            if(card is Currency)
            {
                uICardOutline.GetComponent<Image>().color = new Color32(255, 213, 90, 255);
                RectTransform rectTransform = uiCardMenuGameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = sizeDelta * 0.5f;
                //uiCardMenuGameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            }


            uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCard;
            uICardMenu.OnSelectEvent += UICardMenu_OnSelectCard;
        }
    }

    private void UICardMenu_OnSelectCard(CardID cardID)
    {
        OnSelectCard?.Invoke(CardManager.Instance.GetCardPrefabByCardID(cardID));
    }

    public override void Hide()
    {
        base.Hide();

        
    }
}
