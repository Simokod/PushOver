using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public Animator animator;
    public Player_Combat playerCombat;
    public AudioSource audioSource;
    public TimeManager timeMngr;
    public Level1Manager lvlMngr;
    public BoxCollider2D myCollider;
    public MusicManager bgMusicController;
    public LayerMask tilesMask;
    public PauseMenu pauseMenu;

    [System.NonSerializedAttribute]
    public bool isGrounded = false;         // signals if the character touches the ground (with it's feet)
    [System.NonSerializedAttribute]
    public bool finishedLevel = false;      // literally true if the player finished the level
    [System.NonSerializedAttribute]
    public bool isKicking = false;          // signals if the player is doing a flying kick
    [System.NonSerializedAttribute]
    public bool hasKicked = false;          // signals if the player has already kicked during this jump
    [System.NonSerializedAttribute]
    public float kickTimer = 0f;            // time the player flies after kicking

    [SerializeField]
    private float speed = 5f;               // the movement speed of the player
    [SerializeField]
    private float jumpForce = 5f;           // the force the player jumps with
    [SerializeField]
    private float pushForce = 5f;           // the force applied to the player when getting pushed
    [SerializeField]
    private float fallMultiplier = 2.5f;    // controls the gravity affecting the player when falling down
    [SerializeField]
    private float lowJumpMultiplier = 2f;   // cotrols the gravity affecting the player when not doing a full jump
    [SerializeField]
    private float respawnTime = 0f;         // time it takes for the player to respawn after dying
    [SerializeField]
    private float deathAudioFadeTime = 0f;  // total time the death music plays
    [SerializeField]
    private float kickSpeed = 7f;           // the speed in which the player flies while kicking
    private float currentMoveSpeed = 0f;    // current movement speed, used for animations
    private float stunTimer = 0;            // countdown after getting stunned (occurs after punching or getting hit)
    private float stunDuration = 0.7f;      // time in seconds the player is stunned after getting hit
    private float landingSpeedMult = 1.5f;  // speed multiplier when player lands
    private bool facingRight = true;        // signals the direction the character is facing
    private bool isLanding = false;         // signals if the playing is landing
    private bool isHit = false;             // prevents the player from getting hit twice
    private bool isDead = false;            // signals if the player is literally dead
    private float width;                    // the width of the character sprite
    private float height;                   // the height of the character sprite

    private bool jumpWasPressed = false;    // buffers the jump action if the player pressed the key before touching the ground
    private bool coyoteTime = false;        // allows the player to jump even after leaving the platform
    [SerializeField]
    private float coyoteDelay = 0.1f;       // how long after leaving the platform the player can jump
    [SerializeField]
    private float bufferJumpDelay = 0.1f;   // how long the game will remember the jump press before the player lands

    // 3 rays for detecting ground, left, middle, right
    private RaycastHit2D lRay;
    private RaycastHit2D mRay;
    private RaycastHit2D rRay;

    private GameObject interactObj = null;  // interactable object in range of the player
    public float interactTimer = 0f;        // prevents interacting too many times instantly
    public bool readingSign = false;        // true iff the player is reading a sign

    private void Start() {
        bgMusicController = GameObject.FindGameObjectsWithTag("Music")[0].GetComponent<MusicManager>();
        width = gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
        height = gameObject.GetComponent<SpriteRenderer>().bounds.extents.y;
    }
    void Update() {
        // if the player is kicking, his only actions are to drop down or wait
        if (isKicking) {
            currentMoveSpeed = kickSpeed * (facingRight ? 1 : -1);
            if (kickTimer < 0) {
                endKick();
                animator.SetBool("IsFalling", true);
            }
            else {
                kickTimer -= Time.deltaTime;
                if ((Input.GetKeyDown("down") || Input.GetKeyDown("s")) && !isGrounded)
                    Land();
            }
        }
        // unable to move when stunned
        else if (stunTimer > 0 || isLanding || isDead) {
            stunTimer -= Time.deltaTime;
            currentMoveSpeed = 0;
        }
        // the player can act normally
        else {
            if (pauseMenu.isPaused())
                return;
            // if the player is reading a sign, any key will finish reading
            if (readingSign)
                if (Input.anyKey) {
                    interactObj.GetComponent<InteractableObj>().finishInteract();
                    return;
                }
            currentMoveSpeed = Input.GetAxisRaw("Horizontal") * speed;
            if (Input.GetKeyDown("up") || Input.GetKeyDown("w")) {
                jumpWasPressed = true;
                Invoke("bufferJump", bufferJumpDelay);
                if (isGrounded || coyoteTime)
                    Jump();
            }
            if (Input.GetKeyDown("e") && isGrounded && !readingSign) {
                checkObject();
                return;
            }
            if ((Input.GetKeyDown("down") || Input.GetKeyDown("s")) && !isGrounded)
                Land();
            if (Input.GetKeyDown("space") && isGrounded)
                playerCombat.Punch(facingRight);
            if (Input.GetKeyDown("space") && canKick())
                playerCombat.FlyingKick(facingRight);
        }
    }
    private void FixedUpdate() {
        transform.position += (new Vector3(currentMoveSpeed, 0, 0)) * Time.fixedDeltaTime;
        if (!isKicking) {
            animator.SetFloat("CurrentSpeed", Mathf.Abs(currentMoveSpeed));
            FlipChar(currentMoveSpeed);
            checkGround();

            if (rb2d.velocity.y > -0.01 && rb2d.velocity.y < 0.01 && isGrounded) {
                hasKicked = false;
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            }
            if (rb2d.velocity.y < -0.2) {       // detect falling when speed is high enough
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;  // increase gravity when falling down
                Invoke("CoyoteTime", coyoteDelay);
            }
            else if (isGrounded) {
                coyoteTime = true;
                animator.SetBool("IsFalling", false);
                //animator.SetBool("IsJumping", false);
                isLanding = false;
                // if the jump button was pressed before the player touched the ground (jumpBufferDelay seconds), make the player jump as he lands
                if (jumpWasPressed)
                    Jump();
                if (stunTimer < 0) {         // if the timer finished, the player can move after he lands
                    stunTimer = 0;
                    rb2d.velocity = Vector2.zero;
                }
            }
            // the player is not grounded, pressing down the jump key for longer allows for higher jump
            else {
                Invoke("CoyoteTime", coyoteDelay);
                if (rb2d.velocity.y > 0.1 && !(Input.GetKey("up") || Input.GetKey("w")))
                    rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }
    private void Jump() {
        animator.SetBool("IsJumping", true);
        animator.SetBool("IsFalling", false);
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }
    private void Land() {
        if (isKicking)
            endKick();
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", true);
        isLanding = true;
        rb2d.velocity = new Vector2(0, -jumpForce * landingSpeedMult);
    }
    private void FlipChar(float direction) {
        if((facingRight && direction < 0) || (!facingRight && direction > 0)) {
            facingRight = !facingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
    // Plays the player_hit animation and pushes the player character in the direction opposite to the collision location
    public void GetHit(Collider2D collision) {
        if (!isHit && !isDead && !finishedLevel) {
            if (isKicking)
                endKick();
            isHit = true;
            SetStun(stunDuration);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
            animator.SetTrigger("IsHit");
            Vector2 collisionLoc = collision.transform.position;
            Vector2 myLoc = transform.position;
            Vector2 pushDir = new Vector2((myLoc.x - collisionLoc.x) / Mathf.Abs(myLoc.x - collisionLoc.x), 1).normalized;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
            if (collision.CompareTag("Spikes")) {   // if the player is hit by spikes, kill him, play death song and respawn the level
                isDead = true;
                animator.SetBool("IsDead", true);
                bgMusicController.FadeOut();
                audioSource.PlayOneShot(audioSource.clip);
                StartCoroutine(FadeOut(deathAudioFadeTime));
                timeMngr.SlowMow();
                Invoke("Die", respawnTime);
            }
            else
                Invoke("StopHit", 0.05f);
        }
    }
    // prevents the game from detecting two collisions instead of only one
    private void StopHit() {
        isHit = false;
    }
    // kills the player and resets the level
    private void Die() {
        animator.SetBool("IsDead", false);
        isDead = false;
        isHit = false;
        lvlMngr.Respawn();
    }
    // stuns the player
    public void SetStun(float duration) {
        stunTimer = duration;
    }
    // check for collisions
    private void OnTriggerEnter2D(Collider2D collision) {
        // if colliding with enemy, trigger hit
        if (collision.tag == "Enemy")
            GetHit(collision);
        // if colliding with spikes trigger hit
        if (collision.tag == "Spikes")
            GetHit(collision);
        // if player enters interactable object's range, save a reference to that object
        if (collision.tag == "Interactable")
            interactObj = collision.gameObject;
    }
    // clear interactable object when leaving range
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Interactable") {
            if (interactObj.GetComponent<InteractableObj>().getType() == "sign" && readingSign)
                interactObj.GetComponent<InteractableObj>().finishInteract();
            interactObj = null;
        }
    }
    // fades out an audioSource's audio clip over FadeTime seconds
    public IEnumerator FadeOut(float fadeTime) {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0) {
            audioSource.volume -= (Time.fixedUnscaledDeltaTime / fadeTime) * startVolume;

            yield return new WaitForFixedUpdate();
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
        bgMusicController.FadeIn();
    }
    // check if the player is in the air, if he has kicked already in the same flight, and if he is starting to go down
    private bool canKick() {
        return !isGrounded && !hasKicked && rb2d.velocity.y < 0.1;
    }
    // check if the player is grounded by sending rays downwards
    private void checkGround() {
        int facingDir = facingRight ? 1 : -1;
        if (rb2d.velocity.y < -0.05) {
            isGrounded = false;
            return;
        }
        // create the origin of the rays
        Vector2 lOrigin = (Vector2)transform.position + (Vector2.right * width * 0.5f * facingDir) - (Vector2.up * height);
        Vector2 mOrigin = (Vector2)transform.position - (Vector2.up * height);
        Vector2 rOrigin = (Vector2)transform.position + (Vector2.left * width * 0.5f * facingDir) - (Vector2.up * height);
        // send rays downwards to detect ground
        lRay = Physics2D.Raycast(lOrigin, Vector2.down, 0.1f, tilesMask);
        mRay = Physics2D.Raycast(mOrigin, Vector2.down, 0.1f, tilesMask);
        rRay = Physics2D.Raycast(rOrigin, Vector2.down, 0.1f, tilesMask);
        // if one of the rays hit something, it means the player is grounded
        isGrounded = lRay || mRay || rRay;
    }
    private void CoyoteTime() {
        coyoteTime = false;
    }
    private void bufferJump() {
        jumpWasPressed = false;
    }
    // reset stats after finishing a flying kick
    private void endKick() {
        isKicking = false;
        rb2d.gravityScale = 1f;
        kickTimer = 0;
        animator.SetBool("IsKicking", false);
    }
    // when the player presses the 'interact button'('E'), check interactable objects in range, and interact if possible
    private void checkObject() {
        if (interactObj != null) {
            interactObj.GetComponent<InteractableObj>().Interact();
        }
    }
}


