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
    [SerializeField, Range(0f,1f)] private float volumeVariation = 0.1f;
    [SerializeField, Range(0f,1f)] private float pitchVariation = 0.1f;
    [SerializeField, Range(0, 10)] private int soundFrequency = 1;
    [SerializeField] private bool skipSoundForSpaces = true;
    
    private bool isTyping;
    private Coroutine coroutine;
    private TextMeshProUGUI textMeshProUGUI;

    public event Action OnHideTextComplete;

    public void TypeText(string text, TextMeshProUGUI textMeshProUGUI)
    {
        this.textMeshProUGUI = textMeshProUGUI;
        StopTypingText();
        coroutine = StartCoroutine(TypeTextCoroutine(text,0f));
    }

    public void TypeText(string text, TextMeshProUGUI textMeshProUGUI, float keepShowingDuration)
    {
        this.textMeshProUGUI = textMeshProUGUI;
        StopTypingText();
        coroutine = StartCoroutine(TypeTextCoroutine(text, keepShowingDuration));
    }

    public void StopTypingText()
    {
        if(textMeshProUGUI != null)
        {
            textMeshProUGUI.text = string.Empty;
            textMeshProUGUI.enableAutoSizing = true;
        }
           

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            isTyping = false;
        }
    }

    private IEnumerator TypeTextCoroutine(string text, float keepShowingDuration)
    {
        ApplyOptimalFontSize(text);

        isTyping = true;
        textMeshProUGUI.text = string.Empty;

        int soundCounter = 0;

        for (int i = 0; i < text.Length; i++)
        {
            textMeshProUGUI.text += text[i];

            if (!(char.IsWhiteSpace(text[i]) && skipSoundForSpaces))
            {
                soundCounter++;

                if((soundCounter - 1) % soundFrequency == 0)
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
        isTyping = false;
        yield return new WaitForSecondsRealtime(0.5f + keepShowingDuration);

        if(keepShowingDuration > 0f)
        {
            textMeshProUGUI.text = string.Empty;
            textMeshProUGUI.enableAutoSizing = true;
            OnHideTextComplete?.Invoke();
        }
           
    }

    private void ApplyOptimalFontSize(string text)
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
}