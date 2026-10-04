using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Vector3 newRotation;
    // Update is called once per frame
    void Update()
    {
        newRotation = transform.eulerAngles;
        newRotation.z = 0;
        transform.eulerAngles = newRotation;
    }
}
