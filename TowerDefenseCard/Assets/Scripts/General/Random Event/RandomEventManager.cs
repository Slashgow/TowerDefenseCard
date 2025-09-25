using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;

public class RandomEventManager : MonoSingleton<RandomEventManager>, ISavable, ILoadable
{
    [SerializeField] private Logger logger;

    [Header("Random Event Settings")]
    [SerializeField] private RandomEventSettings eventSettings;
    [SerializeField] private bool enableRandomEvents = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject warningEffectPrefab;
    [SerializeField, Range(0f, 5f)] private float warningDuration = 2f;

    [Header("Player Health")]
    [SerializeField] private Card playerHealth;

    private Timer spawnTimer;
    private bool isActive = false;
    private float nextSpawnTime;

    public static event Action<GameObject, Card> OnRandomEnemySpawned;
    public static event Action OnRandomEventTriggered;
    public static event Action<Vector3> OnRandomEventWarning;

    public bool IsActive => isActive && enableRandomEvents;
    public float NextSpawnTime => nextSpawnTime;

    protected override void Awake()
    {
        base.Awake();
        ValidateSettings();
    }

    private void Start()
    {
        OnStartCraftMode();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        StopSpawnTimer();
    }

    private void SubscribeToEvents()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCraftMode += OnStartCraftMode;
            GameManager.Instance.OnEndCraftMode += OnEndCraftMode;
            GameManager.Instance.OnStartCombatMode += OnStartCombatMode;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCraftMode -= OnStartCraftMode;
            GameManager.Instance.OnEndCraftMode -= OnEndCraftMode;
            GameManager.Instance.OnStartCombatMode -= OnStartCombatMode;
        }
    }

    private void ValidateSettings()
    {
        if (!eventSettings.IsValid)
        {
            logger.LogError("RandomEventManager: Invalid settings! Please configure enemy prefabs.", this);
            enableRandomEvents = false;
        }

        if (eventSettings.MinSpawnInterval >= eventSettings.MaxSpawnInterval)
        {
            logger.LogWarning("RandomEventManager: MinSpawnInterval should be less than MaxSpawnInterval!", this);
        }

        if (!eventSettings.CanTargetDefenseCards && !eventSettings.CanTargetWorkerCards &&
            !eventSettings.CanTargetCurrencyCards && !eventSettings.CanTargetShopCards && 
            !eventSettings.CanTargetRessourceCards && !eventSettings.CanTargetCardExploitation &&
            !eventSettings.CanTargetRessourceGeneratorCards)
        {
            logger.LogWarning("RandomEventManager: No target card types selected! Events will not spawn.", this);
            enableRandomEvents = false;
        }
    }

    #region Game Mode Events
    private void OnStartCraftMode()
    {
        if (ShouldActivateRandomEvents())
            ActivateRandomEvents();
    }

    private void OnEndCraftMode() => DeactivateRandomEvents();
    private void OnStartCombatMode() => DeactivateRandomEvents();

    #endregion

    #region Random Event Logic
    private bool ShouldActivateRandomEvents()
    {
        if (!enableRandomEvents || !eventSettings.IsValid)
        {
            logger.Log("Random events disabled or invalid settings", this);
            return false;
        }
        return true;
    }

    private bool HasDefenseCardsOnBoard() => CardManager.Instance.CurrentNumberOfDefenseCards > 0;

    private List<Card> GetValidTargetCards()
    {
        List<Card> validTargets = new List<Card>();
        List<Card> cardsOnBoard = CardManager.Instance.CardsOnBoard;

        foreach (Card card in cardsOnBoard)
        {
            if (IsValidTarget(card))
            {
                validTargets.Add(card);
            }
        }

        return validTargets;
    }

    private bool IsValidTarget(Card card)
    {
        if (card == null) 
            return false;

        if(card.TryGetComponent(out IStealable stealable))
            return !stealable.IsStolen;

        if (card is CardDefense && eventSettings.CanTargetDefenseCards) return true;
        if (card is CardWorker && eventSettings.CanTargetWorkerCards) return true;
        if (card is Currency && eventSettings.CanTargetCurrencyCards) return true;
        if (card is CardShop && eventSettings.CanTargetShopCards) return true;
        if (card is CardRessource && eventSettings.CanTargetRessourceCards) return true;
        if (card is CardExploitation && eventSettings.CanTargetCardExploitation) return true;
        if (card is CardRessourceGenerator && eventSettings.CanTargetRessourceGeneratorCards) return true;

        return false;
    }

    private void ActivateRandomEvents()
    {
        if (isActive)
            return;

        isActive = true;
        ScheduleNextSpawn();
        logger.Log("Random events activated", this);
    }

    private void DeactivateRandomEvents()
    {
        if (!isActive)
            return;

        isActive = false;
        StopSpawnTimer();
        logger.Log("Random events deactivated", this);
    }

    private void ScheduleNextSpawn()
    {
        if (!isActive || GameManager.Instance.CurrentGameMode != GameMode.CRAFTING)
            return;

        StopSpawnTimer();

        float spawnInterval = UnityEngine.Random.Range(eventSettings.MinSpawnInterval, eventSettings.MaxSpawnInterval);
        nextSpawnTime = Time.time + spawnInterval;

        spawnTimer = Timer.Register(spawnInterval, () =>
        {
            if(TryGetValidTarget(out Card card))
            {
                if (ShouldSpawnEnemy(card))
                    SpawnRandomEnemy(card);
            }

            ScheduleNextSpawn();
        });

        logger.Log($"Next spawn scheduled in {spawnInterval:F1} seconds", this);
    }

    public void MoveToNewTarget(SimpleStealer stealer)
    {
        if(TryGetValidTarget(out Card card))
        {
            if (ShouldSpawnEnemy(card))
            {
                logger.Log($"moving to new target {card.CardData.CardName}", this);
                stealer.JumpCardMovement.MoveTo(card.transform.position);
                return;
            }
        }
        logger.Log($"no target found, moving to player health {card.CardData.CardName}", this);
        stealer.JumpCardMovement.MoveTo(playerHealth.transform.position);
    }

    public bool TryGetValidTarget(out Card card)
    {
        List<Card> validTargets = GetValidTargetCards();

        if (validTargets.Count == 0)
        {
            logger.Log("No valid target cards available for spawning", this);
            card = null;
            return false;
        }
        card = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
        return true;
    }

    private bool ShouldSpawnEnemy(Card targetCard)
    {
        if (GameManager.Instance.CurrentGameMode != GameMode.CRAFTING)
            return false;

        if (WaveManager.Instance.CurrentWaveIndex < eventSettings.MinimumWaveToStart)
        {
            logger.Log($"Not enough waves completed. Current: {WaveManager.Instance.CurrentWaveIndex}, Required: {eventSettings.MinimumWaveToStart}", this);
            return false;
        }

        logger.Log($"Time until next wave: {CraftingManager.Instance.TimeCraftMode:F1}s", this);
        if (CraftingManager.Instance.TimeCraftMode <= eventSettings.MaxTimeRemainingBeforeWaveStartAllowed)
        {
            logger.Log($"Too close to next wave. Time remaining: {CraftingManager.Instance.TimeCraftMode:F1}s, Allowed: {eventSettings.MaxTimeRemainingBeforeWaveStartAllowed:F1}s", this);
            return false;
        }
           

        if (CardManager.Instance.TotalCostCardsOnBoard -  targetCard.CardData.Cost - eventSettings.EnemyPrefabs[0].CardData.Cost < ShopManager.Instance.MinimumShopCost)
        {
            logger.Log($"Not enough ressources", this);
            return false;
        } 
           

        if (!HasDefenseCardsOnBoard())
        {
            logger.Log("Spawn cancelled: No defense cards on board", this);
            return false;
        }

        if (GetValidTargetCards().Count == 0)
        {
            logger.Log("Spawn cancelled: No valid target cards on board", this);
            return false;
        }

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > eventSettings.SpawnProbability)
        {
            logger.Log($"Spawn cancelled by probability: {roll:F2} > {eventSettings.SpawnProbability:F2}", this);
            return false;
        }

        return true;
    }

    private void SpawnRandomEnemy(Card targetCard)
    {
        try
        {
            Ennemy enemyPrefab = eventSettings.EnemyPrefabs[UnityEngine.Random.Range(0, eventSettings.EnemyPrefabs.Length)];
  
            Vector3 spawnPosition = GetSpawnPositionAroundCard(targetCard);

            if (warningEffectPrefab != null && warningDuration > 0)
            {
                ShowWarningEffect(spawnPosition);
                Timer.Register(warningDuration, () => DoSpawnEnemy(enemyPrefab.gameObject, spawnPosition, targetCard));
            }
            else
            {
                DoSpawnEnemy(enemyPrefab.gameObject, spawnPosition, targetCard);
            }

            OnRandomEventTriggered?.Invoke();
            logger.Log($"Random enemy spawn triggered: {enemyPrefab.name} targeting {targetCard.CardData.CardID}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"RandomEventManager: Error spawning enemy - {e.Message}", this);
        }
    }

    private Vector3 GetSpawnPositionAroundCard(Card targetCard)
    {
        Vector3 cardPosition = targetCard.transform.position;
        float actualRadius = eventSettings.SpawnRadius + UnityEngine.Random.Range(-eventSettings.SpawnRadiusVariation, eventSettings.SpawnRadiusVariation);
        actualRadius = Mathf.Max(1f, actualRadius); 
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(angle) * actualRadius,
            Mathf.Sin(angle) * actualRadius,
            0f
        );

        Vector3 spawnPosition = cardPosition + spawnOffset;

        if (eventSettings.SpawnBoundsCenter != null)
        {
            float distanceFromCenter = Vector3.Distance(eventSettings.SpawnBoundsCenter.position, spawnPosition);
            if (distanceFromCenter > eventSettings.MaxSpawnDistance)
            {
                Vector3 directionFromCenter = (spawnPosition - eventSettings.SpawnBoundsCenter.position).normalized;
                spawnPosition = eventSettings.SpawnBoundsCenter.position + directionFromCenter * eventSettings.MaxSpawnDistance;
            }
        }
        spawnPosition.z = 0f;

        return spawnPosition;
    }

    private void DoSpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition, Card targetCard)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        JumpCardMovement jumpMovement = newEnemy.GetComponent<JumpCardMovement>();
        if (jumpMovement != null)
        {
            if (targetCard == null)
                targetCard = playerHealth;

            jumpMovement.MoveTo(targetCard.transform.position);
        }
            
        else
            logger.LogWarning($"RandomEventManager: Enemy {enemyPrefab.name} doesn't have JumpCardMovement component!", this);

        OnRandomEnemySpawned?.Invoke(newEnemy, targetCard);
        logger.Log($"Random enemy spawned: {enemyPrefab.name} at {spawnPosition} targeting {targetCard.CardData.CardID}", this);
    }

    private void ShowWarningEffect(Vector3 position)
    {
        if (warningEffectPrefab == null)
            return;

        GameObject warning = Instantiate(warningEffectPrefab, position, Quaternion.identity);
        OnRandomEventWarning?.Invoke(position);

        Timer.Register(warningDuration, () =>
        {
            if (warning != null)
                Destroy(warning);
        });
    }

    private void StopSpawnTimer()
    {
        if (spawnTimer != null)
        {
            spawnTimer.Cancel();
            spawnTimer = null;
        }
    }
    #endregion

    #region Public Interface
    public void ForceSpawnRandomEnemy()
    {
        if (!eventSettings.IsValid)
        {
            logger.LogError("Cannot force spawn: Invalid settings!", this);
            return;
        }
        if (TryGetValidTarget(out Card card))
        {
            SpawnRandomEnemy(card);
        }
    }

    public void ForceSpawnEnemyAtCard(Card targetCard)
    {
        if (!eventSettings.IsValid || targetCard == null)
        {
            logger.LogError("Cannot force spawn: Invalid settings or target card!", this);
            return;
        }

        Ennemy enemyPrefab = eventSettings.EnemyPrefabs[UnityEngine.Random.Range(0, eventSettings.EnemyPrefabs.Length)];
        Vector3 spawnPosition = GetSpawnPositionAroundCard(targetCard);
        DoSpawnEnemy(enemyPrefab.gameObject, spawnPosition, targetCard);
    }

    public void SetRandomEventsEnabled(bool enabled)
    {
        enableRandomEvents = enabled;
        if (!enabled && isActive)
        {
            DeactivateRandomEvents();
        }
    }

    public float GetTimeUntilNextSpawn() => Mathf.Max(0, nextSpawnTime - Time.time);
    public List<Card> GetCurrentValidTargets() => GetValidTargetCards();
    #endregion

    #region Save/Load System
    public void Save(GameSaveData gameSaveData)
    {
        RandomEventManagerSaveData saveData = new RandomEventManagerSaveData
        {
            isActive = this.isActive,
            nextSpawnTime = this.nextSpawnTime,
            currentWaveThreshold = WaveManager.Instance.CurrentWaveIndex
        };

        // Store in GameSaveData - you might need to add this field to your GameSaveData class
        // gameSaveData.randomEventManagerData = saveData;
    }

    public void Load(GameSaveData gameSaveData)
    {
        // Load from GameSaveData - you might need to add this field to your GameSaveData class
        // RandomEventManagerSaveData saveData = gameSaveData.randomEventManagerData;
        // if (saveData != null)
        // {
        //     this.isActive = saveData.isActive;
        //     this.nextSpawnTime = saveData.nextSpawnTime;
        // }
    }
    #endregion

    #region Debug

    [ContextMenu("Force Spawn Random Enemy")]
    private void DebugForceSpawn()
    {
        ForceSpawnRandomEnemy();
    }

    [ContextMenu("Show Valid Targets")]
    private void DebugShowValidTargets()
    {
        List<Card> targets = GetValidTargetCards();
        Debug.Log($"Valid targets ({targets.Count}): {string.Join(", ", targets.Select(c => c.CardData.CardID.ToString()))}", this);
    }

    [ContextMenu("Toggle Random Events")]
    private void DebugToggleRandomEvents()
    {
        SetRandomEventsEnabled(!enableRandomEvents);
        Debug.Log($"Random events: {(enableRandomEvents ? "Enabled" : "Disabled")}", this);
    }
    #endregion

    //#region Gizmos
    //private void OnDrawGizmosSelected()
    //{
    //    if (!eventSettings.IsValid)
    //        return;
    //
    //    if (eventSettings.SpawnBoundsCenter != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawWireSphere(eventSettings.SpawnBoundsCenter.position, eventSettings.MaxSpawnDistance);
    //    }
    //
    //    List<Card> validTargets = GetValidTargetCards();
    //    foreach (Card card in validTargets)
    //    {
    //        if (card != null)
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawWireSphere(card.transform.position, eventSettings.SpawnRadius);
    //
    //            // Draw variation range
    //            Gizmos.color = Color.yellow;
    //            Gizmos.DrawWireSphere(card.transform.position, eventSettings.SpawnRadius + eventSettings.SpawnRadiusVariation);
    //            Gizmos.DrawWireSphere(card.transform.position, Mathf.Max(1f, eventSettings.SpawnRadius - eventSettings.SpawnRadiusVariation));
    //        }
    //    }
    //}
    //#endregion
}