using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    // Enum of enemy states
    public enum ENEMY_STATE {PATROL = 0, CHASE = 1, ATTACK = 2};

    // Current state
    public ENEMY_STATE ActiveState = ENEMY_STATE.PATROL;

    // Current health
    public int Health = 100;

    
}
