using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWinDeath : MonoBehaviour
{
    [SerializeField] private GameObject deathTexts;
    [SerializeField] private GameObject winTexts;

    public void ShowDeathTexts()
    {
        deathTexts.SetActive(true);
    }

    public void ShowWinTexts()
    {
        winTexts.SetActive(true);
    }
}
