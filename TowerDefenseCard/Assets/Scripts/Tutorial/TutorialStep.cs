using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class TutorialStep
{
    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] private GameObject focusGameObject;
    public GameObject FocusGameObject => focusGameObject;

    public void Show(TextMeshProUGUI descriptionText)
    {
        descriptionText.text = Description.GetLocalizedString();

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
}
