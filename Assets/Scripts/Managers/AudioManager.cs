using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource gameBackgroundTrack;
    public AudioSource menuInteraction;

    public void PlayBackgroundTrack()
    {
        gameBackgroundTrack.Play();
    }

    public void PlayMenuInteraction()
    {
        menuInteraction.Play();
    }
}
