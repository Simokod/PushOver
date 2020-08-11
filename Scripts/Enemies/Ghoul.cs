using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : Enemy
{
    public GameObject player;
    public AudioSource soundManager;
    public AudioClip punchClip;             // clip that plays when this enemy gets hit by the player
    public GameObject aggroObj;
    public Animator animator;
    public Rigidbody2D rb2d;
    public SpriteRenderer mySprite;
    public LayerMask tilesMask;
    public LayerMask spikesMask;
    public LayerMask playerMask;
    public Transform attackPoint;           // center of attack circle
    [System.NonSerializedAttribute]
    public bool isAggro = false;            // signals if this enemy is aggro'd by the player

    private Vector2 originGround;           // the point where the ground checking ray will be casted from
    private Vector2 originTiles;            // the point where the tiles checking ray will be casted from
    private Vector2 originSpikes;           // the point where the spikes checking ray will be casted from
    private Vector2 currentSpeed;           // the enemy's current movemnt speed
    private Vector2 startingPosition;       // the enemy's starting position
    private float width;                    // the width of the enemy sprite
    private float height;                   // the height of the enemy sprite
    private float stunTimer = 0;            // when the enemy is stunned he cannot move
    private float hitVecY = 0.25f;          // the y part of the push vector when getting hit
    private bool isKicked = false;          // signals if the enemy got kicked
    private int facingDir = -1;             // 1 == enemy is facing right, -1 == enemy is facing left
    private bool isAirborn = false;         // signals if the enemy is airborn because of the player's hit
    [SerializeField]
    private float deathTimer = 0.65f;
    [SerializeField]
    private float attackRange = 0.2f;        // range of this enemy's attack
    [SerializeField]
    private float idleSpeed = 1f;            // movement speed of the ghoul when it is idling
    [SerializeField]
    private float aggroSpeed = 1.5f;         // movement speed of the ghoul when it chases the player
    void Start() {
        player = GameObject.Find("Player");
        startingPosition = transform.position;
        width = mySprite.bounds.extents.x;
        height = mySprite.bounds.extents.y;
    }
    private void FixedUpdate() {
        if (stunTimer > 0) {
            stunTimer -= Time.deltaTime;
        }
        else                // if the enemy is in the air make sure it can't move and check for ground under him
            if (isAirborn) {                
                originGround = (Vector2)transform.position + (Vector2.right * width * 0.5f * facingDir) - (Vector2.up * height);
                Vector2 originDir = Vector2.down;
                RaycastHit2D groundRay = Physics2D.Raycast(originGround, originDir, 0.1f, tilesMask);
                if (groundRay) {
                    isAirborn = false;
                    isKicked = false;
                }
            }
            else           // the enemy is not airborn, move according to aggro state
                if (isAggro)
                    AggroWalk();
                else
                    IdleWalk();
    }
    // Walk pattern when the player is inside this enemy's aggro area
    private void AggroWalk() {
        float distance = player.transform.position.x - transform.position.x;
        if ((distance > 0 && facingDir < 0) || (distance < 0 && facingDir > 0))
            flip();
        currentSpeed = rb2d.velocity;
        currentSpeed.x = aggroSpeed * facingDir;
        rb2d.velocity = currentSpeed;
        // Check if the player is in attack range of the enemy, if yes, hit the player
        Collider2D collider = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerMask);
        if (collider)
            player.GetComponent<Player_Move>().GetHit(this.GetComponent<BoxCollider2D>());
    }
    // Walk pattern when the player is outside this enemy's aggro area
    private void IdleWalk() {
        // Ray to find ground
        originGround = (Vector2)transform.position + (Vector2.right * width * 0.5f * facingDir) - (Vector2.up * height);
        Vector2 originDir = Vector2.down;
        // Ray to find obstructing tiles
        originTiles = (Vector2)transform.position + (Vector2.right * width * 0.5f * facingDir);
        Vector2 obsDir = Vector2.right * facingDir;
        // Ray to find spikes
        originSpikes = (Vector2)transform.position + (Vector2.right * width * 0.5f * facingDir) - (Vector2.up * height * 0.75f);
        // Cast rays
        RaycastHit2D groundRay = Physics2D.Raycast(originGround, originDir, 0.1f, tilesMask);
        RaycastHit2D tilesRay = Physics2D.Raycast(originTiles, obsDir, 0.1f, tilesMask);
        RaycastHit2D spikesRay = Physics2D.Raycast(originSpikes, obsDir, 0.1f, spikesMask);
        // continue moving forward
        currentSpeed = rb2d.velocity;
        currentSpeed.x = idleSpeed * facingDir;
        rb2d.velocity = currentSpeed;
        // flip direction if blocked
        if (!groundRay || tilesRay || spikesRay) {
            flip();
        }
    }
    // Flip the enemy according to it's movement
    private void flip() {
        facingDir *= -1;
        Vector3 theScale = base.transform.localScale;
        theScale.x *= -1;
        base.transform.localScale = theScale;
    }
    // Push back the enemy after the player hit it
    public override void GetHit(bool pushDir, float power) {
        if (!isKicked) {
            isKicked = true;
            soundManager.PlayOneShot(punchClip);
            stunTimer = 0.45f;
            isAirborn = true;
            currentSpeed = Vector2.zero;
            Vector2 pushVec = new Vector2(pushDir ? 1 : -1, hitVecY);
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(pushVec * power, ForceMode2D.Impulse);
        }
    }
    // Play explode animation and turn off colliders and rigidbody
    public override void Die() {
        animator.SetTrigger("IsDead");
        stunTimer = deathTimer;
        Invoke("DestroySelf", deathTimer);
    }
    private void DestroySelf() {
        aggroObj.SetActive(false);
        gameObject.SetActive(false);
    }
    // When the enemy is pushed on spikes, kill him
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Spikes")
            Die();
    }
    // When the player walks into the aggro area of this enemy, aggro this enemy and play aggro sound
    // When the player walks out of the aggro area, unaggro this enemy
    public override void SetAggro(bool state) {
        isAggro = state;
        if (state)
            soundManager.PlayOneShot(soundManager.clip, 1f);
    }
    public override void Respawn() {
        transform.position = startingPosition;
        aggroObj.SetActive(true);
        gameObject.SetActive(true);
    }
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
