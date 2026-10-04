using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueReplica
{
    public Character character;
	[TextArea(3, 10)] public string[] sentences;

    public bool IsRyker() {
        return character == Character.Ryker;
    }
}
