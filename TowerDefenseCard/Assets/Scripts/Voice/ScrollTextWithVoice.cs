using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollTextWithVoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<AudioClip> voiceDatas;

    [SerializeField, Range(0f, 1f)] private float typingSpeed = 0.05f;
    [SerializeField, Range(0f,1f)] private float volumeVariation = 0.1f;
    [SerializeField, Range(0f,1f)] private float pitchVariation = 0.1f;
    [SerializeField, Range(0, 10)] private int soundFrequency = 1;
    [SerializeField] private bool skipSoundForSpaces = true;
    
    private bool isTyping;
    private Coroutine coroutine;

    public void TypeText(string text)
    {
        StopTypingText();
        coroutine = StartCoroutine(TypeTextCoroutine(text));
    }

    public void StopTypingText()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            isTyping = false;
        }
    }

    private IEnumerator TypeTextCoroutine(string text)
    {
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
        yield return new WaitForSecondsRealtime(0.5f);
    }
}