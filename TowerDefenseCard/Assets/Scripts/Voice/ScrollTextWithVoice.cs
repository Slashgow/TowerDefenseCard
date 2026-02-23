using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollTextWithVoice : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<AudioClip> voiceDatas;

    [SerializeField, Range(0f, 1f)] private float typingSpeed = 0.05f;
    [SerializeField, Range(0f, 1f)] private float volumeVariation = 0.1f;
    [SerializeField, Range(0f, 1f)] private float pitchVariation = 0.1f;
    [SerializeField, Range(0, 10)] private int soundFrequency = 1;
    [SerializeField] private bool skipSoundForSpaces = true;

    private Dictionary<TextMeshProUGUI, Coroutine> activeCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    private Dictionary<TextMeshProUGUI, bool> typingStates = new Dictionary<TextMeshProUGUI, bool>();

    public event Action<TextMeshProUGUI> OnHideTextComplete;

    public void TypeText(string text, TextMeshProUGUI textMeshProUGUI)
    {
        if (textMeshProUGUI == null)
        {
            Debug.LogWarning("TypeText called with null TextMeshProUGUI", this);
            return;
        }


        StopTypingText(textMeshProUGUI);
        Coroutine coroutine = StartCoroutine(TypeTextCoroutine(text, textMeshProUGUI, 0f));
        activeCoroutines[textMeshProUGUI] = coroutine;
    }

    public void TypeText(string text, TextMeshProUGUI textMeshProUGUI, float keepShowingDuration)
    {
        if (textMeshProUGUI == null)
        {
            Debug.LogWarning("TypeText called with null TextMeshProUGUI", this);
            return;
        }

        StopTypingText(textMeshProUGUI);
        Coroutine coroutine = StartCoroutine(TypeTextCoroutine(text, textMeshProUGUI, keepShowingDuration));
        activeCoroutines[textMeshProUGUI] = coroutine;
    }

    public void StopTypingText(TextMeshProUGUI textMeshProUGUI)
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = string.Empty;
            textMeshProUGUI.enableAutoSizing = true;
        }

        if (activeCoroutines.ContainsKey(textMeshProUGUI) && activeCoroutines[textMeshProUGUI] != null)
        {
            StopCoroutine(activeCoroutines[textMeshProUGUI]);
            activeCoroutines.Remove(textMeshProUGUI);
            typingStates.Remove(textMeshProUGUI);
        }
    }

    public void StopAllTypingText()
    {
        foreach (var kvp in activeCoroutines)
        {
            if (kvp.Key != null)
            {
                kvp.Key.text = string.Empty;
                kvp.Key.enableAutoSizing = true;
            }
            if (kvp.Value != null)
            {
                StopCoroutine(kvp.Value);
            }
        }
        activeCoroutines.Clear();
        typingStates.Clear();
    }

    private IEnumerator TypeTextCoroutine(string text, TextMeshProUGUI textMeshProUGUI, float keepShowingDuration)
    {
        ApplyOptimalFontSize(text, textMeshProUGUI);

        typingStates[textMeshProUGUI] = true;
        textMeshProUGUI.text = string.Empty;

        int soundCounter = 0;

        for (int i = 0; i < text.Length; i++)
        {
            textMeshProUGUI.text += text[i];

            if (!(char.IsWhiteSpace(text[i]) && skipSoundForSpaces))
            {
                soundCounter++;

                if ((soundCounter - 1) % soundFrequency == 0)
                {

                    int soundIndex;
                    char currentChar = char.ToLower(text[i]);

                    if (currentChar >= 'a' && currentChar <= 'z')
                    {
                        soundIndex = currentChar - 'a';
                        soundIndex = soundIndex % voiceDatas.Count;
                    }
                    else
                    {
                        soundIndex = UnityEngine.Random.Range(0, voiceDatas.Count);
                    }

                    AudioClip sound = voiceDatas[soundIndex];
                    if (sound != null)
                    {
                        audioSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
                        float volume = 1f + UnityEngine.Random.Range(-volumeVariation, volumeVariation);
                        volume = Mathf.Clamp01(volume);
                        audioSource.PlayOneShot(sound, volume);
                        yield return new WaitForSecondsRealtime(0.01f);
                    }
                }
            }
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        typingStates[textMeshProUGUI] = false;
        yield return new WaitForSecondsRealtime(0.5f + keepShowingDuration);

        if (keepShowingDuration > 0f)
        {
            textMeshProUGUI.text = string.Empty;
            textMeshProUGUI.enableAutoSizing = true;
            OnHideTextComplete?.Invoke(textMeshProUGUI);
        }

        activeCoroutines.Remove(textMeshProUGUI);
        typingStates.Remove(textMeshProUGUI);
    }

    private void ApplyOptimalFontSize(string text, TextMeshProUGUI textMeshProUGUI)
    {
        textMeshProUGUI.text = text;

        if (textMeshProUGUI.enableAutoSizing)
        {
            textMeshProUGUI.ForceMeshUpdate();
            float calculatedSize = textMeshProUGUI.fontSize;
            textMeshProUGUI.enableAutoSizing = false;
            textMeshProUGUI.fontSize = calculatedSize;
        }
    }

    public bool IsTyping(TextMeshProUGUI textMeshProUGUI)
    {
        return typingStates.ContainsKey(textMeshProUGUI) && typingStates[textMeshProUGUI];
    }
}