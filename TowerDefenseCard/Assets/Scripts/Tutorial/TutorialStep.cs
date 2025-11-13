using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

[Serializable]
public class TutorialStep
{
    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] private GameObject focusGameObject;
    public GameObject FocusGameObject => focusGameObject;

    [SerializeField] private Sprite tanukiSprite;

    public void Show(TextMeshProUGUI descriptionText, Image tanukiImage)
    {
#if UNITY_WEBGL
        Description.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                descriptionText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
            descriptionText.text = Description.GetLocalizedString();
#endif

        tanukiImage.sprite = tanukiSprite;

        if (focusGameObject == null)
            return;
   
        focusGameObject.SetActive(true); 
    }
    public void Show(ScrollTextWithVoice scrollTextWithVoice, Image tanukiImage, TextMeshProUGUI textMeshProUGUI)
    {
#if UNITY_WEBGL
        Description.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                scrollTextWithVoice.TypeText(handle.Result, textMeshProUGUI);
            }
        };
#endif

#if !UNITY_WEBGL
         scrollTextWithVoice.TypeText(Description.GetLocalizedString(), textMeshProUGUI);
#endif

        tanukiImage.sprite = tanukiSprite;

        if (focusGameObject == null)
            return;

        focusGameObject.SetActive(true);
    }

    public void Hide()
    {
        if (focusGameObject == null)
            return;

        focusGameObject.SetActive(false);
    }
    public void Hide(ScrollTextWithVoice scrollTextWithVoice)
    {
        //scrollTextWithVoice.StopTypingText();

        if (focusGameObject == null)
            return;

        focusGameObject.SetActive(false);
    }
}
