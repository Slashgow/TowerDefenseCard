using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardHealerData")]
public class CardHealerData : ScriptableObject
{
    [SerializeField, Range(0f, 100f)] private float healAmount;
    public float HealAmount => healAmount;

    [SerializeField, Range(0f, 10f)] private float healRange;
    public float HealRange => healRange;

    [SerializeField, Range(0f, 10f)] private float healCooldown;
    public float HealCooldown => healCooldown;
}
