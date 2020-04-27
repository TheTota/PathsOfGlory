using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicShuffler : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayMusics());
    }

    /// <summary>
    /// Plays musics all the time, starting by a random one of the availables.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayMusics()
    {
        int i = Random.Range(0, clips.Length);
        
        while(true)
        {
            audioSource.clip = clips[i];
            audioSource.Play();

            // wait until music is done
            yield return new WaitUntil(() => !audioSource.isPlaying);
            Debug.Log("Next music");
            i++;
        }
    }
}
