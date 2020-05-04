using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQueue : TwitchChatListenerBase {
	LinkedList<PlayerCreationData> _waitingQueue = new LinkedList<PlayerCreationData>();
	Dictionary<string, LinkedListNode<PlayerCreationData>> _inQueue = new Dictionary<string, LinkedListNode<PlayerCreationData>>(); 

	public bool Enqueue(string username) {
		if(_inQueue.ContainsKey(username)) {
			return false;
		} else {
			LinkedListNode<PlayerCreationData> node = _waitingQueue.AddLast(new PlayerCreationData(username));
			_inQueue.Add(username, node);
			return true;
		}
	}

	public PlayerCreationData Dequeue() {
		LinkedListNode<PlayerCreationData> node = _waitingQueue.First;
		if(node != null) {
			_waitingQueue.RemoveFirst();
			return node.Value;
		} else {
			return null;
		}
	}

	public void Remove(string username) {
		_inQueue.Remove(username);
	}

	public void Dropout(string username) {
		LinkedListNode<PlayerCreationData> node = _inQueue[username];
		_waitingQueue.Remove(node);
		_inQueue.Remove(username);
	}

	public override void OnMessageReceived(string username, string message) {
		
	}

	public override void OnCommandReceived(string username, string message) {
		Debug.Log(message);
		switch(message) {
			case "join":
				if(Enqueue(username)) {
					Debug.Log(username + " has joined the queue!");
				} else {
					Debug.Log(username + " already exists");
				}
				break;
			default:
				Debug.Log("Unknown command: " + message);
				break;
		}
	}
}