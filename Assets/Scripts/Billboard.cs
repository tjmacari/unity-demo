// Used for our collectable items, so always facing player, like Wolfenstien 3D
using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    private Transform ThisTransform = null; // simply instantiate

	// Use this for initialization
	void Start () {

        // Cache the transform, so slightly better performance than transforming directly
        ThisTransform = transform;
	}

    // Same as Update, but ALWAYS called AFTER, good so cameras can rotate first to avoid odd angle
	void LateUpdate () {

        // Rotate the object to this coordinate, so ALWAYS facing the player
        Vector3 LookAtDir = new Vector3 (
            Camera.main.transform.position.x - ThisTransform.position.x, //x
            0, //y
            Camera.main.transform.position.z - ThisTransform.position.z //z
        );

        // Apply rotation
        // Quaternions describe orientation in 3d space, needed anytime you need to turn an obj or look a certain direction
        ThisTransform.rotation = Quaternion.LookRotation(-LookAtDir.normalized, Vector3.up);
	}
}
