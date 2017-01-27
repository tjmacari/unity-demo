using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int Health {

        get{ return iHealth; }

        set {
            // Update health
            iHealth = value;

            if (iHealth <= 0)
                Die ();
        }
    }

    private int iHealth = 100;

    public void Die() {
        // TODO: Kill our hero
    }
}
