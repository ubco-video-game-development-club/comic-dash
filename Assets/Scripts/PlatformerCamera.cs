/* 
    Mario-style camera-follow script by Jaden Balogh.
*/

using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    [Tooltip("The target object that the camera will be following (typically the player).")]
    public Transform target;
    [Tooltip("Does the camera follow the target to the left?")]
    public bool allowBacktracking = false;
    [Tooltip("The time it takes for the camera to move to the target's current location.")]
    public float followTime = 0.1f;

    private Vector3 currentVelocity;

    void FixedUpdate() {
        // Follow the target based on set conditions
        if (allowBacktracking || target.position.x > transform.position.x) {
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, followTime);
        }
    }
}
