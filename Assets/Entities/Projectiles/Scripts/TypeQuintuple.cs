using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeQuintuple : MonoBehaviour
{
    private float spreadRate = 0f;
    [SerializeField ]private GameObject leftLeft;
    [SerializeField ]private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField ]private GameObject rightRight;

    private bool[] isDestroyed = new bool[5];

    public void SetSpreadRate(float value) {
        this.spreadRate = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        isDestroyed[0] = false;
        isDestroyed[1] = false;
        isDestroyed[2] = false;
        isDestroyed[3] = false;
        isDestroyed[4] = false;

        // make projectiles spread
        leftLeft.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(spreadRate * 2);
        left.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(spreadRate);
        right.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(-spreadRate);
        rightRight.GetComponent<AbilityGuidedUnguided>().SetSpreadRotation(-spreadRate * 2);
    }

    public void SetDestroyed(GameObject child) {
        if(child == leftLeft) {
            isDestroyed[0] = true;
        } else if(child == left) {
            isDestroyed[1] = true;
        } else if(child == right) {
            isDestroyed[2] = true;
        } else if(child == rightRight) {
            isDestroyed[3] = true;
        } else {
            isDestroyed[4] = true;
        }

        if(isDestroyed[0] && isDestroyed[1] && isDestroyed[2] && isDestroyed[3] && isDestroyed[4]) {
            // tell parent (SB) to fire again if parent hasn't been already destroyed
            if (transform.parent != null) {
                transform.parent.GetComponent<FollowingSoulBreaker>().UpdateRotationAndFireAgain();
                transform.parent.GetComponent<FollowingSoulBreaker>().ClearCurrentProjectile();
            }
            // die
            Destroy(gameObject);
        }
    }
}
