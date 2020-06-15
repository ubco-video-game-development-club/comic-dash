/* 
    Mario-style platformer player movement controller by Jaden Balogh.
*/

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlatformerController : MonoBehaviour
{
    [Tooltip("Can the player move past the left-hand side of the screen?")]
    public bool allowBacktracking = false;
    [Tooltip("The strength of the player's horizontal acceleration using the arrow keys.")]
    public float moveAcceleration = 15f;
    [Tooltip("The maximum horizontal speed of the player.")]
    public float maxMoveSpeed = 8f;
    [Tooltip("The horizontal drag slowing the player over time.")]
    public float horizontalDrag = 8f;
    [Tooltip("The horizontal velocity threshold below which the velocity will be clamped to 0.")]
    public float horizontalStopThreshold = 0.3f;
    [Tooltip("The strength of the constant downwards acceleration (gravity) on the player. NOTE: This controller overrides normal physics gravity on the player.")]
    public float fallAcceleration = 15f;
    [Tooltip("The maximum downwards falling speed of the player.")]
    public float maxFallSpeed = 8f;
    [Tooltip("The maximum height the player can jump when the spacebar is held for the full duration.")]
    public float maxJumpHeight = 3.2f;
    [Tooltip("Multiplies the strength of the downwards counter-force applied to the player if they cancel a jump early by releasing the spacebar.")]
    public float jumpCancelFactor = 5f;
    [Tooltip("The vertical velocity threshold above which the player is considered to be jumping.")]
    public float jumpVelocityThreshold = 0.1f;

    private bool isEnabled = true;
    private float jumpForce = 1f;
    private bool isJumpKeyHeld = false;
    private bool isGrounded = true;
    private float xOffset = 0.5f;
    private Rigidbody2D rb2D;
    private BoxCollider2D bc2D;

    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
        bc2D = GetComponent<BoxCollider2D>();
    }

    void Start() {
        // Turn off gravity from other physics sources
        rb2D.gravityScale = 0;
        // Calculate half the collider size for use in distance checks
        xOffset = bc2D.size.x / 2;
        // Calculate the jump force required to reach the max jump height
        jumpForce = CalculateJumpForce(maxJumpHeight);
    }

    void Update() {
        // Do not run if the controller is disabled
        if (!isEnabled) {
            return;
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            // Jump if the player presses spacebar while on the ground
            isJumpKeyHeld = true;
            isGrounded = false;
            rb2D.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        } else if (Input.GetButtonUp("Jump")) {
            // If the spacebar is released early, cancel the jump
            isJumpKeyHeld = false;
        }
    }

    void FixedUpdate() {
        // Do not run if the controller is disabled
        if (!isEnabled) {
            return;
        }

        // Determine if the player is at the left bound and backtracking is disabled
        float leftBound = Camera.main.ScreenToWorldPoint(Vector2.zero).x + xOffset;
        bool isBlocked = !allowBacktracking && transform.position.x <= leftBound;

        // Cancel horizontal movement if blocked
        if (isBlocked) {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            transform.position = new Vector3(leftBound, transform.position.y);
        }

        // Apply horizontal movement force if not blocked
        float moveForce = Input.GetAxisRaw("Horizontal") * moveAcceleration;
        moveForce = isBlocked ? Mathf.Max(0, moveForce) : moveForce;
        rb2D.AddForce(moveForce * Vector2.right);

        // Apply horizontal drag opposite movement direction
        float xMag = Mathf.Abs(rb2D.velocity.x);
        if (xMag > horizontalStopThreshold) {
            float xDir = Mathf.Sign(rb2D.velocity.x);
            rb2D.AddForce(horizontalDrag * -xDir * Vector2.right);
        }

        // Apply jump cancel force if spacebar is released early
        if (rb2D.velocity.y > jumpVelocityThreshold && !isJumpKeyHeld) {
            rb2D.AddForce(-jumpForce * jumpCancelFactor * Vector2.up);
        }

        // Apply reverse jump force when falling
        if (rb2D.velocity.y < -jumpVelocityThreshold) {
            rb2D.AddForce(-jumpForce * Vector2.up);
        }

        // Apply falling force (gravity)
        rb2D.AddForce(-fallAcceleration * Vector2.up);

        // Clamp velocity to set bounds
        Vector2 velocityX = Mathf.Clamp(rb2D.velocity.x, -maxMoveSpeed, maxMoveSpeed) * Vector2.right;
        Vector2 velocityY = Mathf.Max(rb2D.velocity.y, -maxFallSpeed) * Vector2.up;
        rb2D.velocity = velocityX + velocityY;

        // Set the player's horizontal velocity to 0 if it goes below the threshold
        if (moveForce == 0 && xMag <= horizontalStopThreshold) {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }
    }

    void OnCollisionStay2D(Collision2D col) {
        // for each contact point in this collision
        for (int i = 0; i < col.contactCount; i ++) {
            ContactPoint2D contact = col.GetContact(i);
            // if this contact point is flat ground
            if (Vector2.Dot(contact.normal, Vector2.up) == 1) {
                // you are now grounded
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col) {
        // you are no longer grounded
        isGrounded = false;
    }

    ///<summary>Disables all user input and cancels current velocity.</summary>
    public void Disable() {
        isEnabled = false;
        rb2D.velocity = Vector2.zero;
        bc2D.isTrigger = true;
    }

    ///<summary>Causes the player to bounce to a given height.</summary>
    public void Bounce(float bounceHeight) {
        float bounceForce = CalculateJumpForce(bounceHeight);
        rb2D.AddForce(bounceForce * Vector2.up, ForceMode2D.Impulse);
    }

    public bool IsMoving() {
        return Mathf.Abs(rb2D.velocity.x) > horizontalStopThreshold;
    }

    public bool IsJumping() {
        return rb2D.velocity.y > jumpVelocityThreshold;
    }

    public bool IsFalling() {
        return rb2D.velocity.y < -jumpVelocityThreshold;
    }

    public int GetDirection() {
        if (rb2D.velocity.x > horizontalStopThreshold) {
            return 1;
        } else if (rb2D.velocity.x < -horizontalStopThreshold) {
            return -1;
        } else {
            return 0;
        }
    }

    ///<summary>Returns the force required to jump a given height based on the current gravity settings.</summary>
    private float CalculateJumpForce(float jumpHeight) {
        return Mathf.Sqrt(2 * fallAcceleration * jumpHeight);
    }
}
