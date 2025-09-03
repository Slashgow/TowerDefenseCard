using UnityEngine;

public class Yami : Ennemy
{
    [SerializeField] private Sprite dieSprite;

    protected override void Damageable_OnDie()
    {
        base.Damageable_OnDie();

        cardSprite.sprite = dieSprite;
        this.GetComponent<AutoCardMovement>().StopMoving();
        this.GetComponent<BaseDamageor>().StopAttack();
        this.GetComponent<Cloner>().StopCloning();
    }
}
