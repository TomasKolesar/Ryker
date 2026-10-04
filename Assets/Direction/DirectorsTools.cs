using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorsTools : MonoBehaviour
{
    [SerializeField] private GameObject rykerShiftBounds;
    [SerializeField] private GameObject dialogueManager;

    public GameObject GetRykerShiftBounds() {return rykerShiftBounds;}
    public GameObject GetDialogueManager() {return dialogueManager;}
}
