using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTripple : MonoBehaviour
{
    private float spreadRate = 0f;
    [SerializeField ]private GameObject left;
    [SerializeField] private GameObject right;

    private bool[] isDestroyed = new bool[3];

    public void SetSpreadRate(float value) {
        this.spreadRate = value;
    }

    void Start()
    {
        isDestroyed[0] = false;
        isDestroyed[1] = false;
        isDestroyed[2] = false;
        // make projectiles spread
        left.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(spreadRate);
        right.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(-spreadRate);
    }

    public void SetDestroyed(GameObject child) {
        if(child == left) {
            isDestroyed[0] = true;
        } else if(child == right) {
            isDestroyed[2] = true;
        } else {
            isDestroyed[1] = true;
        }

        if(isDestroyed[0] && isDestroyed[1] && isDestroyed[2]) {
            // tell parent (SB) to fire again
            if (transform.parent != null) {
                transform.parent.GetComponent<FollowingSoulBreaker>().UpdateRotationAndFireAgain();
                transform.parent.GetComponent<FollowingSoulBreaker>().ClearCurrentProjectile();
            }
            // die
            Destroy(gameObject);
        }
    }
}
