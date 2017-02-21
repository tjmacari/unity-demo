using System.Collections;
using UnityEngine;

public class PingPongSpriteColor : MonoBehaviour {

    // Source (from) color
    public Color Source = Color.white;

    // Dest (to) color
    public Color Dest = Color.white;

    // Custom ID for this anim
    public int AnimationID = 0;

    // Tot time in secs to transition from source to dest
    public float TransitionTime = 1.0f;

    // List of sprite renders whose color must be set
    private SpriteRenderer[] SpriteRenderers = null;

	// Use this for initialization
	void Start () {

        // Get all child sprite renderers
        SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
	}

    public void PlayColorAnimation(int AnimID = 0) {

        if (AnimationID != AnimID)
            return;

        StopAllCoroutines ();

        StartCoroutine (PlayLerpColors ());
    }

    // Start anim
    private IEnumerator PlayLerpColors() {
        yield return StartCoroutine (LerpColor (Source, Dest));
        yield return StartCoroutine (LerpColor (Dest, Source));
    }

    private IEnumerator LerpColor(Color X, Color Y) {

        // Maintain elapsed time
        float ElapsedTime = 0.0f;

        // Loop for transition time
        while (ElapsedTime <= TransitionTime) {

            // Update Elapsed Time
            ElapsedTime += Time.deltaTime;

            // Set sprite renderer colors
            foreach (SpriteRenderer SR in SpriteRenderers) {
                SR.color = Color.Lerp (X, Y, Mathf.Clamp (ElapsedTime / TransitionTime, 0.0f, 1.0f));
            }

            // Wait until next frame
            yield return null;
        }

        // Set dest color
        foreach(SpriteRenderer SR in SpriteRenderers) SR.color = Y;
    }
	
}
