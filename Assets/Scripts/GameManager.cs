// The Game Manager - the general, managerial and overarching class for our (and any) game
using UnityEngine;
using System.Collections;

// Requires our NoticationManager for sending/receiving notifications
// inserted to designate dependence/relationship with NotifManager
// GameManager's host 'GameObject' will also need NotifManager component
[RequireComponent (typeof(NotificationManager))] 

public class GameManager : MonoBehaviour {

    // C# prop to grab the currently active instance of object (if there are any)
    public static GameManager Instance {

        get {

            // If needed, create new game manager object
            if (instance == null) {
                instance = new GameObject ("GameManager").AddComponent<GameManager> ();
            }
            return instance;
        }
    }

    // C# property to retrieve notifications manager
    public static NotificationManager Notifications {

        get {
            if (notifications == null)
                notifications = instance.GetComponent<NotificationManager>();
            return notifications;
        }
    }

    public bool InputAllowed {

        get{ return bInputAllowed; }

        set {
            bInputAllowed = value;
            Notifications.PostNotification (this, "InputChanged");
        }
    }

    // Since we are using this as as singleton, create internal private reference
    private static GameManager instance = null;

    // Internal reference to our notifications manager
    private static NotificationManager notifications = null;

    private bool bInputAllowed = true;

    // Awake is, of course, called before Start on object creation
    void Awake() {

        // Check if there is an existing instance of this object
        if ((instance) && (instance.GetInstanceID() != GetInstanceID())) {
            DestroyImmediate(gameObject); // delete dup
        } else {
            instance = this; // Make this the only instance of our singleton
            DontDestroyOnLoad(gameObject); // Set as do not destroy
        }
    }
}
