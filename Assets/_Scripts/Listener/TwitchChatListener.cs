using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TwitchChatListener
{
   void OnMessageReceived(string username, string message);
   void OnCommandReceived(string username, string message);
}

