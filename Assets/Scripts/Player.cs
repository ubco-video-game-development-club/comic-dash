using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 1f;
    public float jumpDecay = 1f;

    private bool isJumping = false;
    private bool isGrounded = true;
    private float currentJumpForce = 1f;
    private Rigidbody2D rb2D;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        currentJumpForce = jumpForce;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            isJumping = true;
            isGrounded = false;
            currentJumpForce = jumpForce;
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            isJumping = false;
        }

        if (isJumping) {
            currentJumpForce -= jumpDecay * Time.deltaTime;
        }
    }

    void FixedUpdate() {
        Vector2 velocityX = Input.GetAxis("Horizontal") * moveSpeed * Vector2.right;

        Vector2 velocityY = rb2D.velocity * Vector2.up;
        if (rb2D.velocity.y > 0 && isJumping) {
            velocityY = currentJumpForce * Vector2.up;
        }

        // rb2D.velocity = velocityX + velocityY;
        rb2D.AddForce(velocityX + velocityY);
    }

    void OnCollisionEnter2D(Collision2D col) {
        // for each contact point in this collision
        foreach (ContactPoint2D cp in col.contacts) {
            // if this contact point is flat ground
            if (Vector2.Dot(cp.normal, Vector2.up) == 1) {
                // you are now grounded
                isGrounded = true;
                break;
            }
        }
    }
}
