using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Variables

    // King of all
    private PlayerInput playerInput;

    // States of play
    private bool playingForRyker = true;
    private bool playingForAngel = false; 
    private bool angelInSlowmo = false;
    private bool playingDialogue = false;
    private bool showingNewControlsTutorial = false;
    private bool dead =  false;
    private bool win = false;

    // Those who require actions triggered by player input
    private Ryker ryker;
    private Angel angel;
    private DialogueManager dialogueManager;
    private NewControlsTutorial newControlsTutorial;



    // Setters

    public void SetPlayingForRyker() {
        playingForRyker = true;
        playingForAngel = false;
        playingDialogue = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("Character");
    }

    public void SetAngel(Angel a) {
        angel = a;
        playingForAngel = true;
        playingForRyker = false;
        playingDialogue = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("Character");
        // BUGfix: Ryker keeps the last movement value before instantiating angel, then after angel dissolves that vlue moves him
        ryker.GetComponent<Movement>().Move(Vector2.zero);
    }
    public void SetAngelInSlowmo(bool b) {
        angelInSlowmo = b;
    }
    public bool IsAngelInSlowmo() {
        return angelInSlowmo;
    }
    public void DestroyAngelReference() {
        angel = null;
        playingForAngel = false;
        playingForRyker = true;
    }

    public void SetPlayingDialogue() {
        playingDialogue = true;
        playingForRyker = false;
        playingForAngel = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SetShowingNewControlsTutorial() {
        showingNewControlsTutorial = true;
        playingForRyker = false;
        playingForAngel = false;
        playingDialogue = false;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SetDead() {
        dead = true;
        playingForRyker = false;
        playingForAngel = false;
        playingDialogue = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SetWin() {
        win = true;
        playingForRyker = true;
        playingForAngel = false;
        playingDialogue = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("Character");
    }

    public void EnteringElevator() {
        playingForRyker = false;
        playingForAngel = false;
        playingDialogue = false;
        showingNewControlsTutorial = false;
        playerInput.SwitchCurrentActionMap("UI");
    }

    // Getters
    public bool IsPlayingForRyker() {
        return playingForRyker;
    }

    public bool IsPlayingForAngel() {
        return playingForAngel;
    }



    //
    // AWAKE & UPDATE
    //

    private void Awake() {
        ryker = FindObjectOfType<Ryker>();
        playerInput = GetComponent<PlayerInput>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        newControlsTutorial = FindObjectOfType<NewControlsTutorial>();
    }

    private void Update() {
        // RYKER
        if (playingForRyker && !win) {
            // To hide Ryker's init dash direction arrow
            if (playerInput.actions["MouseRight"].WasReleasedThisFrame() && ryker.IsAimingDash()) {
                    ryker.HideDashDirection();
            }
        }

        // ANGEL
        if (playingForAngel) {
            // To hide Angel's dash direction arrow
            if (playerInput.actions["MouseRight"].WasReleasedThisFrame() && !angel.IsAimingShift()) {
                angel.EndSlowmoByInput();
                SetAngelInSlowmo(false);
            }
            if (playerInput.actions["ShiftAim"].WasReleasedThisFrame() && !angel.IsAimingDash()) {
                angel.HideShiftIndicatorsByInput();
            }
        }
    }


    // CHARACTER (actionmap)

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        if (playingForRyker) {
            ryker.GetComponent<Movement>().Move(movement);
        } else if (playingForAngel) {
            angel.GetComponent<Movement>().Move(movement);
        }
    }

    public void OnShiftAim(InputValue value)
    {
        if (playingForAngel && !win) {
            angel.AimShift();
        }
    }


    public void OnMouseRight(InputValue value)
    {
        if (playingForRyker && !win) {
            ryker.ShowDashDirection();
        }else if (playingForAngel) {
            angel.MouseRight();
        }
    }

    public void OnMouseLeft(InputValue value)
    {
        if (playingForRyker && !win) {
            ryker.InitDash();
        }else if (playingForAngel) {
            angel.MouseLeft();
        }
    }


    // UI (actionmap)

    void OnNextDialogueSentence(InputValue input)
    {
        if (playingDialogue) {
            dialogueManager.NextDialogueSentence();
        }
    }

    void OnHideControlsTutorial(InputValue input) 
    {
        if (showingNewControlsTutorial && newControlsTutorial.CanHide()) {
            newControlsTutorial.HideControlsTutorial();
        }
    }

    void OnReload(InputValue input)
    { 
        if (dead) {
            ryker.Reload();
        }
    }
}
