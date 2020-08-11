using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Door : MonoBehaviour {
    public Level1Manager lvlMngr;
    private void Start() {
        lvlMngr = GameObject.Find("LevelManager").GetComponent<Level1Manager>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
            lvlMngr.FinishLevel();
    }
}
