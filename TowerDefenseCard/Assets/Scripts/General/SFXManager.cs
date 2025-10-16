using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoSingleton<SFXManager>
{
    [Header("References")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    [SerializeField] private List<AudioClip> popSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> coinSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> grabSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> releaseSounds = new List<AudioClip>();

    [Header("AudioClips")]
    [SerializeField] private AudioClip onCompleteQuest;
    [SerializeField] private AudioClip onUnlockShop;
    [SerializeField] private AudioClip onApplyUpgrade, onRemoveUpgrade;

    private int currentAudioSourceIndex;

    private int currentGrabSoundIndex = 0;

    private void Start()
    {
        Shop.OnAnyShopUnlock += Shop_OnAnyShopUnlock;
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        Reseller.OnResell += Reseller_OnResell;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
        Booster.OnOpenCardIdea += Booster_OnOpenCardIdea;
        ShopManager.OnPurchaseBooster += ShopManager_OnPurchaseBooster;
        ShopManager.OnPurchasePartiallyBoosterEvent += ShopManager_OnPurchaseBooster;
        QuestManager.OnAnyQuestCompleted += QuestManager_OnAnyQuestCompleted;
        CardMover.OnStartDragCard += CardMover_OnGrabCard;
        CardMover.OnEndDragCard += CardMover_OnReleaseCard;
        CardUpgrade.OnAppliedAnyUpgrade += CardUpgrade_OnAppliedAnyUpgrade;
        CardUpgrade.OnRemovedAnyUpgrade += CardUpgrade_OnRemovedAnyUpgrade;
        CardRecruter.OnAnyRecruitmentComplete += CardRecruter_OnAnyRecruitmentComplete;
    }

 

    private void OnDisable()
    {
        if (CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;

        Reseller.OnResell -= Reseller_OnResell;
        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        Booster.OnOpenCardIdea -= Booster_OnOpenCardIdea;
        ShopManager.OnPurchaseBooster -= ShopManager_OnPurchaseBooster;
        ShopManager.OnPurchasePartiallyBoosterEvent -= ShopManager_OnPurchaseBooster;
        QuestManager.OnAnyQuestCompleted -= QuestManager_OnAnyQuestCompleted;
        Shop.OnAnyShopUnlock -= Shop_OnAnyShopUnlock;
        CardMover.OnStartDragCard -= CardMover_OnGrabCard;
        CardMover.OnEndDragCard -= CardMover_OnReleaseCard;
        CardUpgrade.OnAppliedAnyUpgrade -= CardUpgrade_OnAppliedAnyUpgrade;
        CardUpgrade.OnRemovedAnyUpgrade -= CardUpgrade_OnRemovedAnyUpgrade;
        CardRecruter.OnAnyRecruitmentComplete -= CardRecruter_OnAnyRecruitmentComplete;
    }

    private void Booster_OnOpenCardIdea() => PlayRandomAudioClip(popSounds);
    private void ShopManager_OnPurchaseBooster() => PlayRandomAudioClip(coinSounds);
    private void Booster_OnOpenBooster(CardID cardID) => PlayRandomAudioClip(popSounds);
    private void Reseller_OnResell(int numberOfReselledCard) => PlayRandomAudioClip(coinSounds);
    private void CraftingManager_OnCraftComplete(int arg1, CardID arg2) => PlayRandomAudioClip(popSounds);
    private void QuestManager_OnAnyQuestCompleted() => PlayAudioClip(onCompleteQuest);
    private void Shop_OnAnyShopUnlock() => PlayAudioClip(onUnlockShop);
    private void CardMover_OnReleaseCard() => PlayMatchingReleaseAudioClip(releaseSounds);
    private void CardMover_OnGrabCard() => PlayRandomGrabAudioClip(grabSounds);
    private void CardUpgrade_OnRemovedAnyUpgrade() => PlayAudioClip(onRemoveUpgrade);
    private void CardUpgrade_OnAppliedAnyUpgrade() => PlayAudioClip(onApplyUpgrade);
    private void CardRecruter_OnAnyRecruitmentComplete() => PlayRandomAudioClip(popSounds);

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSources[currentAudioSourceIndex].clip = audioClip;
        audioSources[currentAudioSourceIndex].Play();
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Count;
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip randomClip = audioClips[randomIndex];
        PlayAudioClip(randomClip);
    }

    public void PlayRandomGrabAudioClip(List<AudioClip> audioClips)
    {
        currentGrabSoundIndex = Random.Range(0, audioClips.Count);
        AudioClip randomClip = audioClips[currentGrabSoundIndex];
        PlayAudioClip(randomClip);
    }

    public void PlayMatchingReleaseAudioClip(List<AudioClip> audioClips)
    {
        AudioClip randomClip = audioClips[currentGrabSoundIndex];
        PlayAudioClip(randomClip);
    }
}
