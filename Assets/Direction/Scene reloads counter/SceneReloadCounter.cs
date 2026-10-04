using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloadCounter : MonoBehaviour
{
    private static bool isThisSceneReloaded = false;
    public bool getIsThisSceneReloaded() {
        return isThisSceneReloaded;
    }
    
    private int reloadCount = 0;    // int cause it's being used as index in DialogueTrigger.cs
    public int getReloadCount() {
        return reloadCount;
    }
    
    private static bool newControlsInThisSceneAlreadyShown = false;
    public bool SetNewControlsAlreadyShown(bool value) {
        return newControlsInThisSceneAlreadyShown = value;
    }
    public bool NewControlsAlreadyShown() {
        return newControlsInThisSceneAlreadyShown;
    }

    private static string prevSceneName;



    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        prevSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(prevSceneName == scene.name) {
            isThisSceneReloaded = true;
            reloadCount++;
        } else {
            isThisSceneReloaded = false;
            prevSceneName = scene.name;
            reloadCount = 0;
            newControlsInThisSceneAlreadyShown = false;
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
