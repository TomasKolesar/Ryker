using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointers : MonoBehaviour
{
    [SerializeField] GameObject pointerPrefab;
    // list of all instantiated pointers
    private List<GameObject> pointers = new List<GameObject>();
    
    private SoulBreakers soulBreakers;
    private List<GameObject> activeSBs = new List<GameObject>();

    [SerializeField] private GameObject rykerPointer;
    public GameObject GetRykerPointer() { return rykerPointer; }

    //
    //
    public void SetNavigationForAngel()
    {
        // OLD
        soulBreakers = FindObjectOfType<SoulBreakers>();
        // activeSBs = soulBreakers.GetAllSBs();

        GameObject angel = FindObjectOfType<Angel>().gameObject;

        GameObject pointer;
        PointerSB pointerScript;
        foreach (GameObject sb in activeSBs)
        {
            // Make new instance for speciifc SoulBreaker
            pointer = Instantiate(pointerPrefab, sb.transform.position, Quaternion.identity);
            // Add it to list of pointers
            pointers.Add(pointer);
            // Set it's parent Pointers parental
            pointer.transform.SetParent(transform);
            // Set necessary parameters
            pointerScript = pointer.GetComponent<PointerSB>();
            pointerScript.SetTarget(sb);
            pointerScript.SetOrigin(angel);
            pointerScript.SetIsPlayingForAngel(true);
            // Set pointer to SoulBreaker so it can be destroyed when SB is
            // sb.GetComponent<FollowingSoulBreaker>().SetPointer(pointer);
        }
    }

    public void DestroyAllSBPointers()
    {
        foreach(GameObject p in pointers)
        {
            Destroy(p);
        }
    }
}
