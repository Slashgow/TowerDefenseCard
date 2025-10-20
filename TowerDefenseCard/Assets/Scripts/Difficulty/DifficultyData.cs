using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "William/Difficulty")]
[Serializable]
public class DifficultyData : ScriptableObject
{
    [SerializeField] private GameDifficulty difficulty;
    [SerializeField, Range(-100f,100f)] private float timeBetweenWavesPercentModifier;
    [SerializeField, Range(-100f, 100f)] private float ennemyHealthPercentModifier;
    [SerializeField, Range(-100f, 100f)] private float ennemyDamagePercentModifier;
    [SerializeField, Range(-100f, 100f)] private float ennemyAttackSpeedPercentModifier;
    [SerializeField, Range(0, 60)] private int startMaxCardsAllowed;
    [SerializeField, Range(0, 10)] private int startMaxDefenseCardsAllowed;
    public GameDifficulty Difficulty => difficulty;
    public float TimeBetweenWavesPercentModifier => timeBetweenWavesPercentModifier;
    public float EnnemyHealthPercentModifier => ennemyHealthPercentModifier;
    public float EnnemyDamagePercentModifier => ennemyDamagePercentModifier;
    public float EnnemyAttackSpeedPercentModifier => ennemyAttackSpeedPercentModifier;
    public int StartMaxCardsAllowed => startMaxCardsAllowed;
    public int StartMaxDefenseCardsAllowed => startMaxDefenseCardsAllowed;
}
