using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class EnemyController : MonoBehaviour
{
    private Vector2 startDirection = Vector2.right;
    public float moveSpeed = 1.5f;
    public float deathBounceHeight = 1f;

    private float bottomBound;
    private Vector2 moveDirection;
    private Rigidbody2D rb2D;

    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start() {
        float yOffset = GetComponent<BoxCollider2D>().size.y / 2;
        bottomBound = Camera.main.ScreenToWorldPoint(Vector2.zero).y - yOffset;

        moveDirection = startDirection;
    }

    void Update() {
        if (transform.position.y < bottomBound) {
            Die();
        }
    }

    void FixedUpdate() {
        rb2D.velocity = moveSpeed * moveDirection + rb2D.velocity.y * Vector2.up;
    }

    void OnCollisionEnter2D(Collision2D col) {
        Player player;
        if (col.gameObject.TryGetComponent<Player>(out player)) {
            if (Vector2.Dot(col.GetContact(0).normal, Vector2.up) == -1) {
                player.GetController().Bounce(deathBounceHeight);
                Die();
            } else {
                player.Die();
            }
        } else {
            moveDirection *= -1;
        }
    }

    public void Die() {
        Destroy(gameObject);
    }
}
