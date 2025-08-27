using UnityEngine;

public class SuccessManager : MonoSingleton<SuccessManager>
{
    [SerializeField] private SuccessData firstCraftSuccess;
    public SuccessData FirstCraftSuccess => firstCraftSuccess;

    [SerializeField] private SuccessData firstFactorySuccess;
    public SuccessData FirstFactorySuccess => firstFactorySuccess;

    [SerializeField] private SuccessData craftTempleSuccess;
    public SuccessData CraftTempleSuccess => craftTempleSuccess;

    [SerializeField] private SuccessData craftFirstDefenseSuccess;
    public SuccessData CraftFirstDefenseSuccess => craftFirstDefenseSuccess;

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftCompleted;
    }

    private void OnDestroy()
    {
        if (CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftCompleted;
    }

    private void CraftingManager_OnCraftCompleted(int craftID, CardID outputCardID)
    {
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
                craftFirstDefenseSuccess.Complete();
                break;
            case CardID.TORII_GATE:
                break;
            case CardID.TENGU:
                break;
            case CardID.ONI:
                break;
            case CardID.BAMBOO_FACTORY:
                firstFactorySuccess.Complete();
                break;
            case CardID.SAKURA_FACTORY:
                firstFactorySuccess.Complete();
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
                firstFactorySuccess.Complete();
                break;
            case CardID.SPIRIT_FACTORY:
                firstFactorySuccess.Complete();
                break;
            case CardID.HACHIMAN:
                break;
            case CardID.AKITA_INU:
                craftFirstDefenseSuccess.Complete();
                break;
            case CardID.KOMAINU:
                break;
            case CardID.RED_CROWN_CRATE:
                craftFirstDefenseSuccess.Complete();
                break;
            case CardID.PHOENIX:
                break;
            case CardID.WHITE_SNAKE:
                craftFirstDefenseSuccess.Complete();
                break;
            case CardID.DRAGON:
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
                break;
            case CardID.TEMPLE:
                craftTempleSuccess.Complete();
                break;
            case CardID.YUREI:
                break;
            case CardID.CHEST:
                break;
            case CardID.FUJIN:
                break;
            case CardID.RAIJIN:
                break;
            case CardID.AMATERASU:
                break;
            case CardID.SUSANOO:
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
                break;
            case CardID.AMETHYSTE_FACTORY:
                break;
            case CardID.BAMBOO_PLANK_FACTORY:
                break;
            case CardID.SAKURA_BRICK_FACTORY:
                break;
        }
    }
}
