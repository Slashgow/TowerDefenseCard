using System;
using DG.Tweening;
using UnityEngine;

public class MusicManager : MonoSingleton<MusicManager>
{
    [SerializeField] private Logger logger;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource secondaryAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip craftMusic, combatMusic;

    [Header("Crossfade Settings")]
    [SerializeField, Range(0f, 10f)] private float crossfadeDuration = 2f;

    [Header("Pause Settings")]
    [SerializeField, Range(0f,5f)] private float fadeDuration = 1f;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;
    [SerializeField, Range(-3f, 3f)] private float pausePitch = 0.3f;
    [SerializeField, Range(-3f, 3f)] private float resumePitch = 1f;

    private Tween fadeTween;
    private Tween volumeFadeOutTween;
    private Tween volumeFadeInTween;
    public AudioSource CurrentAudioSource { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CurrentAudioSource = audioSource;
        CurrentAudioSource.clip = craftMusic;
        CurrentAudioSource.Play();
    }

    private void Start()
    {
        GameManager.Instance.OnStartCraftMode += StartCraftMusic;
        GameManager.Instance.OnStartCombatMode += StartCombatMusic;
    }

    private void StartCombatMusic() => CrossfadeToClip(combatMusic, crossfadeDuration);
    private void StartCraftMusic() => CrossfadeToClip(craftMusic, crossfadeDuration);

    public void PitchDownMusic()
    {
        fadeTween?.Kill();
        fadeTween = CurrentAudioSource.DOPitch(pausePitch, fadeDuration).SetEase(fadeEase).SetUpdate(true);
        logger.Log("Music pitch down", this);
    }

    public void ResumePitchMusic()
    {
        fadeTween?.Kill();
        fadeTween = CurrentAudioSource.DOPitch(resumePitch, fadeDuration).SetEase(fadeEase).SetUpdate(true);
        logger.Log("Music pitch resume", this);
    }

    public void CrossfadeToClip(AudioClip newClip, float duration)
    {
        if (newClip == null)
        {
            logger.Log("Cannot crossfade to null clip", this);
            return;
        }

        volumeFadeOutTween?.Kill();
        volumeFadeInTween?.Kill();

        AudioSource fadeOutSource = CurrentAudioSource;
        AudioSource fadeInSource = (CurrentAudioSource == audioSource) ? secondaryAudioSource : audioSource;

        fadeInSource.clip = newClip;
        fadeInSource.volume = 0f;
        fadeInSource.pitch = CurrentAudioSource.pitch; 
        fadeInSource.Play();

        volumeFadeOutTween = fadeOutSource.DOFade(0f, duration)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() => fadeOutSource.Stop());

        volumeFadeInTween = fadeInSource.DOFade(1f, duration)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() => {
                CurrentAudioSource = fadeInSource;
                logger.Log($"Crossfaded to clip: {newClip.name} over {duration}s", this);
            });

        logger.Log($"Starting crossfade to clip: {newClip.name} over {duration}s", this);
    }
    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCraftMode -= StartCraftMusic;
            GameManager.Instance.OnStartCombatMode -= StartCombatMusic;
        }
    }

}
