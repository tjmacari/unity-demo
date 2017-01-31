using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {


    // ENEMY PROPERTIES

    public enum ENEMY_TYPE {ZombieGreen = 0, ZombieBlue = 1};

    // Custom ID of this enemy
    public int EnemyID = 0;

    public int Health = 100;

    public int AttackDamage = 10;

    public float RecoveryDelay = 1.0f;

    // Enemy Cached transform
    protected Transform ThisTransform = null;


    // AI PROPERTIES

    // Reference to NavMesh agent component
    protected NavMeshAgent Agent = null;

    // Reference to active player controller
    protected PlayerController PC = null;

    // Reference to player transform
    protected Transform PlayerTransform = null;

    // Unity units distance before enemy changes
    public float PatrolDistance = 10.0f;

    public float AttackDistance = 1.0f;

    // Different enum states
    public enum ENEMY_STATE {PATROL = 0, ATTACK = 1, DEATH = 2};

    // Current enemy state
    public ENEMY_STATE ActiveState = ENEMY_STATE.PATROL;

    // Sprites for walk anim
    public SpriteRenderer[] WalkSprites = null;

    public SpriteRenderer[] AttackSprites = null;

    public SpriteRenderer[] DeathSprites = null;

    public SpriteRenderer DefaultSprite = null;

    protected virtual void Start() {

        Agent = GetComponent<NavMeshAgent> ();

        // Get player controller
        GameObject PlayerObject = GameObject.Find ("Player");
        PC = PlayerObject.GetComponentInChildren<PlayerController> ();

        // Get player transform
        PlayerTransform = PC.transform;

        // Get enemy transform
        ThisTransform = transform;

        // Set default state
        ChangeState(ActiveState);
    }

    public void ChangeState(ENEMY_STATE State) {

        StopAllCoroutines ();

        ActiveState = State;

        switch (ActiveState) {

        case ENEMY_STATE.ATTACK:
            StartCoroutine (AI_Attack ());

            SendMessage ("Attack", SendMessageOptions.DontRequireReceiver);
            return;

        case ENEMY_STATE.PATROL:
            StartCoroutine (AI_Patrol ());
            SendMessage ("Patrol", SendMessageOptions.DontRequireReceiver);
            return;
        }
    }

    private IEnumerator AI_Patrol() {

        Agent.Stop ();

        while (ActiveState == ENEMY_STATE.PATROL) {

            // Get random destination on map
            Vector3 randomPosition = Random.insideUnitSphere * PatrolDistance;

            randomPosition += ThisTransform.position;

            // Get nearest valid position
            NavMeshHit hit;
            NavMesh.SamplePosition (randomPosition, out hit, PatrolDistance, 1);

            // Set destination
            Agent.SetDestination(hit.position);

            // Set distance range between object and destination to classify as 'arrived'
            float ArrivalDistance = 2.0f;

            // Set timeout before new path is generated (5 secs)
            float TimeOut = 5.0f;

            // Elapsed time
            float ElapsedTime = 0;

            // Wait until next frame
            yield return null;
        }
    }

    private IEnumerator AI_Attack() {

        Agent.Stop ();

        // Elapsed time - to calculate strike intervals
        float ElapsedTime = RecoveryDelay;

        while (ActiveState == ENEMY_STATE.ATTACK) {

            ElapsedTime += Time.deltaTime;

            float DistanceFromPlayer = Vector3.Distance (ThisTransform.position, PlayerTransform.position);

            if (DistanceFromPlayer > AttackDistance) {
                ChangeState (ENEMY_STATE.PATROL);
                yield break;
            }

            // Make strike
            if (ElapsedTime >= RecoveryDelay) {

                ElapsedTime = 0;
                SendMessage ("Strike", SendMessageOptions.DontRequireReceiver);
            }

            // Wait until next frame
            yield return null;
        }
    }

    public IEnumerator Damage(int Damage = 0) {

        Health -= Damage;

        // Play damage animation
        gameObject.SendMessage("PlayColorAnimation", 0, SendMessageOptions.DontRequireReceiver);

        if (Health <= 0) {

            gameObject.SendMessage ("StopSpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

            // Tot time in secs to transition from source to dest
            float TransitionTime = 5.0f;

            // Maintain elapsed time
            float ElapsedTime = 0.0f;

            Death ();

            // Loop for transition time
            while (ElapsedTime <= TransitionTime) {

                // Update Elapsed Time
                ElapsedTime += Time.deltaTime;

                if (ElapsedTime >= 4.9f) {

                    // Send enemy destroyed notif
                    GameManager.Notifications.PostNotification(this, "EnemyDestroyed");

                    DestroyImmediate(gameObject);

                    // Clean up old listeners
                    GameManager.Notifications.RemoveRedundancies();
                }

                // Wait until next frame
                yield return null;
            }
        }
    }

    public void Patrol() {

        Debug.Log ("Patrol state");

        // Hide default and attack sprites
        foreach(SpriteRenderer SR in AttackSprites) SR.enabled = false;
        foreach(SpriteRenderer SR in DeathSprites) SR.enabled = false;

        // Hide default sprite
        DefaultSprite.enabled = false;

        // Entered patrol state
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.PATROL), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.ATTACK), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.DEATH), SendMessageOptions.DontRequireReceiver);
        SendMessage("PlaySpriteAnimation", ((int) ENEMY_STATE.PATROL), SendMessageOptions.DontRequireReceiver);
    }

    public void Attack() {

        Debug.Log ("Attack state");

        foreach (SpriteRenderer SR in WalkSprites) SR.enabled = false;
        foreach(SpriteRenderer SR in DeathSprites) SR.enabled = false;

        // Hide default sprite
        DefaultSprite.enabled = false;

        // Entered attack state
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.PATROL), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.ATTACK), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.DEATH), SendMessageOptions.DontRequireReceiver);
        SendMessage("PlaySpriteAnimation", ((int) ENEMY_STATE.ATTACK), SendMessageOptions.DontRequireReceiver);
    }

    public void Death() {

        Debug.Log ("Death state");

        foreach (SpriteRenderer SR in WalkSprites) SR.enabled = false;
        foreach(SpriteRenderer SR in AttackSprites) SR.enabled = false;

        // Hide default sprite
        DefaultSprite.enabled = false;

        // Entered attack state
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.DEATH), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.PATROL), SendMessageOptions.DontRequireReceiver);
        SendMessage("StopSpriteAnimation", ((int) ENEMY_STATE.ATTACK), SendMessageOptions.DontRequireReceiver);
        SendMessage("PlaySpriteAnimation", ((int) ENEMY_STATE.DEATH), SendMessageOptions.DontRequireReceiver);
    }

    public void Strike() {

        // Damage player
        PC.gameObject.SendMessage("ApplyDamage", AttackDamage, SendMessageOptions.DontRequireReceiver);
    }

}
