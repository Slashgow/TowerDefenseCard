using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class TanukiLineData
{
    [SerializeField] private LocalizedString line;
    public LocalizedString Line => line;

    [SerializeField] private Sprite tanukiSprite;
    public Sprite TanukiSprite => tanukiSprite;
}
