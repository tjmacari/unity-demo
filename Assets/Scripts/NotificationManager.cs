// EVENTS MANAGER CLASS - for receiving notifications and notifying registered listeners

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for dictionary and list

public class NotificationManager: MonoBehaviour {

	// Private vars

	// Internal reference to all listeners for notifications
	private Dictionary<string, List<Component>> Listeners = new Dictionary<string, List<Component>>();


	// Methods

	// Add listener for notification to listeners list
	public void AddListener(Component Sender, string NotificationName) {

		// Add listener to dictionary if doesn't already exist
		if (!Listeners.ContainsKey(NotificationName)) {
			Listeners.Add(NotificationName, new List<Component>());
		}

		// Add object to listener list for this notification
        Listeners[NotificationName].Add(Sender);
	}

	// Remove notification listener
	public void RemoveListener(Component Sender, string NotificationName) {

		// If no key in dictionary exists, exit
		if (!Listeners.ContainsKey(NotificationName)) {
			return;
		}

		// Cycle through listeners, identify components, then remove
        for(int i = 0; i < Listeners[NotificationName].Count; i++) {

			// Check ID, remove at index
			if(Listeners[NotificationName][i].GetInstanceID() == Sender.GetInstanceID()) {
				Listeners[NotificationName].RemoveAt(i); // Matched, remove at index
			}
		}
	}

	// Post notification to listener
	public void PostNotification(Component Sender, string NotificationName) {

        Debug.Log ("Notification: " + Sender + " - " + NotificationName);

		// Exit if no matching key in dictionary
		if(!Listeners.ContainsKey(NotificationName)) return;

		// Otherwise, loop through each listener and post notification if matches
		foreach (Component Listener in Listeners[NotificationName]) {
			Listener.SendMessage(NotificationName, Sender, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Clear all listeners
	public void ClearListeners() {
		Listeners.Clear();
	}

	// Remove any redundant listeners
	public void RemoveRedundancies() {

		// Create new dictionary
		Dictionary<string, List<Component>> TmpListeners = new Dictionary<string, List<Component>>();

        // Cycle through all dictionary entries
		foreach(KeyValuePair<string, List<Component>> Item in Listeners) {

			// Cycle through all listener objects in list, remove null objects
            for(int i = 0; i < Item.Value.Count; i++) {
				if (Item.Value[i] == null) {
					Item.Value.RemoveAt(i);
				}
			}

            // If items remain in list for this notification, then add to temp dictionary
            if (Item.Value.Count > 0)
                TmpListeners.Add (Item.Key, Item.Value);
		}

        // Replace listeners object with our new optimized dicitonary
        Listeners = TmpListeners;
	}
}
