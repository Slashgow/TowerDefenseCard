using UnityEngine;

public class SoundEffect : Effect
{
    [SerializeField] private AudioClip audioClip;

    public override void DoEffect()
    {
        SFXManager.Instance.PlayAudioClip(audioClip);
    }
}
