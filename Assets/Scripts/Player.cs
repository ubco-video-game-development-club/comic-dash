using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerController))]
public class Player : MonoBehaviour
{
    private float bottomBound;
    private PlatformerController platformerController;

    void Awake() {
        platformerController = GetComponent<PlatformerController>();
    }

    void Start() {
        float yOffset = GetComponent<BoxCollider2D>().size.y / 2;
        bottomBound = Camera.main.ScreenToWorldPoint(Vector2.zero).y - yOffset;
    }

    void Update() {
        if (transform.position.y < bottomBound) {
            Die();
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
