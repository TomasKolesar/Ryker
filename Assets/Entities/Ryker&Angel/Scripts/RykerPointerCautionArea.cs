using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RykerPointerCautionArea : MonoBehaviour
{
    void Update() {
        transform.position = GameObject.Find("Ryker").transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Projectile")) {
            transform.parent.gameObject.GetComponent<Ryker>().CautionPointer();
        }
    }
}
