using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQueue : TwitchChatListenerBase {
    public class PlayerQueueResponse {
        public static string ValidJoin = "@{0} has joined the fray!";
        public static string AlreadyJoin = "@{0}, you are already registered in queue.";
    }
	private LinkedList<PlayerCreationData> _waitingQueue = new LinkedList<PlayerCreationData>();
	private Dictionary<string, LinkedListNode<PlayerCreationData>> _inQueue = new Dictionary<string, LinkedListNode<PlayerCreationData>>(); 

	public bool Enqueue(string username) {
		if(_inQueue.ContainsKey(username)) {
			return false;
		} else {
            List<Job> mookJobs = GameManager.Instance.models.getMookJobs();
            Job mookJob = mookJobs[UnityEngine.Random.Range(0, mookJobs.Count)];

			LinkedListNode<PlayerCreationData> node = _waitingQueue.AddLast(new PlayerCreationData(username, mookJob));
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

	public List<PlayerCreationData> Peek(int n) {
		List<PlayerCreationData> peek = new List<PlayerCreationData>();

		foreach(PlayerCreationData data in _waitingQueue) {
			if(n <= 0) {
				break;
			}

			peek.Add(data);
			n--;
		}

		return peek;
	}

	public override void OnMessageReceived(string username, string message) {
		
	}

	public override void OnCommandReceived(string username, string message) {
		switch(message) {
			case "join":
                this.PlayerJoin(username);
				break;
			default:
				// Debug.Log("Unknown command: " + message);
				break;
		}
	}

    public void PlayerJoin(string username) {
        if(Enqueue(username)) {
            this.EchoMessage(String.Format(PlayerQueueResponse.ValidJoin, username));
            Messenger.Broadcast(Messages.OnPlayerJoinQueue, username);
        } else {
            this.EchoMessage(String.Format(PlayerQueueResponse.AlreadyJoin, username));
        }
    }
}