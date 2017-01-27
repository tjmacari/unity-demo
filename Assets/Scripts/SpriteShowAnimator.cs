using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShowAnimator : MonoBehaviour {

    // Loop type
    public enum ANIMATOR_PLAYBACK_TYPE {PLAYONCE = 0, PLAYLOOP = 1};

    public ANIMATOR_PLAYBACK_TYPE PlaybackType = ANIMATOR_PLAYBACK_TYPE.PLAYONCE;

    // FPS
    public int FPS = 6;

    // Custom anim ID
    public int AnimationID = 0;

    // Frames of anim
    public SpriteRenderer[] Sprites = null;

    // Auto-play?
    public bool AutoPlay = false;

    // Hide sprites on start?
    public bool HideSpritesOnStart = true;

    // Currently playing?
    bool IsPlaying = false;

	void Start () {
		// Autoplay at startup?
        if(AutoPlay){StartCoroutine(PlaySpriteAnimation(AnimationID));}
	}

    public IEnumerator PlaySpriteAnimation(int AnimID = 0) {

        // Check if anim should be started, could be called via SendMessage or BroadcastMessage
        if(AnimID != AnimationID) yield break;

        // Should hide all sprite renderers?
        if (HideSpritesOnStart) {
            foreach (SpriteRenderer SR in Sprites) {
                SR.enabled = false;
            }
        }

        // Set is playing
        IsPlaying = true;

        // Calc delay time
        float DelayTime = 1.0f/FPS;

        // Run anim at least once
        do {
            foreach (SpriteRenderer SR in Sprites) {

                SR.enabled = !SR.enabled;
                yield return new WaitForSeconds (DelayTime);
                SR.enabled = !SR.enabled;
            }
        } while(PlaybackType == ANIMATOR_PLAYBACK_TYPE.PLAYLOOP);

        // Stop anim
        StopSpriteAnimation(AnimationID);
    }

    public void StopSpriteAnimation(int AnimID = 0) {

        // Check if anim can and should be stopped
        if((AnimID != AnimationID) || (!IsPlaying)) return;

        // Stop coroutines
        StopAllCoroutines();

        // Is playing false
        IsPlaying = false;

        // Send Sprite anim stopped event to Gameobject
        gameObject.SendMessage("SpriteAnimationStopped", AnimID, SendMessageOptions.DontRequireReceiver);
    }
}
