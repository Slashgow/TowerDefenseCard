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

    [SerializeField] private Sprite spriteLocked;
    public Sprite SpriteLocked => spriteLocked;

    [SerializeField] private Sprite spriteUnlocked;
    public Sprite SpriteUnlocked => spriteUnlocked;

    public bool isDone;
    public Action<SuccessData> OnComplete;

    public void Complete()
    {
        if(isDone) 
            return;

        isDone = true;
        OnComplete?.Invoke(this);
    }
}
