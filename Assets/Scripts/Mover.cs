using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

    // Speed to move, units per sec (5 units/sec)
    public float Speed = 5.0f;

    // Cached transform
    private Transform ThisTransform = null;

	// Move spaceship
	void Update () {

        // Update position on x, better to use exact delta/difference than to be dependent on Frames/sec
        ThisTransform.position += new Vector3(Speed * Time.deltaTime, 0, 0);
	}
}
