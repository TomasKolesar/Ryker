using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashIndicators : MonoBehaviour
{
    //
    private GameObject baseDashIndicator;
    //
    private List<GameObject> storedDashesIndicators;
    //
    private GameObject dashDirectionSmall;
    private GameObject dashDirectionBig;
    //
    private GameObject slowMoStamina;
    //
    private Material slowMoHarmMaterial;


    //
    // 
    private void Awake() {
        storedDashesIndicators = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.tag == "StackedDashIndicator")
            {
                storedDashesIndicators.Add(child.gameObject);
            }else if(child.tag == "AngelDashDirection")
            {
                dashDirectionSmall = child.gameObject.transform.GetChild(0).gameObject;
                dashDirectionBig = child.gameObject.transform.GetChild(1).gameObject;
            }else if (child.tag == "SlowMoStamina")
            {
                slowMoStamina = child.gameObject;
                slowMoHarmMaterial = slowMoStamina.GetComponent<SpriteRenderer>().material;
            }
        }
    }

    //
    // Hides small arrow and shows big arrow
    public void DrawOutDashDirection()
    {
        dashDirectionSmall.gameObject.SetActive(false);
        dashDirectionBig.gameObject.SetActive(true);
    }

    // Hides big arrow and shows small arrow
    public void DrawInDashDirection()
    {
        dashDirectionSmall.gameObject.SetActive(true);
        dashDirectionBig.gameObject.SetActive(false);
    }

    // Hide both in case of player lost
    public void HideDashDirection()
    {
        dashDirectionSmall.gameObject.SetActive(false);
        dashDirectionBig.gameObject.SetActive(false);
    }

    //
    // Shows base dash indicator
    public void IndicateBaseDashReady()
    {
        dashDirectionSmall.GetComponent<SpriteRenderer>().color = Color.red;
        foreach (Transform child in dashDirectionBig.transform)
        {
            child.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    // Hides base dash indicator
    public void IndicateBaseDashReadyNot()
    {
        dashDirectionSmall.GetComponent<SpriteRenderer>().color = Color.white;
        foreach (Transform child in dashDirectionBig.transform)
        {
            child.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //
    // Show next stored dash indicator by enabling its sprite renderer
    public void ShowNewStoredDashIndicator()
    {
        foreach (GameObject indicator in storedDashesIndicators)
        {
            if (!indicator.GetComponent<SpriteRenderer>().enabled)
            {
                indicator.GetComponent<SpriteRenderer>().enabled = true;
                break;
            }
        }
    }
    
    // Hides the last enabled stored dash indicator
    public void HideLastStoredDashIndicator()
    {
        for (int i = storedDashesIndicators.Count - 1; i >= 0; i--)
        {
            if (storedDashesIndicators[i].GetComponent<SpriteRenderer>().enabled)
            {
                storedDashesIndicators[i].GetComponent<SpriteRenderer>().enabled = false;
                break;
            }
        }
    }

    // Hides all dash indicators for stored dashes
    public void HideAllStoredDashIndicators()
    {
        foreach (GameObject indicator in storedDashesIndicators)
        {
            indicator.SetActive(false);
        }
    }

    public void ShowAllStoredDashIndicators()
    {
        foreach (GameObject indicator in storedDashesIndicators)
        {
            indicator.SetActive(true);
        }
    }

    //
    // Sets the value of harm caused from using slowmotion to a certain value between 0 and 1
    public void SetSlowMoHarmValue(float value)
    {
        if (value >= 0f && value <= 1f)
        {
            slowMoHarmMaterial.SetFloat("_FillRate", value);
        }
    }

    //
    // Changes color of slowmo harm to darker red
    public void StartRestoringSlowMoLimit()
    {
        slowMoHarmMaterial.SetFloat("_IsRestoring", 0f);
    }

    // Changes color of slowmo harm to normal red
    public void StopRestoringSlowMoLimit()
    {
        slowMoHarmMaterial.SetFloat("_IsRestoring", 1f);
    }

    //
    // Rotates all indicators (direction, storedDashesIndicators) towards the mouse position around Angel's body
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector3 direction = (worldPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
}
