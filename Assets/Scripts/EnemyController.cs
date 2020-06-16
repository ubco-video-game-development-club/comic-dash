using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class EnemyController : MonoBehaviour
{
    [Tooltip("If true, the enemy will not start moving until it becomes visible to the player.")]
    public bool idleOffScreen = true;
    [Tooltip("If true, the enemy will change directions to avoid falling off edges.")]
    public bool avoidFalling = false;
    [Tooltip("The layer that is considered ground for direction change raycasts.")]
    public LayerMask groundLayer;
    [Tooltip("The initial direction of the enemy's movement.")]
    public Vector2 startDirection = Vector2.left;
    [Tooltip("The velocity of the enemy.")]
    public float moveSpeed = 1.5f;
    [Tooltip("The cooldown between direction changes.")]
    public float directionChangeCooldown = 0.1f;
    [Tooltip("The maximum bounce height applied to the player upon killing this enemy.")]
    public float deathBounceHeight = 1f;
    [Tooltip("The score given upon killing this enemy.")]
    public int deathScore = 100;

    private bool idle;
    private bool canChangeDirections;
    private float bottomBound;
    private float xOffset;
    private float yOffset;
    private Vector2 moveDirection;
    private Rigidbody2D rb2D;

    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start() {
        xOffset = GetComponent<BoxCollider2D>().size.x / 2;
        yOffset = GetComponent<BoxCollider2D>().size.y / 2;
        bottomBound = Camera.main.ScreenToWorldPoint(Vector2.zero).y - yOffset;

        moveDirection = startDirection;
        canChangeDirections = true;
        idle = true;
    }

    void Update() {
        float rightBound = Camera.main.ScreenToWorldPoint(Screen.width * Vector2.right).x + xOffset;
        if (transform.position.x < rightBound) {
            idle = false;
        }

        if (transform.position.y < bottomBound) {
            Die();
        }

        if (avoidFalling && canChangeDirections) {
            Vector2 leftRayPos = transform.position + xOffset * Vector3.left + yOffset * Vector3.down;
            RaycastHit2D leftHit = Physics2D.Raycast(leftRayPos, Vector2.down, 0.5f, groundLayer);

            Vector2 rightRayPos = transform.position + xOffset * Vector3.right + yOffset * Vector3.down;
            RaycastHit2D rightHit = Physics2D.Raycast(rightRayPos, Vector2.down, 0.5f, groundLayer);

            if (!leftHit || !rightHit) {
                moveDirection *= -1;
                StartCoroutine(DirectionChangeCooldown());
            }
        }
    }

    void FixedUpdate() {
        if (idle) {
            return;
        }

        rb2D.velocity = moveSpeed * moveDirection + rb2D.velocity.y * Vector2.up;
    }

    void OnCollisionEnter2D(Collision2D col) {
        Player player;
        if (col.gameObject.TryGetComponent<Player>(out player)) {
            if (Vector2.Dot(col.GetContact(0).normal, Vector2.up) == -1) {
                player.GetController().Bounce(deathBounceHeight);
                GameController.instance.AddScore(deathScore);
                Die();
            } else {
                player.Die();
            }
        } else {
            if (Vector2.Dot(col.GetContact(0).normal, Vector2.up) == 0) {
                moveDirection *= -1;
            }
        }
    }

    public void Die() {
        Destroy(gameObject);
    }

    private IEnumerator DirectionChangeCooldown() {
        canChangeDirections = false;
        yield return new WaitForSeconds(directionChangeCooldown);
        canChangeDirections = true;
    }
}
