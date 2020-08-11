using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : InteractableObj {

    public GameObject popUpBox;
    public GameObject player;
    Animator animator;
    Player_Move player_move;

    private float interactStun = 0.2f;        // stun duration when interacting with a sign
    private void Start() {
        player = GameObject.Find("Player");
        player_move = player.GetComponent<Player_Move>();
        animator = popUpBox.GetComponent<Animator>();
    }
    public override void Interact() {
        animator.SetTrigger("Pop");
        player_move.SetStun(interactStun);
        player_move.readingSign = true;
    }
    public override void finishInteract() {
        animator.SetTrigger("Close");
        player_move.SetStun(interactStun);
        player_move.readingSign = false;
    }

    public override string getType() {
        return "sign";
    }
}
