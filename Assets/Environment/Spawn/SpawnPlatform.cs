using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class SpawnPlatform : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ryker"))
        {
            if (FindObjectOfType<SoulBreakers>() != null) FindObjectOfType<SoulBreakers>().AllSoulBreakersActivate();
            Destroy(gameObject); // Later remake to animation of closing elevator platform
        }
    }
}
