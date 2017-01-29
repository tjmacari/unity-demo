using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Punch : Weapon {

    public override IEnumerator Fire() {

        if (!CanFire || !IsEquipped)
            yield break;

        CanFire = false;

        gameObject.SendMessage ("PlaySpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

        if (SFX) {
            SFX.PlayOneShot (WeaponAudio, 1.0f);
        }

        // Calc hit

        // Get ray from screen center target
        Ray R = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));

        // Test for collision
        RaycastHit hit;

        if (Physics.Raycast (R.origin, R.direction, out hit, Range)) {

            // Target hit - check if target is enemy
            if (hit.collider.gameObject.CompareTag ("enemy")) {

                hit.collider.gameObject.SendMessage ("Damange", Damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Wait for refire enabling
        yield return new WaitForSeconds (RecoveryDelay);

        // Inherited from abstract parent
        CanFire = true;

        yield break;
    }
}
