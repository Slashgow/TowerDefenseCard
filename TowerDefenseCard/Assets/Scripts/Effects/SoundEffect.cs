using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : Effect
{
    [SerializeField] private List<AudioClip> audioClips;

    public override void DoEffect()
    {
        SFXManager.Instance.PlayRandomAudioClip(audioClips);
    }
}
