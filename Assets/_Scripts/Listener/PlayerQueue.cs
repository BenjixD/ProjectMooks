using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQueue : TwitchChatListenerBase {
	[SerializeField]
	private PlayerStats _template;

	private LinkedList<PlayerCreationData> _waitingQueue = new LinkedList<PlayerCreationData>();
	private Dictionary<string, LinkedListNode<PlayerCreationData>> _inQueue = new Dictionary<string, LinkedListNode<PlayerCreationData>>(); 

	public bool Enqueue(string username) {
		if(_inQueue.ContainsKey(username)) {
			return false;
		} else {
			LinkedListNode<PlayerCreationData> node = _waitingQueue.AddLast(new PlayerCreationData(username, _template));
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
		switch(message) {
			case "join":
				Enqueue(username);
				break;
			default:
				Debug.Log("Unknown command: " + message);
				break;
		}
	}
}