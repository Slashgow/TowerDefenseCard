using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoSingleton<SFXManager>
{
    [Header("References")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    [SerializeField] private List<AudioClip> popSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> coinSounds = new List<AudioClip>();

    [Header("AudioClips")]
    [SerializeField] private AudioClip onCompleteQuest;

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
        if (CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;

        Reseller.OnResell -= Reseller_OnResell;
        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        Booster.OnOpenCardIdea -= Booster_OnOpenCardIdea;
        ShopManager.OnPurchaseBooster -= ShopManager_OnPurchaseBooster;
        QuestManager.OnAnyQuestCompleted -= QuestManager_OnAnyQuestCompleted;
    }

    private void Booster_OnOpenCardIdea() => PlayRandomAudioClip(popSounds);
    private void ShopManager_OnPurchaseBooster() => PlayRandomAudioClip(coinSounds);
    private void Booster_OnOpenBooster(CardID cardID) => PlayRandomAudioClip(popSounds);
    private void Reseller_OnResell(int numberOfReselledCard) => PlayRandomAudioClip(coinSounds);
    private void CraftingManager_OnCraftComplete(int arg1, CardID arg2) => PlayRandomAudioClip(popSounds);
    private void QuestManager_OnAnyQuestCompleted() => PlayAudioClip(onCompleteQuest);


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
}
