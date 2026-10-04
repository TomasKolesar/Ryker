using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewControlsTutorial : MonoBehaviour
{
    [SerializeField] private bool isSthToBeShown = false;
    [SerializeField] private float timeBeforeEscShown = 1.25f;
    [SerializeField] private GameObject pressEscToClose;
    [SerializeField] private GameObject toBeShown;

    private PlayerControls playerInput;

    public bool CanHide() {
        return pressEscToClose.activeSelf;
    }



    private void Awake() {
        playerInput = FindObjectOfType<PlayerControls>();
    }

    public bool IsSthToBeShown() {
        return isSthToBeShown;
    }

    public void ShowControlScheme() {
        toBeShown.SetActive(true);
        StartCoroutine(ShowEsc());
    }

    private IEnumerator ShowEsc() {
        yield return new WaitForSeconds(timeBeforeEscShown);
        pressEscToClose.SetActive(true);
    }
    
    public void HideControlsTutorial() {
        if (pressEscToClose.activeSelf) {
            toBeShown.SetActive(false);
            pressEscToClose.SetActive(false);
            isSthToBeShown = false;
        }
        playerInput.SetPlayingForRyker();
    }
}
