using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPlatforms : MonoBehaviour
{
    private static List<GameObject> listOfAll = new List<GameObject>();

    public void Awake()
    {
        listOfAll.Clear();
        foreach (Transform platform in transform)
        {
            if (platform.CompareTag("BoostPlatform"))
            {
                listOfAll.Add(platform.gameObject);
            }
        }
    }

    // FORMER CALL by event from Ryker.cs when player returned from Angel to Ryker
    public void ActivateBoostPlatforms()
    {
        foreach (GameObject platform in listOfAll)
        {
            platform.GetComponent<Platform>().SetActiveColor();
        }
    }

    public void DeactivateBoostPlatforms()
    {
        foreach (GameObject platform in listOfAll)
        {
            platform.GetComponent<Platform>().SetInactiveColor();
        }
    }
}
