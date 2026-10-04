using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRenewer : MonoBehaviour
{
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;

    public void EnableDashRenewer()
    {
        GetComponent<SpriteRenderer>().sprite = enabledSprite;
    }

    public void DisableDashRenewer()
    {
        GetComponent<SpriteRenderer>().sprite = disabledSprite;
    }
}
