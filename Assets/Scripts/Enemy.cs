using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int Health = 100;

    public IEnumerator Damage(int Damage = 0) {

        Health -= Damage;

        // Play damage animation
        gameObject.SendMessage("PlayColorAnimation", 0, SendMessageOptions.DontRequireReceiver);

        if (Health <= 0) {

            // Send enemy destroyed notif
            GameManager.Notifications.PostNotification(this, "EnemyDestroyed");

            // Show death animation
            gameObject.SendMessage ("StopSpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

            // After small delay, remove this enemy
            float DelayTime = 1.0f/8;
            int Frame = 0;
            do {
                yield return new WaitForSeconds (DelayTime);
                Frame++;
                if(Frame == 7) {
                    DestroyImmediate(gameObject);

                    // Clean up old listeners
                    GameManager.Notifications.RemoveRedundancies();
                }
            } while(Frame < 8);
        }
    }
}
