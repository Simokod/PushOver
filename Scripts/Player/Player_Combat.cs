using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    Animator animator;
    AudioSource audioSource;
    Player_Move playerController;
    Rigidbody2D rb2d;
    public LayerMask enemyLayer;            // enemies hit by this attack
    public Transform punchPoint;            // center of punch circle
    public Transform kickPoint;             // center of kick circle
    public AudioClip punchClip;

    public float punchRadius = 0.52f;       // radius around the center which counts inside the attack range
    public float punchPower = 5f;           // pushback power of the player's punch
    public float punchCooldown = 0.2f;      // cooldown of a punch; time in seconds the player is stunned after punching
    public float kickFlyTime = 0.2f;        // the amount of time the player kick lasts
    public float kickRadius = 0.5f;         // radius around the center which counts inside the attack range
    public float kickPower = 8f;            // pushback power of the player's punch
    public float kickHitFreq = 0.25f;       // the frequency of the kick hit detection
    private float delayPunchTime = 0f;      // delays the punch trigger collider
    private void Start() {
        animator = gameObject.GetComponentInParent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        playerController = gameObject.GetComponent<Player_Move>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Punch(bool facingRight) {
        playerController.SetStun(punchCooldown);
        audioSource.PlayOneShot(punchClip);
        animator.SetTrigger("IsPunching");
        StartCoroutine(HitEnemies(facingRight));
    }
    public void FlyingKick(bool facingRight) {
        playerController.isKicking = true;
        playerController.hasKicked = true;
        playerController.kickTimer = kickFlyTime;
        rb2d.gravityScale = 0f;
        rb2d.velocity = Vector2.zero;
        // insert flying kick sounds
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsKicking", true);
        StartCoroutine(KickFly(facingRight));
    }
    private IEnumerator HitEnemies(bool facingRight) {
        yield return new WaitForSeconds(delayPunchTime);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPoint.position, punchRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<Enemy>().GetHit(facingRight, punchPower);
        }
    }
    private IEnumerator KickFly(bool facingRight) {
        Collider2D[] hitEnemies;
        while (playerController.isKicking) {
            yield return new WaitForFixedUpdate(); 
            hitEnemies = Physics2D.OverlapCircleAll(kickPoint.position, kickRadius, enemyLayer);
            foreach (Collider2D enemy in hitEnemies) {
                enemy.GetComponent<Enemy>().GetHit(facingRight, kickPower);
                playerController.GetHit(enemy);
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
        Gizmos.DrawWireSphere(kickPoint.position, kickRadius);
    }
}
