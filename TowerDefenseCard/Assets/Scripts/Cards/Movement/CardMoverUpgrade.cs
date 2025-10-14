using System.Collections;
using DG.Tweening;
using UnityEngine;


public class CardMoverUpgrade : CardMover
{
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    [SerializeField, Range(0f,1f)] private float timeToReachSlot = 0.2f;
    [SerializeField, Range(0f, 1f)] private float endScale = 0.2f;

    protected override void TryStackCards()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, detectionLayerMaskCards);
        if (!this.card.CardData.IsStackable)
            hits = null;

        if (hits != null)
        {
            foreach (var hit in hits)
            {
                if (hit.gameObject == this.gameObject)
                    continue;

                if (hit.transform.IsChildOf(this.transform))
                    continue;

                Card otherCard = hit.GetComponent<Card>();

                if (otherCard.StackedCards.Count > 0)
                {
                    // double check if stack cards contain null references and clean them
                    if (otherCard.TryClearStackCards())
                    {
                        if (otherCard.StackedCards.Count > 0)
                            continue;
                    }
                    else
                        continue;
                }

                if (otherCard != null && otherCard.CardData.IsStackable)
                {
                    

                    if(otherCard.TryGetComponent(out IUpgradable upgradable))
                    {
                        // TO DO : lerp position to first empty slot.

                        if(otherCard.GetComponentInChildren<UIUpgrades>())
                        {
                            UIUpgrades uiUpgrades = otherCard.GetComponentInChildren<UIUpgrades>();

                            if (uiUpgrades.HasEmptySlot())
                            {
                                card.OnStack(otherCard);
                                UIUpgradeSlot firstEmptySlot = uiUpgrades.GetFirstEmptySlot();
                                firstEmptySlot.FillSlot(this.card);

                                this.card.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                                this.card.transform.DOMove(firstEmptySlot.transform.position, timeToReachSlot).SetUpdate(true).SetEase(moveEase);
                                this.card.transform.DOScale(endScale, timeToReachSlot).SetUpdate(true).SetEase(moveEase)
                                    .OnComplete(() => this.card.gameObject.SetActive(false));
                                this.enabled = false;

                            }
                            else
                            {
                                Debug.LogWarning($"Card {otherCard.CardData.CardName} has no empty upgrade slots for upgrade {card.CardData.CardName}. Unstacking.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Card {otherCard.CardData.CardName} has IUpgradable but not UIUpgrades component.");
                        }

                    }
                    else
                    {
                        card.OnStack(otherCard);
                        Vector3 newPos = Vector3.zero;
                        newPos.y = -stackingHeight * (otherCard.StackedCards.Count);//.transform.childCount);
                        transform.localPosition = newPos;
                    }

                    CardUtility.AssignSortingOrderRecursively(card.transform, otherCard.CardSprite.sortingOrder + otherCard.transform.childCount);

                    if (CraftingManager.Instance.TryCraft(otherCard.transform.root, this.card))
                        return;

                    return;
                }
            }
        }


        Vector3 pos2D = transform.position;
        pos2D.z = 0.0f;
        transform.position = pos2D;
        CardUtility.AssignSortingOrderRecursively(card.transform, 3);
        this.card.OnUnstack();
        //transform.SetParent(startParent, false);
    }

}
