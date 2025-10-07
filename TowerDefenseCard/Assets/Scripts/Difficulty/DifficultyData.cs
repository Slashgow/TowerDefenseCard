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
    public GameDifficulty Difficulty => difficulty;
    public float TimeBetweenWavesPercentModifier => timeBetweenWavesPercentModifier;
    public float EnnemyHealthPercentModifier => ennemyHealthPercentModifier;
    public float EnnemyDamagePercentModifier => ennemyDamagePercentModifier;
    public float EnnemyAttackSpeedPercentModifier => ennemyAttackSpeedPercentModifier;
}
