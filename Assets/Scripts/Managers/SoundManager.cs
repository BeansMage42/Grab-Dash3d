using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private AudioSource mainSource;
    [SerializeField] private SoundObjectSO bgm;
    [SerializeField] private SoundObjectSO winMusic;


    private void Awake()
    {
        mainSource = GetComponent<AudioSource>();
        mainSource.spatialBlend = 0;
        mainSource.playOnAwake = false;
    }
    public void GameStart()
    {
        ChangeMainAudio(bgm);
    }

    public void GameEnd()
    {
        ChangeMainAudio(winMusic);
    }

    private void ChangeMainAudio(SoundObjectSO sound)
    {
        mainSource.clip = sound.clip;
        mainSource.volume = sound.volume;
        mainSource.pitch = sound.pitch;
        mainSource.loop = sound.loop;
        mainSource.Play();
    }
}
