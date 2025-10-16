using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CardRecruter : Card
{
    [System.Serializable]
    public class RecruitmentData
    {
        [Tooltip("The recipe that triggers this recruitment")]
        public CraftingRecipe triggerRecipe;

        [Tooltip("Delay before spawning the card (in seconds)")]
        [Range(0f, 120f)]
        public float recruitmentDelay = 5f;

        [Tooltip("The card to spawn after the delay")]
        public CardID cardIDToSpawn;

        [Tooltip("Offset from this card's position where the new card spawns")]
        public Vector3 spawnOffset = new Vector3(1f, 0f, 0f);
    }

    [Header("Recruitment Settings")]
    [SerializeField] private List<RecruitmentData> recruitmentOptions = new List<RecruitmentData>();

    [Header("Visual Feedback")]
    [SerializeField] private GameObject recruitmentBarPrefab;
    [SerializeField, Range(0f, 2f)] private float recruitmentBarOffset = 0.5f;

    [Header("Debug")]
    [SerializeField] private Logger logger;

    private Dictionary<int, Timer> activeRecruitments = new Dictionary<int, Timer>();
    private Dictionary<int, GameObject> activeRecruitmentBars = new Dictionary<int, GameObject>();
    private int nextRecruitmentID = 0;

    public static event Action OnAnyRecruitmentComplete;

    public bool IsRecruiting => activeRecruitments.Count > 0;
    public int GetActiveRecruitmentCount => activeRecruitments.Count;

    protected override void Start()
    {
        base.Start();

        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnCraftCompleteWithInfo += OnCraftComplete;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnCraftCompleteWithInfo -= OnCraftComplete;
        }

        CancelAllRecruitments();
    }

    private void OnCraftComplete(CraftInfo craftInfo, CardID completedCardID)
    {
        if(completedCardID == CardID.CURRENCY)
        {
            ShopManager.Instance.AddPlayerCoin(1);
        }


        foreach (var recruitment in recruitmentOptions)
        {
            if (recruitment.triggerRecipe == null)
                continue;

            bool isMatchingRecipe = craftInfo.CraftingRecipe == recruitment.triggerRecipe;

            if (isMatchingRecipe)
            {
                StartRecruitment(recruitment);
                break;
            }
        }
    }

    private void StartRecruitment(RecruitmentData recruitment)
    {
        int recruitmentID = nextRecruitmentID++;

        logger.Log($"[RECRUTER] {cardData.CardID} starting recruitment for {recruitment.cardIDToSpawn} (delay: {recruitment.recruitmentDelay}s)", this);

        InitRecrutementCooldownBar(recruitment, recruitmentID);
        
        Timer recruitmentTimer = Timer.Register(
            recruitment.recruitmentDelay,
            onComplete: () => SpawnRecruitedCard(recruitmentID, recruitment)
        );

        activeRecruitments[recruitmentID] = recruitmentTimer;
    }

    private void InitRecrutementCooldownBar(RecruitmentData recruitment, int recruitmentID)
    {
        if (recruitmentBarPrefab != null)
        {
            GameObject recruitmentBar = Instantiate(recruitmentBarPrefab, transform.position, Quaternion.identity, transform);
            recruitmentBar.GetComponent<Canvas>().sortingOrder = 30;

            CooldownBarUI cooldownBarUI = recruitmentBar.GetComponentInChildren<CooldownBarUI>();
            if (cooldownBarUI != null)
            {
                cooldownBarUI.OnCraftDelayEnd -= OnRecruitmentComplete;
                cooldownBarUI.OnCraftDelayEnd += OnRecruitmentComplete;
                cooldownBarUI.Init(transform, recruitment.recruitmentDelay, recruitmentBarOffset, recruitmentID);
            }

            recruitmentBar.transform.position = transform.position + new Vector3(0, recruitmentBarOffset, 0);
            activeRecruitmentBars[recruitmentID] = recruitmentBar;
        }
    }

    private void OnRecruitmentComplete(int recruitmentID)
    {
        if (activeRecruitmentBars.ContainsKey(recruitmentID))
        {
            Destroy(activeRecruitmentBars[recruitmentID]);
            activeRecruitmentBars.Remove(recruitmentID);
        }
    }

    private void SpawnRecruitedCard(int recruitmentID, RecruitmentData recruitment)
    {
        if (CardManager.HasInstance && CardManager.Instance.IsMaxCardsReached)
        {
            logger.LogWarning($"[RECRUTER] Cannot spawn {recruitment.cardIDToSpawn} - max cards reached", this);
            CleanupRecruitment(recruitmentID);
            return;
        }

        Vector3 spawnPosition = transform.position + recruitment.spawnOffset;
        GameObject spawnedCard = Instantiate( CardManager.Instance.GetCardPrefabByCardID(recruitment.cardIDToSpawn).gameObject, spawnPosition, Quaternion.identity);

        OnAnyRecruitmentComplete?.Invoke();

        logger.Log($"[RECRUTER] {cardData.CardID} successfully recruited {recruitment.cardIDToSpawn} at {spawnPosition}", this);

        CleanupRecruitment(recruitmentID);
    }

    private void CleanupRecruitment(int recruitmentID)
    {
        if (activeRecruitments.ContainsKey(recruitmentID))
        {
            activeRecruitments.Remove(recruitmentID);
        }

        if (activeRecruitmentBars.ContainsKey(recruitmentID))
        {
            if (activeRecruitmentBars[recruitmentID] != null)
            {
                Destroy(activeRecruitmentBars[recruitmentID]);
            }
            activeRecruitmentBars.Remove(recruitmentID);
        }
    }

    private void CancelAllRecruitments()
    {
        foreach (var kvp in activeRecruitments)
        {
            if (kvp.Value != null)
            {
                kvp.Value.Cancel();
            }
        }
        activeRecruitments.Clear();

        foreach (var kvp in activeRecruitmentBars)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        activeRecruitmentBars.Clear();
    }

    public void CancelRecruitment(int recruitmentID)
    {
        if (activeRecruitments.ContainsKey(recruitmentID))
        {
            activeRecruitments[recruitmentID]?.Cancel();
            CleanupRecruitment(recruitmentID);
            Debug.Log($"[RECRUTER] Cancelled recruitment {recruitmentID}");
        }
    }

  


}