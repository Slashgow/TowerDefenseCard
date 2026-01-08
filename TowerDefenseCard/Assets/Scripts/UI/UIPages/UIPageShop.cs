using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPageShop : UIPage
{
    [Header("References")]
    [SerializeField] private GameObject uiCardMenuPrefab;
    [SerializeField] private CardShop cardShop;

    [Header("Cards Section")]
    [SerializeField] private Transform cardsContentScrollView;

    [Header("Card Ideas Section")]
    [SerializeField] private Transform cardIdeasContentScrollView;

    [Header("Visual Settings")]
    [SerializeField] private Color cardNamesColor = new Color32(255, 213, 90, 255);

    public event Action<CardID> OnSelectCard;
    public event Action<CardID> OnSelectCardIdea;

    public override void Show()
    {
        base.Show();

        CardUtility.DestroyAllChildren(cardsContentScrollView);
        CardUtility.DestroyAllChildren(cardIdeasContentScrollView);

        PopulateCardsScrollView();
        PopulateCardIdeasScrollView();
    }

    private void PopulateCardsScrollView()
    {
        if (cardShop?.Shop?.ShopItems == null) 
            return;

        foreach (ShopItem shopItem in cardShop.Shop.ShopItems)
        {
            Card card = shopItem.CardPrefab.GetComponent<Card>();
            if (card == null) 
                continue;

            GameObject uiCardMenuGameObject = Instantiate(uiCardMenuPrefab, cardsContentScrollView);
            UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
            uICardMenu.SetupUICardMenu(card.CardData, false);

            ApplyCardStyling(uiCardMenuGameObject, card);

            uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCard;
            uICardMenu.OnSelectEvent += UICardMenu_OnSelectCard;
        }
    }

    private void PopulateCardIdeasScrollView()
    {
        if (cardShop?.Shop?.ShopCardIdeas == null) 
            return;

        foreach (ShopCardIdea shopCardIdea in cardShop.Shop.ShopCardIdeas)
        {
            if (shopCardIdea.CardIdeaPrefab?.CardData == null) 
                continue;

            GameObject uiCardMenuGameObject = Instantiate(uiCardMenuPrefab, cardIdeasContentScrollView);
            UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
            uICardMenu.SetupUICardMenu(shopCardIdea.CardIdeaPrefab.CardData, false);

            ApplyCardIdeaStyling(uiCardMenuGameObject);

            uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCardIdea;
            uICardMenu.OnSelectEvent += UICardMenu_OnSelectCardIdea;
        }
    }

    private void ApplyCardStyling(GameObject uiCardMenuGameObject, Card card)
    {
        UICardOutlineSelector uICardOutline = uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutlineSelector>();

        if (card is Currency)
        {
            if (uICardOutline != null)
            {
                uICardOutline.GetComponent<Image>().color = cardNamesColor;
            }

            RectTransform rectTransform = uiCardMenuGameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = sizeDelta * 0.5f;
            }
        }
    }

    private void ApplyCardIdeaStyling(GameObject uiCardMenuGameObject)
    {
        // Apply any specific styling for card ideas here
        // For example, different border color or effects
        UICardOutlineSelector uICardOutline = uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutlineSelector>();

        if (uICardOutline != null)
        {
            // Optional: Apply different styling for card ideas
            // uICardOutline.GetComponent<Image>().color = someColor;
        }
    }

    private void UICardMenu_OnSelectCard(CardID cardID)
    {
        OnSelectCard?.Invoke(cardID);
    }

    private void UICardMenu_OnSelectCardIdea(CardID cardID)
    {
        OnSelectCardIdea?.Invoke(cardID);
    }

    public override void Hide()
    {
        base.Hide();

        if (cardsContentScrollView != null)
        {
            foreach (Transform child in cardsContentScrollView)
            {
                UICardMenu uICardMenu = child.GetComponent<UICardMenu>();
                if (uICardMenu != null)
                {
                    uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCard;
                }
            }
        }

        if (cardIdeasContentScrollView != null)
        {
            foreach (Transform child in cardIdeasContentScrollView)
            {
                UICardMenu uICardMenu = child.GetComponent<UICardMenu>();
                if (uICardMenu != null)
                {
                    uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCardIdea;
                }
            }
        }
    }

    private void OnDestroy()
    {
        Hide();
    }
}