using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkalatorPlatforms : MonoBehaviour
{
    private static List<GameObject> listOfAll = new List<GameObject>();

    public void Awake()
    {
        foreach (Transform platform in transform)
        {
            if (platform.CompareTag("WalkalatorPlatform"))
            {
                listOfAll.Add(platform.gameObject);
            }
        }
    }

    public void ActivateWalkalatorPlatforms()
    {
        foreach (GameObject platform in listOfAll)
        {
            platform.GetComponent<Platform>().SetActiveColor();
        }
    }

    public void DeactivateWalkalatorPlatforms()
    {
        foreach (GameObject platform in listOfAll)
        {
            platform.GetComponent<Platform>().SetInactiveColor();
        }
    }
}
