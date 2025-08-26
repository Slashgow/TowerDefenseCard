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
        descriptionText.text = Description.GetLocalizedString();
        tanukiImage.sprite = tanukiSprite;

        if (focusGameObject == null)
            return;
   
        focusGameObject.SetActive(true); 
    }
    public void Show(ScrollTextWithVoice scrollTextWithVoice, Image tanukiImage)
    {
        scrollTextWithVoice.TypeText(Description.GetLocalizedString());
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
        scrollTextWithVoice.StopTypingText();

        if (focusGameObject == null)
            return;

        focusGameObject.SetActive(false);
    }
}
