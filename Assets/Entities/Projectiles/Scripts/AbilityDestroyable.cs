using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDestroyable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Angel" && other.gameObject.GetComponent<Angel>().IsInDash()) {

            // Destroying only child when TypeTripple or TypeQuintuple
            if(transform.parent.gameObject.GetComponent<TypeTripple>() != null) {
                // tell parent that child is destroyed
                transform.parent.GetComponent<TypeTripple>().SetDestroyed(gameObject);
                // destroy child
                Destroy(gameObject);
                return;
            } else if(transform.parent.gameObject.GetComponent<TypeQuintuple>() != null) {
                // tell parent that child is destroyed
                transform.parent.GetComponent<TypeQuintuple>().SetDestroyed(gameObject);
                // destroy child
                Destroy(gameObject);
                return;
            // Destroying whole projectile when not TypeTripple
            } else {
                transform.parent.GetComponent<FollowingSoulBreaker>().UpdateRotationAndFireAgain();
                Destroy(gameObject);
            }

        }
    }
}
