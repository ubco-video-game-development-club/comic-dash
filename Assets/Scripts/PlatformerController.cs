/* 
    Mario-style platformer player movement controller by Jaden Balogh.
*/

using UnityEngine;

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
    [Tooltip("The horizontal drag slowing the player over time.")]
    public float horizontalStopThreshold = 0.01f;
    [Tooltip("The strength of the constant downwards acceleration (gravity) on the player. NOTE: This controller overrides normal physics gravity on the player.")]
    public float fallAcceleration = 15f;
    [Tooltip("The maximum downwards falling speed of the player.")]
    public float maxFallSpeed = 8f;
    [Tooltip("The maximum height the player can jump when the spacebar is held for the full duration.")]
    public float maxJumpHeight = 3.2f;
    [Tooltip("Multiplies the strength of the downwards counter-force applied to the player if they cancel a jump early by releasing the spacebar.")]
    public float jumpCancelFactor = 5f;

    private float jumpForce = 1f;
    private bool isJumpKeyHeld = false;
    private bool isGrounded = true;
    private float xOffset = 0.5f;
    private Rigidbody2D rb2D;

    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
        xOffset = GetComponent<BoxCollider2D>().size.x / 2;
    }

    void Start() {
        // Turn off gravity from other physics sources
        rb2D.gravityScale = 0;
        // Calculate the jump force required to reach the max jump height
        jumpForce = Mathf.Sqrt(2 * fallAcceleration * maxJumpHeight);
    }

    void Update() {
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

        // Apply horizontal drag when no force is applied
        float xDir = rb2D.velocity.normalized.x;
        if (Mathf.Abs(xDir) > 0) {
            rb2D.AddForce(horizontalDrag * -xDir * Vector2.right);
        }

        // Set the player's horizontal velocity to 0 if it goes below the threshold
        // if (rb2D.velocity.x < horizontalStopThreshold) {
        //     rb2D.velocity = rb2D.velocity.y * Vector2.up;
        // }

        // Apply jump cancel force if spacebar is released early
        if (rb2D.velocity.y > 0.1f && !isJumpKeyHeld) {
            rb2D.AddForce(-jumpForce * jumpCancelFactor * Vector2.up);
        }

        // Apply reverse jump force when falling
        if (rb2D.velocity.y < -0.1f) {
            rb2D.AddForce(-jumpForce * Vector2.up);
        }

        // Apply falling force (gravity)
        rb2D.AddForce(-fallAcceleration * Vector2.up);

        // Clamp velocity to set bounds
        Vector2 velocityX = Mathf.Clamp(rb2D.velocity.x, -maxMoveSpeed, maxMoveSpeed) * Vector2.right;
        Vector2 velocityY = Mathf.Max(rb2D.velocity.y, -maxFallSpeed) * Vector2.up;
        rb2D.velocity = velocityX + velocityY;
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
}
