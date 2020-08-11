using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioSource audioSource;

    [SerializeField]
    float fadeTime = 0f;        // controls how fast the background music fades in/out after the player is hit
    float volume = 0f;          // the music volume
    float stoppedTime = 0f;     // marks the point in time when the audio clip stopped
    float loopTimer = 0f;       // counts time until next loop of background music
    [SerializeField]
    float loopTime = 22.5f;     // the amount of time one loop of the background music takes

    public float SFXVal = 0;
    public float BGMVal = 0;
    private void Awake() {
        volume = audioSource.volume;
        if (GameObject.FindGameObjectsWithTag("Music").Length > 1)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Update() {
        if (loopTimer < loopTime)
            loopTimer += Time.deltaTime;
        else {
            loopTimer = 0f;
            audioSource.Play();
        }
    }
    public void FadeIn() {
        StartCoroutine(FadeIn(audioSource));
    }
    public void FadeOut() {
        StartCoroutine(FadeOut(audioSource));
    }
    // fades out an audioSource's audio clip over FadeTime seconds
    public IEnumerator FadeOut(AudioSource audioSource) {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0) {
            audioSource.volume -= Time.fixedUnscaledDeltaTime / fadeTime;

            yield return new WaitForFixedUpdate();
        }
        stoppedTime = audioSource.time;
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
    // fades in an audioSource's audio clip over FadeTime seconds
    public IEnumerator FadeIn(AudioSource audioSource) {
        audioSource.Play();
        audioSource.time = stoppedTime;
        audioSource.volume = 0;
        while (audioSource.volume < volume) {
            audioSource.volume += Time.fixedUnscaledDeltaTime / fadeTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
