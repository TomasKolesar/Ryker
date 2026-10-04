using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRenewers : MonoBehaviour
{
    private List<DashRenewer> dashRenewers;

    public void populateDashRenewers()
    {
        dashRenewers = new List<DashRenewer>(Resources.FindObjectsOfTypeAll<DashRenewer>());
    }

    // Start is called before the first frame update
    void Start()
    {
        // populateDashRenewers();
        DisableDashRenewers();
    }

    public void EnableDashRenewers()
    {
        populateDashRenewers();
        foreach (DashRenewer dashRenewer in dashRenewers)
        {
            dashRenewer.EnableDashRenewer();
        }
    }

    // FORMER CALL by event from Ryker.cs when player returned from Angel to Ryker
    public void DisableDashRenewers()
    {
        populateDashRenewers();
        foreach (DashRenewer dashRenewer in dashRenewers)
        {
            dashRenewer.DisableDashRenewer();
        }
    }
}
