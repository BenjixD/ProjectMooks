using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQueue : ListenerImpl {
	LinkedList<Player> _waitingQueue = new LinkedList<Player>();
	Dictionary<string, LinkedListNode<Player>> _inQueue = new Dictionary<string, LinkedListNode<Player>>(); 

	public bool Enqueue(string username) {
		if(_inQueue.Contains(username)) {
			return false;
		} else {
			ListedListNode<Player> node = _waitingQueue.AddLast(new Player(username));
			_inQueue.Add(username, node);
			return true;
		}
	}

	public Player Dequeue() {
		LinkedListNode<Player> node = _waitingQueue.First();
		if(node != null) {
			_inQueue.Remove(node.Value.Name);
			_waitingQueue.RemoveFirst();
			return node.Value;
		} else {
			return null;
		}
	}

	public void Remove(string username) {
		LinkedListNode<Player> node = _inQueue[username];
		_waitingQueue.Remove(node);
		_inQueue.Remove(username);
	}

	void OnCommandRecieved(string username, string message) {
		switch(message) {
			case "join":
				enqueue(username);
				break;
			case "default":
				Debug.Log("Unknown command: " + message);
		}
	}
}