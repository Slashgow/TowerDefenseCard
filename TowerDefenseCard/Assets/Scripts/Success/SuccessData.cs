using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class SuccessData
{
    [SerializeField] private LocalizedString title;
    public LocalizedString Title => title;

    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    public bool isDone;
    public Action OnComplete;

    public void Complete()
    {
        if(isDone) 
            return;

        isDone = true;
        OnComplete?.Invoke();
    }
}
