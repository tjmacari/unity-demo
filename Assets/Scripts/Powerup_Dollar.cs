// 1. Increase amount of cash, 2. Play collection sound, 3. Remove power-up from scene
using UnityEngine;
using System.Collections;

public class Powerup_Dollar : MonoBehaviour {

    // Amount of cash incremeted each time for player
    public float CashAmount = 100.0f;

    // Audio source for sound playback
    private AudioSource SFX = null;

    // Audio clip
    public AudioClip Clip = null;

	void Start () {

        // Find sound obj in scene
        GameObject SoundsObject = GameObject.FindGameObjectWithTag("powerSound");

        // Get audio source component for sfx
        SFX = SoundsObject.GetComponent<AudioSource>();
	}

    // Event triggered when colliding with player
    void OnTriggerEnter(Collider Other) {

        // Is collider object a player?  Can't collide with enemies
        if(!Other.CompareTag("player")) return;

        Debug.Log ("playing sounds...");

        // Play collection sound, if audio source is available
        SFX.PlayOneShot(Clip, 1.0f);

        // Hide away so can't be recollected later
        gameObject.SetActive(false);

        // Get PlayerController object and update cash
        PlayerController PC = Other.gameObject.GetComponent<PlayerController>();

        // If there is a PC attached to colliding object, update cash
        if(PC) PC.Cash += CashAmount;

        // Post power up collected notification - so other objects can handle if needed, using static prop 'Nofications'
        GameManager.Notifications.PostNotification(this, "PowerupCollected");
    }
	
}
