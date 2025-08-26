using System;
using UnityEngine;

[Serializable]
public class VoiceData
{
    [SerializeField] private string voiceName;
    public string VoiceName => voiceName;
    [SerializeField] private AudioClip audioClip;
    public AudioClip AudioClip => audioClip;
}
