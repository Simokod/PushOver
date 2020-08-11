using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Aggro : MonoBehaviour
{
    public Enemy enemyScript;
    private void Awake() {
        transform.SetParent(null);
    }
private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            enemyScript.GetComponent<Enemy>().SetAggro(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            enemyScript.GetComponent<Enemy>().SetAggro(false);
        }
    }
}
