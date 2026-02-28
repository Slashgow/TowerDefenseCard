using System;
using System.Collections.Generic;
using UnityEngine;

public class SuccessManager : MonoSingleton<SuccessManager>
{
    [Header("References")]
    [SerializeField] private QuestManager mainQuestManager;
    [SerializeField] private QuestManager sideQuestManager;

    [SerializeField] private bool listenToSuccessCompletion = true;

    [Header("Newbie")]
    [SerializeField] private SuccessData openFirstBoosterSuccess;

    [Header("Booster Addict")]
    [SerializeField] private SuccessData open50BoosterSuccess;
    [SerializeField] private SuccessData open250BoosterSuccess;
    [SerializeField] private SuccessData open500BoosterSuccess;
    //[SerializeField] private SuccessData open1000BoosterSuccess;

    [Header("Reroller")]
    [SerializeField] private SuccessData open100BoosterDuringAGame;

    [Header("Storage")]
    [SerializeField] private SuccessData cardStorage30Success;
    [SerializeField] private SuccessData cardStorage50Success;
    [SerializeField] private SuccessData cardStorage100Success;

    [Header("Survivor")]
    [SerializeField] private SuccessData wave1Success;
    [SerializeField] private SuccessData wave3Success;
    [SerializeField] private SuccessData wave6Success;

    [Header("General")]
    [SerializeField] private SuccessData waveBossSuccess;

    [Header("Crafter")]
    [SerializeField] private SuccessData firstCraftSuccess;
    [SerializeField] private SuccessData craft250Cards;
    [SerializeField] private SuccessData craft1000Cards;
    //[SerializeField] private SuccessData craft10000Cards;

    [Header("Seller")]
    [SerializeField] private SuccessData sell100CardsSuccess;
    [SerializeField] private SuccessData sell500CardsSuccess;
    //[SerializeField] private SuccessData sell2500CardsSuccess;
    //[SerializeField] private SuccessData sell5000CardsSuccess;

    [Header("Industrialist")]
    [SerializeField] private SuccessData firstFactorySuccess;
    [SerializeField] private SuccessData craft4DifferentFactoriesSuccess;
    [SerializeField] private SuccessData craft8DifferentFactoriesSuccess;

    [Header("Kami Seeker")]
    [SerializeField] private SuccessData craftTempleSuccess;

    [Header("Defender")]
    [SerializeField] private SuccessData craftFirstDefenseSuccess;
    [SerializeField] private SuccessData craft4DifferentDefenseSuccess;
    [SerializeField] private SuccessData craft8DifferentDefenseSuccess;

    [Header("Thrifty")]
    [SerializeField] private SuccessData accumulate100InkAGameSuccess;

    [Header("Explorator")]
    [SerializeField] private SuccessData discoverAllCardsSuccess; // TO DO

    [Header("Ink Collecter")]
    [SerializeField] private SuccessData craftChestSuccess;

    [Header("Quests")]
    [SerializeField] private SuccessData completeMainQuestSuccess;
    [SerializeField] private SuccessData completeSideQuestSuccess;

    [Header("Pack")]
    [SerializeField] private SuccessData discoverAllPackSuccess;

    [Header("Recruiter")]
    [SerializeField] private SuccessData recruitAnotherWorkerSuccess;

    [Header("Upgrader")]
    [SerializeField] private SuccessData upgradeToMaxCardDefenseSuccess;

    [Header("Village")]
    [SerializeField] private SuccessData craftVillageSuccess;

    public SuccessData CraftTempleSuccess => craftTempleSuccess;
    public SuccessData FirstCraftSuccess => firstCraftSuccess;
    public SuccessData FirstFactorySuccess => firstFactorySuccess;
    public SuccessData CraftFirstDefenseSuccess => craftFirstDefenseSuccess;

    private SuccessStatData successStatData;
    public SuccessStatData SuccessStatData => successStatData; 
    private List<SuccessData> allSuccessData;
    public List<SuccessData > AllSuccessData => allSuccessData;

