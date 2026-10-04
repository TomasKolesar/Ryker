using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBar : MonoBehaviour
{
    // [SerializeField] private GameObject dialogueName;
    private TMP_Text nameText;
    // [SerializeField] private GameObject dialogueText;
    private TMP_Text dialogueText;

    public TMP_Text GetNameText() {
        return nameText;
    }

    public TMP_Text GetDialogueText() {
        return dialogueText;
    }

    // Start is called before the first frame update
    void Awake()
    {
        nameText = transform.Find("Dialogue_Name").GetComponent<TMP_Text>();
        dialogueText = transform.Find("Dialogue_Text").GetComponent<TMP_Text>();
    }
}
