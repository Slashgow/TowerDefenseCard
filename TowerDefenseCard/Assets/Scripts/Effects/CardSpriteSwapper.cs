using System.Collections;
using UnityEngine;


public class CardSpriteSwapper : Effect
{
    [SerializeField] private Card card;
    [SerializeField] private Sprite newSprite;

    public override void DoEffect()
    {
        card.CardSprite.sprite = newSprite;
    }
}
