using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Manager : MonoBehaviour
{
    public GameObject player;
    public List<Enemy> enemies;
    public Transform startingPosition;
    public AudioSource audioSource;
    public GameObject firstSign;                    // the first sign telling the player how to move and interact with signs

    private bool finishedLevel = false;             // prevents the finish level event from happening multiple times
    private void Start() {
        // get all enemies in the level in the enemies list
        Enemy enemyVar;
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemyObjects) {
            enemyVar = enemy.GetComponent<Enemy>();
            enemies.Add(enemyVar);
        }
        // initiallization
        audioSource = gameObject.GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        firstSign.GetComponent<Sign>().Interact();
    }
    public void Respawn() {
        player.transform.position = startingPosition.position;
        foreach(Enemy enemy in enemies) {
            enemy.Respawn();
        }
    }
    public void FinishLevel() {
        if (!finishedLevel) {
            finishedLevel = true;
            player.GetComponent<Player_Move>().finishedLevel = true;
            audioSource.PlayOneShot(audioSource.clip);
            GameObject.FindGameObjectWithTag("Music").SetActive(false);
            Invoke("LoadMainMenu", 9f);
        }
    }
    private void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
