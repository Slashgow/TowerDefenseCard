using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPageCardsDiscovered : UIPage
{
    [SerializeField] private GameObject uiCardMenuPrefab;
    public GameObject UICardMenuPrefab => uiCardMenuPrefab;

    [SerializeField] private Transform contentScrollView;

    public event Action<Card> OnSelectCard;
    public override void Show()
    {
        base.Show();

        CardUtility.DestroyAllChildren(contentScrollView);

        List<Card> cardDiscovered = CardManager.Instance.GetAllCardsDiscovered();

        foreach (Card card in cardDiscovered)
        {
            GameObject uiCardMenuGameObject = Instantiate(uiCardMenuPrefab, contentScrollView);
            UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
            uICardMenu.SetupUICardMenu(card.CardData);

            UICardOutline uICardOutline = uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutline>();
            uICardOutline.OnSelectCard -= UICardOutline_OnSelectCard;
            uICardOutline.OnSelectCard += UICardOutline_OnSelectCard;
        }
    }

    private void UICardOutline_OnSelectCard(CardID cardID)
    {
        OnSelectCard?.Invoke(CardManager.Instance.GetCardPrefabByCardID(cardID));
    }

    public override void Hide()
    {
        base.Hide();

        
    }
}
