using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    // Amount of cash for player to collect to complete level
    public float CashTotal = 1100.0f;

    // Amount of cash player has
    private float cash = 0.0f;

    // Reference for transform
    private Transform ThisTransform = null;

    // Respawn after death, time in seconds
    public float RespawnTime = 2.0f;

    // Get Macanim animator component in children
    private Animator AnimComp = null;

    // Player health
    public int health = 100;

    // Private damage texture
    private Texture2D DamageTexture = null;

    // Screen coords
    private Rect ScreenRect;

    // Show damage texture yet?
    private bool ShowDamage = false;

    // Damage texture interval (amount of time in secs to show texture)
    private float DamageInterval = 0.2f;

    // Default player weapon (punch)
    public Weapon DefaultWeapon = null;

    // Current active weapon
    public Weapon ActiveWeapon = null;

    // Called when object is created
    void Start() {

        // Register controller for weapon expriation events
        GameManager.Notifications.AddListener(this, "AmmoExpired");

        // Activate default weapon
        DefaultWeapon.gameObject.SendMessage("Equip", DefaultWeapon.Type);

        // Set active weapon
        ActiveWeapon = DefaultWeapon;

        // Get first person capsule and make non-visible
        //MeshRenderer Capsule = GetComponentInChildren<MeshRenderer>();
        //Capsule.enabled = false;
       
        AnimComp = GetComponentInChildren<Animator> ();

        // Create 1px 50% opaque red dot, then stretch it to fill screen, to indicate damage
        DamageTexture = new Texture2D(1, 1);
        DamageTexture.SetPixel (0, 0, new Color (255, 0, 0, 0.5f));
        DamageTexture.Apply ();

        // Get cached transform
        ThisTransform = transform;
    }

    // Accessors to get/set cash
    public float Cash {

        // Grab private var
        get{ return cash; }

        // Update private var
        set {
            // Set cash
            cash = value;
            Debug.Log (cash+"/"+CashTotal);

            // Check if we've beaten level
            if (cash >= CashTotal) {
                Debug.Log ("LEVEL COMPLETE!!!!!!");
                GameManager.Notifications.PostNotification (this, "CashCollected");
            }
        }
    }

    // Health accessors
    public int Health {

        // Return health val
        get {return health;}

        // Set health and validate, if required
        set {
            health = value;
            Debug.Log (health);

            if (health <= 0)
                gameObject.SendMessage ("Die", SendMessageOptions.DontRequireReceiver);
        }
    }

    // Apply damage
    public IEnumerator ApplyDamage(int Amount = 0) {

        // Reduce health
        Health -= Amount;

        // Post damage notification
        GameManager.Notifications.PostNotification(this, "PlayerDamaged");

        // Show damage texture
        ShowDamage = true;

        // Wait for interval
        yield return new WaitForSeconds(DamageInterval);

        // Hide damage texture
        ShowDamage = false;
    }

    // ON GUI function to draw our new red texture, OnGUI is called more often than even 'update' so it's expensive,
    // so keep the functionality simple (ie. simple red rect)
    void OnGUI() {

        if (ShowDamage)
            GUI.DrawTexture (ScreenRect, DamageTexture);
    }

    // Kill our character
    public IEnumerator Die() {

        // Disable input
        GameManager.Instance.InputAllowed = false;

        if (AnimComp)
            AnimComp.SetTrigger ("ShowDeath");

        // Wait for respawn time to expire
        yield return new WaitForSeconds(RespawnTime);

        //Restart level
        Application.LoadLevel(Application.loadedLevel);
    }

    void Update() {

        // Build screen rectangle on update (in case screen size changes)
        ScreenRect.x = ScreenRect.y = 0;
        ScreenRect.width = Screen.width;
        ScreenRect.height = Screen.height;

        if(Input.GetKeyDown(KeyCode.Period))
            EquipNextWeapon();
            
    }

    public void EquipNextWeapon() {

        bool bFoundWeapon = false;

        while (!bFoundWeapon) {

            // Get next weapon
            ActiveWeapon = ActiveWeapon.NextWeapon;

            // Activate Weapon, if possible
            ActiveWeapon.gameObject.SendMessage("Equip", ActiveWeapon.Type);

            // Equipped?
            bFoundWeapon = ActiveWeapon.IsEquipped;
        }
    }

    public void AmmoExpired(Component Sender) {

        EquipNextWeapon ();
    }
}
