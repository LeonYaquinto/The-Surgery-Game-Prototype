using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonsound : MonoBehaviour
{
    private AudioSource music;
    public AudioClip ClickAudio;
    public AudioClip SwitchAudio;
    void Start()
    {
        music = GetComponent<AudioSource>();    
    }

    public void ClickAudioOn()
    {
        music.PlayOneShot(ClickAudio);
    }

    public void SwitchAudioOn() {
        music.PlayOneShot(SwitchAudio);
    }
}
