using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_DelayAudio : MonoBehaviour
{

    public AudioSource audioSource;
    public float delay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayDelayed());
    }

    IEnumerator PlayDelayed()
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null)
            audioSource.Play();
    }
}
