using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

	public float playAfterSceneReloadCount = 0f;
	public bool playAfterWin = false;
	public DialogueReplica[] replicas;

}