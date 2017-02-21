using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WEAPON_TYPE {Punch=0, Gun = 1};

    public WEAPON_TYPE Type = WEAPON_TYPE.Punch;

    public float Damage = 0.0f;

    // Linear distance from camera in units
    public float Range = 1.0f;

    public int Ammo = -1;

    public float RecoveryDelay = 0.0f;

    public bool Collected = false;

    public bool IsEquipped = false;

    public bool CanFire = true;

    public Weapon NextWeapon = null;

    // Show active sprite when not attacking
    public SpriteRenderer DefaultSprite = null;

    // Sounds to play on attack
    public AudioClip Clip = null;

    // Audio source for sound playback
    protected AudioSource SFX = null;

    // Reference to all child sprite renderers for this weapon
    protected SpriteRenderer[] WeaponSprites = null;

    void Start() {

        GameObject SoundsObject = GameObject.FindGameObjectWithTag ("sounds");

        if (SoundsObject == null)
            return;

        SFX = SoundsObject.GetComponent<AudioSource> ();

        // Get all child sprite renderers for weapon
        WeaponSprites = gameObject.GetComponentsInChildren<SpriteRenderer> ();

        // Register weapon for weapon change events
        GameManager.Notifications.AddListener(this, "WeaponChange");
    }

    void Update() {

        if (!IsEquipped)
            return;

        if (!GameManager.Instance.InputAllowed)
            return;

        if (Input.GetButton ("Fire1") && CanFire)
            StartCoroutine (Fire ());
    }

    public virtual IEnumerator Fire() {
        yield break;
    }

    public void SpriteAnimationStopped() {

        // If not equipped, exit
        if (!IsEquipped)
            return;

        // Show default sprite
        DefaultSprite.enabled = true;
    }

    public bool Equip(WEAPON_TYPE WeaponType) {

        // If wrong type, exit and don't equip
        if((WeaponType != Type) || (!Collected) || (Ammo == 0) || (IsEquipped)) return false;

        IsEquipped = true;

        // Show default sprite
        DefaultSprite.enabled = true;

        CanFire = true;

        // Send weapon change event
        GameManager.Notifications.PostNotification (this, "WeaponChange");

        // Weapon was equipped
        return true;
    }

    public void WeaponChange(Component Sender) {

        if (Sender.GetInstanceID () == GetInstanceID ())
            return;

        // Has changed to other weapon, hide this one
        StopAllCoroutines();

        gameObject.SendMessage ("StopSpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

        // Deactivate equipped
        IsEquipped = false;

        foreach (SpriteRenderer SR in WeaponSprites)
            SR.enabled = false;
    }
}

