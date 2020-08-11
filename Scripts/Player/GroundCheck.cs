using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
    public GameObject player;
    public Player_Move playerMove;
    private const float CollisionRadius = 0.2f;
    private void FixedUpdate() {
        bool flag = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, CollisionRadius);
        for (int i = 0; i < colliders.Length; i++) {                     // Checks if the character is on the ground by checking all collisions
            if (colliders[i].tag == "Ground") {
                flag = true;
                playerMove.isGrounded = true;
            }
        }
        // Reset isGrounded when the character lands   
        if (!flag)
            playerMove.isGrounded = false;
    }
    //private void FixedUpdate() {

    //}
}
