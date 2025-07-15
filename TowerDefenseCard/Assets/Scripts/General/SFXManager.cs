using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoSingleton<SFXManager>
{
    [Header("References")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] private Reseller reseller;

    [Header("AudioClips")]
    [SerializeField] private AudioClip onPurchaseBoosterClip, onOpenBoosterClip, onCraftCompletedClip, onResellClip;

    private int currentAudioSourceIndex;

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        reseller.OnResell += Reseller_OnResell;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
        ShopManager.OnPurchaseBooster += ShopManager_OnPurchaseBooster;
    }

    private void OnDisable()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;
        
        reseller.OnResell -= Reseller_OnResell;
        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        ShopManager.OnPurchaseBooster -= ShopManager_OnPurchaseBooster;
    }

    private void ShopManager_OnPurchaseBooster() => PlayAudioClip(onPurchaseBoosterClip);
    private void Booster_OnOpenBooster() => PlayAudioClip(onOpenBoosterClip);
    private void Reseller_OnResell() => PlayAudioClip(onResellClip);
    private void CraftingManager_OnCraftComplete(int arg1, CardID arg2) => PlayAudioClip(onCraftCompletedClip);

    private void PlayAudioClip(AudioClip audioClip)
    {
        audioSources[currentAudioSourceIndex].clip = audioClip;
        audioSources[currentAudioSourceIndex].Play();
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Count;
    }
}
