using System.Collections;
using UnityEngine;

public class PingPongDistance : MonoBehaviour {

    // Direction to move
    public Vector3 MoveDir = Vector3.zero;

    // Speed to move (units per sec)
    public float Speed = 0.0f;

    // Distance to travel in world units (before inverting back)
    public float TravelDistance = 0.0f;

    // Cache the transform for performance
    private Transform ThisTransform = null;

    // Use this for initialization
    IEnumerator Start() {

        // Get cached transform
        ThisTransform = transform;

        // Loop forever
        while (true) {

            // Invert
            MoveDir = MoveDir * -1;

            // Start movement
            yield return StartCoroutine(Travel());
        }
    }

    // Travel full distance in direction, from curr position
    IEnumerator Travel() {

        // Distance travelled so far
        float DistanceTravelled = 0;

        // Move
        while (DistanceTravelled < TravelDistance) {

            // Get new position based on speed and direction
            Vector3 DistToTravel = MoveDir * Speed * Time.deltaTime;

            // Update position
            ThisTransform.position += DistToTravel;

            // Update distance travelled so far
            DistanceTravelled += DistToTravel.magnitude;

            // Wait until next update
            yield return null;
        }
    }
}
