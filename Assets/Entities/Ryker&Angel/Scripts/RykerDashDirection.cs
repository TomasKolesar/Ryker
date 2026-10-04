using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RykerDashDirection : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector3 direction = (worldPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
}
