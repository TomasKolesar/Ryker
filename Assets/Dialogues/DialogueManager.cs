using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour {
	[SerializeField] private float charTypingSpeed = 0.02f;

	[Header("Aligned character names to datatypes in DialogueReplica")]
	[SerializeField] private List<CharacterDataName> characterDataNames;

	[Header("Colours for characters")]
    [SerializeField] private List<CharacterColor> characterColors;

	// For later instantiating and being able to have dialogues when not starting from level 1
	[Header("Just for development phase")]
	[SerializeField] private GameObject sceneReloadCounterPrefab;
	private SceneReloadCounter sceneReloadCounter;

	private Dialogue[] dialogues;
	private GameObject dialogueBar;
	private TMP_Text nameText;
	private TMP_Text dialogueText;

	private Dialogue currentDialogue;

	private Queue<string> currentSentences;

	private int replicaIndex = 0;

	public void HideDialogueBar() {
		dialogueBar.SetActive(false);
	}
	public void ShowDialogueBar() {
		dialogueBar.SetActive(true);
	}

	private PlayerControls playerInput;


	// Use this for initialization
	void Awake () {
		playerInput = FindObjectOfType<PlayerControls>();

		currentSentences = new Queue<string>();
		dialogueBar = FindObjectOfType<DialogueBar>().gameObject;

		// If not playing from 1st level (durign development) then instantiate the SceneReloadCounter
        if(FindObjectOfType<SceneReloadCounter>() == null) {
            sceneReloadCounter = Instantiate(sceneReloadCounterPrefab, Vector3.zero, Quaternion.identity).GetComponent<SceneReloadCounter>();
        }else {
			sceneReloadCounter = FindObjectOfType<SceneReloadCounter>();
		}
	}

	public void PlayDialogue(Dialogue d) {
		currentDialogue = d;
		currentSentences.Clear();
		replicaIndex = 0;
	
		// Get references to later display names and lines
		if (nameText == null) {
			// This fixes name not being show for first replica
			nameText = dialogueBar.GetComponent<DialogueBar>().GetNameText();
			// // Align name and color
			// nameText.color = characterColors.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).color;
			// nameText.text = characterDataNames.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).name;
		}
		if (dialogueText == null) dialogueText = dialogueBar.GetComponent<DialogueBar>().GetDialogueText();

		LoadNextReplicaSentences();

		DisplayNextSentence();
	}

	// Triggered by playerInput
	public void NextDialogueSentence() {
		if (replicaIndex >= (currentDialogue.replicas.Length - 1)) {
			dialogueBar.SetActive(false);
			NewControlsTutorial controlsTutorial = FindObjectOfType<NewControlsTutorial>();
			if (controlsTutorial.IsSthToBeShown() && !sceneReloadCounter.NewControlsAlreadyShown()) {
				playerInput.SetShowingNewControlsTutorial();
				controlsTutorial.ShowControlScheme();
				sceneReloadCounter.SetNewControlsAlreadyShown(true);
				return;
			}else {
				playerInput.SetPlayingForRyker();
			}
			if (FindObjectOfType<Angel>() != null) FindObjectOfType<Angel>().UnfreezeDashAndLastSBDissolvement();
			return;
		}
		DisplayNextSentence();
	}

	private void LoadNextReplicaSentences() {
		currentSentences.Clear();
		foreach (string sentence in currentDialogue.replicas[replicaIndex].sentences) {
			currentSentences.Enqueue(sentence);
		}
		nameText.color = characterColors.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).color;
		nameText.text = characterDataNames.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).name;

	}

	public void DisplayNextSentence ()
	{
		if (currentSentences.Count == 0) {
			//
			replicaIndex++;
			//
			// nameText.color = characterColors.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).color;
			// nameText.text = characterDataNames.Find(x => x.character == currentDialogue.replicas[replicaIndex].character).name;

			LoadNextReplicaSentences();
		}
		string sentence = currentSentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(charTypingSpeed);
		}
	}
}