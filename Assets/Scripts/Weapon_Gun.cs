using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Gun : Weapon {

    public override IEnumerator Fire() {

        // DIFFERENT**
        if (!CanFire || !IsEquipped || Ammo <= 0)
            yield break;

        CanFire = false;

        gameObject.SendMessage ("PlaySpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

        // Play collect sound, if audio source available *DIFFERENT* moved here from line 73
        if(SFX) {SFX.PlayOneShot(WeaponAudio, 1.0f);}

        // Calc hit

        // Get ray from screen center target
        Ray R = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));

        // Test for collision
        RaycastHit hit;
        if (Physics.Raycast (R.origin, R.direction, out hit, Range)) {

            // Target hit - check if target is enemy
            if (hit.collider.gameObject.CompareTag ("enemy")) {

                Debug.Log ("Enemy hit!");

                hit.collider.gameObject.SendMessage ("Damage", Damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Reduce ammo *DIFFERENT*
        --Ammo;

        // *DIFFERENT*
        if (Ammo <= 0)
            GameManager.Notifications.PostNotification (this, "AmmoExpired");

        // Wait for refire enabling
        yield return new WaitForSeconds (RecoveryDelay);

        // Inherited from abstract parent
        CanFire = true;

        yield break;
    }
}
