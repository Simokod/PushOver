using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret_Door : MonoBehaviour
{
    public AudioSource audioSource;

    private float clipTimer = 0f;    // times how long until the timer can play again
    private void Start() {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Update() {
        if (clipTimer > 0)
            clipTimer -= Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            playClip();
        }
    }
    private void playClip() {
        if (clipTimer <= 0) {
            audioSource.PlayOneShot(audioSource.clip);
            clipTimer = audioSource.clip.length;
        }
    }
}
