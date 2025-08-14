using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoSingleton<SFXManager>
{
    [Header("References")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    [Header("AudioClips")]
    [SerializeField] private AudioClip onPurchaseBoosterClip, onOpenBoosterClip, onCraftCompletedClip, onResellClip, onHarvestCurrency, onOpenCardIdea, onCompleteQuest;

    private int currentAudioSourceIndex;

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        Reseller.OnResell += Reseller_OnResell;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
        Booster.OnOpenCardIdea += Booster_OnOpenCardIdea;
        ShopManager.OnPurchaseBooster += ShopManager_OnPurchaseBooster;
        QuestManager.OnAnyQuestCompleted += QuestManager_OnAnyQuestCompleted;
    }

  
    private void OnDisable()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;
        
        Reseller.OnResell -= Reseller_OnResell;
        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        Booster.OnOpenCardIdea -= Booster_OnOpenCardIdea;
        ShopManager.OnPurchaseBooster -= ShopManager_OnPurchaseBooster;
        QuestManager.OnAnyQuestCompleted -= QuestManager_OnAnyQuestCompleted;
    }

    private void Booster_OnOpenCardIdea() => PlayAudioClip(onOpenCardIdea);
    private void ShopManager_OnPurchaseBooster() => PlayAudioClip(onPurchaseBoosterClip);
    private void Booster_OnOpenBooster(CardID cardID) => PlayAudioClip(onOpenBoosterClip);
    private void Reseller_OnResell(int numberOfReselledCard) => PlayAudioClip(onResellClip);
    private void CraftingManager_OnCraftComplete(int arg1, CardID arg2) => PlayAudioClip(onCraftCompletedClip);
    private void QuestManager_OnAnyQuestCompleted() => PlayAudioClip(onCompleteQuest);


    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSources[currentAudioSourceIndex].clip = audioClip;
        audioSources[currentAudioSourceIndex].Play();
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Count;
    }
}
