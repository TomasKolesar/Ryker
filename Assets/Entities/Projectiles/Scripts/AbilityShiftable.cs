using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShiftable : MonoBehaviour
{
    private Rigidbody2D rb;

    // Click in editor if script attached to Ryker
    [SerializeField] private bool isRyker = false;

    private TimeFlow timeFlow;


    private bool isShifting = false;
    private float shiftSpeed;
    private float maxShiftDistance;
    private float currentShiftDistance = 0f;
    private Vector3 shiftDirection = new Vector3();
    private Vector3 positionBeforeShift = new Vector3();
    
    // private GameObject rykerShiftBounds;
    private List<GameObject> wallBoundColliders = new List<GameObject>();

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        timeFlow = GameObject.Find("TimeFlow").GetComponent<TimeFlow>();
    }


    private void FixedUpdate() {
        if(isShifting) {
            // Move projectile in direction
            Vector3 newPosition = transform.position + shiftDirection * shiftSpeed * Time.deltaTime * timeFlow.getTimeFlow();
            transform.position = newPosition;
            // Check if reached max distance, if so then stop shifting
            if(Vector3.Distance(transform.position, positionBeforeShift) >= currentShiftDistance)
            {
                EndShifting();
            }
        }
    }

    public void SetWallBoundCollidersTrigger() {
        wallBoundColliders = new List<GameObject>(GameObject.FindGameObjectsWithTag("WallRykerBoundCollider"));
        foreach (GameObject wallBoundCollider in wallBoundColliders) {
            wallBoundCollider.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    public void SetWallBoundCollidersNotTrigger() {
        foreach (GameObject wallBoundCollider in wallBoundColliders) {
            wallBoundCollider.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }

    private void EndShifting() {
        isShifting = false;
        rb.velocity = Vector2.zero;
        // Continue player control if Ryker, new movement direction if projectile
        if (!isRyker) {
            GetComponent<AbilityGuidedUnguided>().enabled = true;
            GetComponent<AbilityGuidedUnguided>().SetMovementAfterShift(shiftDirection);
        } else { // When Ryker
            SetWallBoundCollidersTrigger();
            GetComponent<Collider2D>().isTrigger = true;
        }
        // reset for next shift
        currentShiftDistance = 0f;
    }

    public void ShiftYourslef(Vector3 shiftDirection, Vector3 angelPosition, float maxDistanceBetweenThisAndAngel, float shiftSpeed, float maxShiftDistance)
    {
        // Just for Ryker, because his position is frozen after InitDash
        if (GetComponent<Ryker>() != null) {
            // GetComponent<Ryker>().StopKeepingPositionAfterDash();
            SetWallBoundCollidersNotTrigger();
            GetComponent<Collider2D>().isTrigger = false;
        }

        // Disable preprogrammed movement on shifting object
        if (!isRyker) {
            GetComponent<AbilityGuidedUnguided>().enabled = false;
        }

        isShifting = true;
        this.shiftDirection = shiftDirection;
        this.shiftDirection.z = 0;
        this.shiftSpeed = shiftSpeed;
        this.maxShiftDistance = maxShiftDistance;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, shiftDirection);

        positionBeforeShift = transform.position;
        // maybe polish this?
        currentShiftDistance = (1 - (Vector3.Distance(transform.position, angelPosition)  / maxDistanceBetweenThisAndAngel)) * maxShiftDistance;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (isShifting && isRyker && other.gameObject.tag == "WallRykerBoundCollider") {
            EndShifting();
        }
    }
}
