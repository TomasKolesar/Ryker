using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Boost Attributes")]
    [SerializeField] private float speed = 2000f;
    [SerializeField] private float boostDistance = 3f;
    [SerializeField] private float smoothTime = .1f;

    [Header("Activeness Colors")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.grey;

    private Vector2 boostDirection;


    // Find out boostDirection based on own orientation
    private void Awake() {
        boostDirection = Quaternion.Euler(transform.rotation.eulerAngles).normalized * Vector2.up;
    }

    //
    // Handling color change based on currently controlled character
    //
    public void SetActiveColor() {
        GetComponent<SpriteRenderer>().color = activeColor;
    }

    public void SetInactiveColor() {
        GetComponent<SpriteRenderer>().color = inactiveColor;
    }

    // Boosting object while it stays inside the trigger
    private void OnTriggerStay2D(Collider2D other) {
        Bounds triggerBounds = GetComponent<Collider2D>().bounds;
        Bounds otherBounds = other.bounds;

        if (triggerBounds.Contains(otherBounds.min) &&
            triggerBounds.Contains(otherBounds.max) &&
            other.CompareTag("Ryker"))
        {
            Ryker r = other.GetComponent<Ryker>();
            r.setIsInBoost(true);
            r.HideDashDirection();
            other.GetComponent<PlatformReaction>().SetUpDash(speed, boostDistance, smoothTime, boostDirection);
        }
    }
}