    protected override void Awake()
    {
        base.Awake();
        if(allSuccessData == null)
            InitializeSuccessList();
    }
    public void LoadDefault()
    {
        InitializeSuccessList();
        InitializeSuccessStatData();
    }

    public void LoadFromSuccessSaveData(SuccessSaveData successSaveData)
    {
        InitializeSuccessList();

        if (successSaveData.successCompletionStates != null &&
            successSaveData.successCompletionStates.Count == allSuccessData.Count)
        {
            for (int i = 0; i < allSuccessData.Count; i++)
            {
                allSuccessData[i].isDone = successSaveData.successCompletionStates[i];
            }
        }

        successStatData = new SuccessStatData(successSaveData.successStatData.boosterOpenedCounterAllTime,
                successSaveData.successStatData.boosterOpenedCounterInGame, successSaveData.successStatData.craftedCardCounterAllTime,
                successSaveData.successStatData.soldCardCounterAllTime,
                successSaveData.successStatData.factoriesIDThisGame,
                successSaveData.successStatData.defenseIDThisGame);
        
    }

    private void InitializeSuccessStatData() => successStatData = new SuccessStatData();

    private void InitializeSuccessList()
    {
        allSuccessData = new List<SuccessData>
        {
            // Newbie
            openFirstBoosterSuccess,
            
            // Booster Addict
            open50BoosterSuccess,
            open250BoosterSuccess,
            open500BoosterSuccess,
            //open1000BoosterSuccess,
            
            // Reroller
            open100BoosterDuringAGame,
            
            // Storage
            cardStorage30Success,
            cardStorage50Success,
            cardStorage100Success,
            
            // Survivor
            wave1Success,
            wave3Success,
            wave6Success,
            
            // General
            waveBossSuccess,
            
            // Crafter
            firstCraftSuccess,
            craft250Cards,
            craft1000Cards,
            //craft10000Cards,
            
            // Seller
            sell100CardsSuccess,
            sell500CardsSuccess,
            //sell2500CardsSuccess,
            //sell5000CardsSuccess,
            
            // Industrialist
            firstFactorySuccess,
            craft4DifferentFactoriesSuccess,
            craft8DifferentFactoriesSuccess,
            
            // Kami Seeker
            craftTempleSuccess,
            
            // Defender
            craftFirstDefenseSuccess,
            craft4DifferentDefenseSuccess,
            craft8DifferentDefenseSuccess,
            
            // Thrifty
            accumulate100InkAGameSuccess,
            
            // Explorator
            discoverAllCardsSuccess,
            
            // Ink Collector
            craftChestSuccess,

            // Quests
            completeMainQuestSuccess,
            completeSideQuestSuccess,

            // Pack
            discoverAllPackSuccess,

            // Recruiter
            recruitAnotherWorkerSuccess,

            // Upgrader
            upgradeToMaxCardDefenseSuccess,

            // Village
            craftVillageSuccess

        };
    }

    public List<SuccessData> GetCompletedSuccessData()
    {
        List<SuccessData> completedSuccess = new List<SuccessData>();

        foreach (SuccessData success in allSuccessData)
        {
            if (success.isDone)
                completedSuccess.Add(success);
        }

        return completedSuccess;
    }

    public int GetCompletedSuccessCount() => GetCompletedSuccessData().Count;

    public List<SuccessData> GetUncompletedSuccessData()
    {
        List<SuccessData> uncompletedSuccess = new List<SuccessData>();

        foreach (SuccessData success in allSuccessData)
        {
            if (!success.isDone)
                uncompletedSuccess.Add(success);
        }

        return uncompletedSuccess;
    }
    public float GetCompletionPercentage()
    {
        if (allSuccessData.Count == 0) return 0f;

        int completedCount = GetCompletedSuccessData().Count;
        return (float)completedCount / allSuccessData.Count * 100f;
    }

