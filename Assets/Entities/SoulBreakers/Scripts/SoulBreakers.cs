using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulBreakers : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private List<GameObject> allSoulBreakers = new List<GameObject>();
    private List<GameObject> allFollowingSoulBreakers = new List<GameObject>();
    private List<GameObject> allCoveringSoulBreakers = new List<GameObject>();

    // Just for development
    [Header("Development")]
    public bool letAngelContinueWhenThereAreNoSBs = false;

    private GameObject lastSBAlive = null;

    public List<GameObject> GetAllSBs() {
        return allSoulBreakers;
    }

    public GameObject GetLastSBReference() {
        return lastSBAlive;
    }

    private void Start() {
        foreach (Transform child in transform) {
            allSoulBreakers.Add(child.gameObject);
            if (child.gameObject.GetComponent<FollowingSoulBreaker>()) {
                allFollowingSoulBreakers.Add(child.gameObject);
                child.gameObject.GetComponent<FollowingSoulBreaker>().SetTarget(target);
            } else if (child.gameObject.GetComponent<CoveringSoulBreaker>() || child.gameObject.GetComponent<CoveringSoulBreakerCustomizable>()) {
                allCoveringSoulBreakers.Add(child.gameObject);
            }
        }
    }

    public void AllSoulBreakersSetTriggerTrue() {
        foreach (GameObject soulBreaker in allSoulBreakers) {
            soulBreaker.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    public void AllSoulBreakersSetTriggerFalse() {
        foreach (GameObject soulBreaker in allSoulBreakers) {
            soulBreaker.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }

    public void AllSoulBreakersActivate() {
        foreach (GameObject soulBreaker in allFollowingSoulBreakers) {
            soulBreaker.GetComponent<FollowingSoulBreaker>().Activate();
        }
    }

    public void AllSoulBreakersFire() {
        foreach (GameObject soulBreaker in allFollowingSoulBreakers) {
            soulBreaker.GetComponent<FollowingSoulBreaker>().ManualFire();
        }
        foreach (GameObject soulBreaker in allCoveringSoulBreakers) {
            if (soulBreaker.GetComponent<CoveringSoulBreaker>()) {
                soulBreaker.GetComponent<CoveringSoulBreaker>().StartFiring();
            } else if (soulBreaker.GetComponent<CoveringSoulBreakerCustomizable>()) {
                soulBreaker.GetComponent<CoveringSoulBreakerCustomizable>().StartFiring();
            }
        }
    }

    public void RemoveSoulBreaker(GameObject soulBreaker) {
        // if there's the last SB alive, keep a reference
        if (allSoulBreakers.Count == 1) {
            lastSBAlive = allSoulBreakers[0];
        }
        // remove from common list
        allSoulBreakers.Remove(soulBreaker);
        // remove from specific list
        if (soulBreaker.GetComponent<FollowingSoulBreaker>()) {
            allFollowingSoulBreakers.Remove(soulBreaker);
        }
        else if (soulBreaker.GetComponent<CoveringSoulBreaker>()) {
            allCoveringSoulBreakers.Remove(soulBreaker);
        }
        // Angel keeps track of which may be his final dash
        FindObjectOfType<Angel>().SBDestroyed(allSoulBreakers.Count);
    }

    // Called everytime when Angel makes a dash, while there's only 1 SB left
    // If dash destroys last SB, calls PlayerWon() in Ryker.cs
    public void LaunchPostWinPhase() {
        // Just for development
        if (letAngelContinueWhenThereAreNoSBs) { return; }

        if (allSoulBreakers.Count == 0 && target.GetComponent<Ryker>()) {
            target.GetComponent<Ryker>().PlayerWon();
        }
    }
}