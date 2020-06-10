using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    /* 
        Mario-style platformer player movement controller by Jaden Balogh.
    */

    [Tooltip("The maximum horizontal speed of the player.")]
    public float maxMoveSpeed = 6f;
    [Tooltip("The maximum downwards falling speed of the player.")]
    public float maxFallSpeed = 8f;
    [Tooltip("The strength of the player's horizontal acceleration using the arrow keys.")]
    public float moveAcceleration = 15f;
    [Tooltip("The strength of the constant downwards acceleration (gravity) on the player. NOTE: This controller overrides normal physics gravity on the player.")]
    public float fallAcceleration = 15f;
    [Tooltip("The maximum height the player can jump when the spacebar is held for the full duration.")]
    public float maxJumpHeight = 3.2f;
    [Tooltip("Multiplies the strength of the downwards counter-force applied to the player if they cancel a jump early by releasing the spacebar.")]
    public float jumpCancelFactor = 5f;

    private float jumpForce = 1f;
    private bool isJumpKeyHeld = false;
    private bool isGrounded = true;
    private Rigidbody2D rb2D;

    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
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
            // If the spacebar is released early, trigger the jump cancel
            isJumpKeyHeld = false;
        }
    }

    void FixedUpdate() {
        // Apply horizontal movement force
        rb2D.AddForce(Input.GetAxisRaw("Horizontal") * moveAcceleration * Vector2.right);

        // Apply jump cancel force if spacebar is released early
        if (rb2D.velocity.y > 0.1f && !isJumpKeyHeld) {
            rb2D.AddForce(-jumpForce * jumpCancelFactor * Vector2.up);
        }

        // Apply reverse jump force when falling
        if (rb2D.velocity.y < -0.1f) {
            rb2D.AddForce(-jumpForce * Vector2.up);
        }

        // Apply falling force
        rb2D.AddForce(-fallAcceleration * Vector2.up);

        // Clamp velocity
        Vector2 clampedX = Mathf.Clamp(rb2D.velocity.x, -maxMoveSpeed, maxMoveSpeed) * Vector2.right;
        Vector2 clampedY = Mathf.Max(rb2D.velocity.y, -maxFallSpeed) * Vector2.up;
        rb2D.velocity = clampedX + clampedY;
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
