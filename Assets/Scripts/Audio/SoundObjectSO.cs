using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundObject", order = 1)]
public class SoundObjectSO : ScriptableObject
{
    public AudioClip clip;
    public string clipName;
    [SerializeField, Range(0.1f, 1)] public float volume = 1;
    [SerializeField, Range(0.1f, 3)] public float pitch = 1;
    public bool loop;
}
