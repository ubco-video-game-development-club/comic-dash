using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerController), typeof(SpriteRenderer), typeof(Animator))]
public class Player : MonoBehaviour
{
    private float bottomBound;
    private PlatformerController platformerController;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake() {
        platformerController = GetComponent<PlatformerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        float yOffset = GetComponent<BoxCollider2D>().size.y / 2;
        bottomBound = Camera.main.ScreenToWorldPoint(Vector2.zero).y - yOffset;
    }

    void Update() {
        if (transform.position.y < bottomBound) {
            Die();
        }

        animator.SetBool("IsMoving", platformerController.IsMoving());
        animator.SetBool("IsJumping", platformerController.IsJumping());
        animator.SetBool("IsFalling", platformerController.IsFalling());

        int xDir = platformerController.GetDirection();
        if (xDir != 0) {
            spriteRenderer.flipX = xDir < 0;
        }
    }

    public void Die() {
        platformerController.Disable();
        GameController.instance.Die();
    }

    public PlatformerController GetController() {
        return platformerController;
    }
}
