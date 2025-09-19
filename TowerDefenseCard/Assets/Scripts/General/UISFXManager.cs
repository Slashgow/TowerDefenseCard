using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISFXManager : MonoSingleton<UISFXManager>
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    protected override void Awake()
    {
        base.Awake();

        SelectableInputRegister.OnAnySelectableHover += SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed += SelectableInputRegister_OnAnySelectablePressed;
    }
    private void OnDestroy()
    {
        SelectableInputRegister.OnAnySelectableHover -= SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed -= SelectableInputRegister_OnAnySelectablePressed;
    }

    private void SelectableInputRegister_OnAnySelectablePressed() => PlayClip(clickSound);
    private void SelectableInputRegister_OnAnySelectableHover() => PlayClip(hoverSound);

    public void PlayClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
