using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Just for development phase")]
    [SerializeField] private bool doNotTrigger = false;

    [Header("Sentences")]
    [SerializeField] private Dialogue[] dialogues;
    private Dialogue dialogueForThisNthReloadOfScene;
    private Dialogue dialogueAfterWin;

    private PlayerControls playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerControls>();

        if(doNotTrigger) {
            FindObjectOfType<DialogueBar>().gameObject.SetActive(false);
            return;
        }

        int reloadCount = FindObjectOfType<SceneReloadCounter>().getReloadCount();
        foreach (Dialogue d in dialogues) {
            if (d.playAfterSceneReloadCount == reloadCount) {
                dialogueForThisNthReloadOfScene = d;
            } else if (d.playAfterWin) {
                dialogueAfterWin = d;
                FindObjectOfType<Ryker>().SetIsDialogueAfterWin(true);
            }
        }

        if(dialogueForThisNthReloadOfScene == null) {
            FindObjectOfType<DialogueBar>().gameObject.SetActive(false);
        } else {
            playerInput.SetPlayingDialogue();
            FindObjectOfType<DialogueManager>().PlayDialogue(dialogueForThisNthReloadOfScene);
        }
    }

    public void PlayDialogueAfterWin() {
        playerInput.SetPlayingDialogue();
        FindObjectOfType<DialogueManager>().ShowDialogueBar();
        FindObjectOfType<DialogueManager>().PlayDialogue(dialogueAfterWin);
    }
}
