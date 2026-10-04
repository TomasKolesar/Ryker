using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingGates : MonoBehaviour
{
    private List<GameObject> gates = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        gates.Clear();
        foreach(Transform t in transform){
            gates.Add(t.gameObject);
        }
    }

    public void DisableIsTrigger(){
        foreach(GameObject g in gates){
            g.GetComponent<FlashingGate>().DisableIsTrigger();
        }
    }

    public void SetIsAngelInDash(bool val){
        foreach(GameObject g in gates){
            g.GetComponent<FlashingGate>().setAngelInDash(val);
        }
    }
}