    private void Start()
    {
        if (!listenToSuccessCompletion)
            return;

        ShopManager.Instance.OnUpdatePlayerCoin += ShopManager_OnUpdatePlayerCoin;
        Reseller.OnResell += Reseller_OnResell;
        WaveManager.Instance.OnWaveEnd += WaveManager_OnWaveEnd;
        CardManager.Instance.OnUpdateMaxNumberOfCards += CardManager_OnUpdateMaxNumberOfCards;
        CardManager.OnDiscoverAllCards += CardManager_OnDiscoverAllCards;
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftCompleted;
        Booster.OnDestroyBooster += Booster_OnDestroyBooster;
        mainQuestManager.OnCompleteAllQuests += MainQuestManager_OnCompleteAllQuests;
        sideQuestManager.OnCompleteAllQuests += SideQuestManager_OnCompleteAllQuests;
        TutorialController.OnUnlockAllShops += TutorialController_OnUnlockAllShops;
        CardRecruter.OnAnyRecruitmentComplete += CardRecruter_OnAnyRecruitmentComplete;
        UIUpgrades.OnFillAllUpgradeSlots += UIUpgrades_OnFillAllUpgradeSlots;
    }


    private void OnDestroy()
    {
        if (!listenToSuccessCompletion)
            return;

        Booster.OnDestroyBooster -= Booster_OnDestroyBooster;
        Reseller.OnResell -= Reseller_OnResell;

        if (CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftCompleted;

        if (CardManager.HasInstance)
        {
            CardManager.Instance.OnUpdateMaxNumberOfCards -= CardManager_OnUpdateMaxNumberOfCards;
            CardManager.OnDiscoverAllCards -= CardManager_OnDiscoverAllCards;
        }
            
        if(WaveManager.HasInstance)
            WaveManager.Instance.OnWaveEnd -= WaveManager_OnWaveEnd;

        if(ShopManager.HasInstance)
            ShopManager.Instance.OnUpdatePlayerCoin -= ShopManager_OnUpdatePlayerCoin;

        mainQuestManager.OnCompleteAllQuests -= MainQuestManager_OnCompleteAllQuests;
        sideQuestManager.OnCompleteAllQuests -= SideQuestManager_OnCompleteAllQuests;

        if(TutorialController.HasInstance)
            TutorialController.OnUnlockAllShops -= TutorialController_OnUnlockAllShops;

        CardRecruter.OnAnyRecruitmentComplete -= CardRecruter_OnAnyRecruitmentComplete;
        UIUpgrades.OnFillAllUpgradeSlots -= UIUpgrades_OnFillAllUpgradeSlots;
    }


    private void CardManager_OnDiscoverAllCards() => discoverAllCardsSuccess.Complete();
    private void TutorialController_OnUnlockAllShops() => discoverAllPackSuccess.Complete();
    private void CardRecruter_OnAnyRecruitmentComplete() => recruitAnotherWorkerSuccess.Complete();
    private void UIUpgrades_OnFillAllUpgradeSlots() => upgradeToMaxCardDefenseSuccess.Complete();

    private void ShopManager_OnUpdatePlayerCoin(int currentPlayerCoin)
    {
        if (currentPlayerCoin >= 100)
            accumulate100InkAGameSuccess.Complete();
    }

    private void Reseller_OnResell(int cardCount)
    {
        successStatData.soldCardCounterAllTime += cardCount;

        //if (successStatData.soldCardCounterAllTime >= 5000)
        //    sell5000CardsSuccess.Complete();
        //if (successStatData.soldCardCounterAllTime >= 2500)
        //    sell2500CardsSuccess.Complete();
        if (successStatData.soldCardCounterAllTime >= 500)
            sell500CardsSuccess.Complete();
        if (successStatData.soldCardCounterAllTime >= 100)
            sell100CardsSuccess.Complete();
    }


    private void WaveManager_OnWaveEnd(int currentWaveIndex)
    {
        if (WaveManager.Instance.IsAllWavesCompleted)
            waveBossSuccess.Complete();
        if (currentWaveIndex >= 6)
            wave6Success.Complete();
        if (currentWaveIndex >= 3)
            wave3Success.Complete();
        if (currentWaveIndex >= 1)
            wave1Success.Complete();
    }

    private void CardManager_OnUpdateMaxNumberOfCards(int currentNumberOfCard, int maxCardsAllowed)
    {
        if (maxCardsAllowed >= 100)
            cardStorage100Success.Complete();
        if (maxCardsAllowed >= 50)
            cardStorage50Success.Complete();
        if (maxCardsAllowed >= 30)
            cardStorage30Success.Complete();
    }

    private void Booster_OnDestroyBooster()
    {
        successStatData.boosterOpenedCounterAllTime++;
        successStatData.boosterOpenedCounterInGame++;

        if(successStatData.boosterOpenedCounterInGame > 100)
            open100BoosterDuringAGame.Complete();

        //if(successStatData.boosterOpenedCounterAllTime >= 1000)
        //    open1000BoosterSuccess.Complete();
        if(successStatData.boosterOpenedCounterAllTime >= 500)
            open500BoosterSuccess.Complete();
        if(successStatData.boosterOpenedCounterAllTime >= 250)
            open250BoosterSuccess.Complete();
        if(successStatData.boosterOpenedCounterAllTime >= 50)
            open50BoosterSuccess.Complete();
        if (successStatData.boosterOpenedCounterAllTime >= 1)
            openFirstBoosterSuccess.Complete();
    }

    private void CraftingManager_OnCraftCompleted(int craftID, CardID outputCardID)
    {
        successStatData.craftedCardCounterAllTime++;

        //if(successStatData.craftedCardCounterAllTime >= 10000)
        //    craft10000Cards.Complete();
        if(successStatData.craftedCardCounterAllTime >= 1000)
            craft1000Cards.Complete();
        if (successStatData.craftedCardCounterAllTime >= 250)
            craft250Cards.Complete();
        if (successStatData.craftedCardCounterAllTime >= 1)
            firstCraftSuccess.Complete();


        switch (outputCardID)
        {
            case CardID.BAMBOO:
                break;
            case CardID.JADE:
                break;
            case CardID.SAKURA:
                break;
            case CardID.SPIRIT_ESSENCE:
                break;
            case CardID.BAMBOO_PLANK:
                break;
            case CardID.SAKURA_BRICK:
                break;
            case CardID.ARCHER:
                CheckSuccessDefense(CardID.ARCHER);
                break;
            case CardID.TORII_GATE:
                break;
            case CardID.TENGU:
                break;
            case CardID.ONI:
                break;
            case CardID.BAMBOO_FACTORY:
                CheckSuccessFactories(CardID.BAMBOO_FACTORY);
                break;
            case CardID.SAKURA_FACTORY:
                CheckSuccessFactories(CardID.SAKURA_FACTORY);
                break;
            case CardID.SHOP:
                break;
            case CardID.PLAYER_HEALTH:
                break;
            case CardID.MATCHA:
                break;
            case CardID.AMETHYSTE:
                break;
            case CardID.KAMI_ESSENCE:
                break;
            case CardID.JADE_FACTORY:
                CheckSuccessFactories(CardID.JADE_FACTORY);
                break;
            case CardID.SPIRIT_FACTORY:
                CheckSuccessFactories(CardID.SPIRIT_FACTORY);
                break;
            case CardID.HACHIMAN:
                CheckSuccessDefense(CardID.HACHIMAN);
                break;
            case CardID.AKITA_INU:
                CheckSuccessDefense(CardID.AKITA_INU);
                break;
            case CardID.KOMAINU:
                CheckSuccessDefense(CardID.KOMAINU);
                break;
            case CardID.RED_CROWN_CRATE:
                CheckSuccessDefense(CardID.RED_CROWN_CRATE);
                break;
            case CardID.PHOENIX:
                CheckSuccessDefense(CardID.PHOENIX);
                break;
            case CardID.WHITE_SNAKE:
                CheckSuccessDefense(CardID.WHITE_SNAKE);
                break;
            case CardID.DRAGON:
                CheckSuccessDefense(CardID.DRAGON);
                break;
            case CardID.RICE:
                break;
            case CardID.NOODLES:
                break;
            case CardID.EGG:
                break;
            case CardID.WATER:
                break;
            case CardID.WASABI:
                break;
            case CardID.RAMEN:
                break;
            case CardID.SPCIY_RAMEN:
                break;
            case CardID.SAKE:
                break;
            case CardID.TAMAGO_GOHAN:
                break;
            case CardID.KAPPA:
                break;
            case CardID.LEAVES:
                break;
            case CardID.BOOSTER:
                break;
            case CardID.BARN:
                break;
            case CardID.WAREHOUSE:
                break;
            case CardID.IDEA:
                break;
            case CardID.CURRENCY:
                break;
            case CardID.WORKER:
                break;
            case CardID.FOREST:
                break;
            case CardID.RICE_PADDY:
                break;
            case CardID.FARM:
                break;
            case CardID.MONTAIN:
                break;
            case CardID.OKUNINUSHI:
                CheckSuccessDefense(CardID.OKUNINUSHI);
                break;
            case CardID.TEMPLE:
                craftTempleSuccess.Complete();
                break;
            case CardID.YUREI:
                break;
            case CardID.CHEST:
                craftChestSuccess.Complete();
                break;
            case CardID.FUJIN:
                CheckSuccessDefense(CardID.FUJIN);
                break;
            case CardID.RAIJIN:
                CheckSuccessDefense(CardID.RAIJIN);
                break;
            case CardID.AMATERASU:
                CheckSuccessDefense(CardID.AMATERASU);
                break;
            case CardID.SUSANOO:
                CheckSuccessDefense(CardID.SUSANOO);
                break;
            case CardID.YAMATA_NO_OROCHI:
                break;
            case CardID.BAKENEKO:
                break;
            case CardID.NEKOMATA:
                break;
            case CardID.HOUSE:
                break;
            case CardID.STRAW:
                break;
            case CardID.KAMI_FACTORY:
                CheckSuccessFactories(CardID.KAMI_FACTORY);
                break;
            case CardID.AMETHYSTE_FACTORY:
                CheckSuccessFactories(CardID.AMETHYSTE_FACTORY);
                break;
            case CardID.BAMBOO_PLANK_FACTORY:
                CheckSuccessFactories(CardID.BAMBOO_PLANK_FACTORY);
                break;
            case CardID.SAKURA_BRICK_FACTORY:
                CheckSuccessFactories(CardID.SAKURA_BRICK_FACTORY);
                break;
            case CardID.VILLAGE:
                craftVillageSuccess.Complete();
                break;
        }
    }

    private void CheckSuccessFactories(CardID factoryID)
    {
        if (!successStatData.factoriesIDThisGame.Contains(factoryID))
            successStatData.factoriesIDThisGame.Add(factoryID);

        if (successStatData.factoriesIDThisGame.Count >= 8)
            craft8DifferentFactoriesSuccess.Complete();
        if (successStatData.factoriesIDThisGame.Count >= 4)
            craft4DifferentFactoriesSuccess.Complete();
        if (successStatData.factoriesIDThisGame.Count >= 1)
            firstFactorySuccess.Complete();
    }

    private void CheckSuccessDefense(CardID defenseID)
    {
        if (!successStatData.defenseIDThisGame.Contains(defenseID))
            successStatData.defenseIDThisGame.Add(defenseID);

        if (successStatData.defenseIDThisGame.Count >= 8)
            craft8DifferentDefenseSuccess.Complete();
        if (successStatData.defenseIDThisGame.Count >= 4)
            craft4DifferentDefenseSuccess.Complete();
        if (successStatData.defenseIDThisGame.Count >= 1)
            craftFirstDefenseSuccess.Complete();
    }

    private void SideQuestManager_OnCompleteAllQuests()
    {
        completeSideQuestSuccess.Complete();
    }

    private void MainQuestManager_OnCompleteAllQuests()
    {
        completeMainQuestSuccess.Complete();
    }
}
