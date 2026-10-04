using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelShiftIndicator : MonoBehaviour
{
    private List<GameObject> perojectilesWithinCollider = new List<GameObject>();
    public List<GameObject> GetProjectilesWithinCollider() {return perojectilesWithinCollider;}
    private GameObject rykerWithinColliderReference;
    public GameObject GetRykerWithinColliderReference() {return rykerWithinColliderReference;}

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Ryker")) {
            rykerWithinColliderReference = other.gameObject;
        } else if (other.gameObject.CompareTag("Projectile")){
            perojectilesWithinCollider.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Ryker")) {
            rykerWithinColliderReference = null;
        } else if (other.gameObject.CompareTag("Projectile")){
            perojectilesWithinCollider.Remove(other.gameObject);
        }
    }
}
