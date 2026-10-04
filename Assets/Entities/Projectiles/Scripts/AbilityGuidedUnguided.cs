using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGuidedUnguided : MonoBehaviour
{
    private Rigidbody2D rb;

    private float speed = 0f;
    private bool activated = false;
    private bool guided = false;
    private Vector3 positionInNextFrame = new Vector3();

    private GameObject target;
    private Vector3 targetLocation = new Vector3();
    private Vector3 direction = new Vector2();

    private bool hasBeenShifted = false;

    private float rotationOfTypeMultiple = 0f; // only when projectile is left or right of TypeTripple

    private TimeFlow timeFlow;

    private GameObject guidedVisual;
    private float blinkTime = 1f;

    private bool isCovering = false;
    private bool isTypeTrippleOrQuintuple = false;

    private Material material;
    private bool isDisolving = false;
    private float disolveSpeed = 0f;

    private bool playerDied = false;
    private float rykersDistanceFromSB = 0;

    //
    // Getters
    public Vector3 GetPositionOnNextFrame() {
        return positionInNextFrame;
    }

    //
    // Setters

    public void SetGuidedTrue() {
        guided = true;
    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }

    public void SetMovementAfterShift(Vector3 direction) {
        hasBeenShifted = true;
        this.direction = direction;
        // If projectile is guided, it updates direction on next frame and every frame after that
    }

    public void SetSpreadRotation(float degrees) {
        this.rotationOfTypeMultiple = degrees;
        SetDirection();
    }

    public void SetIsCoveringTrue() {
        isCovering = true;
        SetDirection();
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SetIsTypeTrippleOrQuintupleTrue() {
        isTypeTrippleOrQuintuple = true;
    }

    public void SetPlayerDiedAndRykersDistance(bool val, float distance) {
        playerDied = val;
        rykersDistanceFromSB = distance;
    }

    public void Activate() {
        activated = true;
    }

    //
    //
    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        timeFlow = GameObject.Find("TimeFlow").GetComponent<TimeFlow>();
        SetDirection();
        
        if (guided) {
            guidedVisual = transform.Find("VisualsParental/AbilityGuidedVisual(Clone)").gameObject;
            StartCoroutine(KeepFading());
        }
    }

    private void FixedUpdate() {
        if (!activated || (target == null && !isCovering)) return;

        if (guided) {
            GuidedMovement();
        } else {
            UnguidedMovement();
        }
    }

    private void Update() {
        if (isDisolving) {
            // Asign material reference if not done yet
            if (!material) {
                material = GetComponent<SpriteRenderer>().material;
            }
            // Disolve
            material.SetFloat("_Fade", material.GetFloat("_Fade") - disolveSpeed * Time.deltaTime);
            return;
            // No need for death function or end of disolving check, cause it's the same speed as SB
        }
    }

    private void UnguidedMovement() {
        if (playerDied) {
            MoveProjectileToRyker();
            return;
        }
        // Move projectile
        if (transform.position == transform.parent.position || hasBeenShifted) {
            transform.position += direction.normalized * speed * Time.deltaTime * timeFlow.getTimeFlow();
        }else{
            transform.position = positionInNextFrame;
        }
        // Keep track of future position, so FollowingSoulBreaker.cs can calculate whether will be target hit
        // After playe's death this variable does not change because of timeflow being 0
        positionInNextFrame = transform.position + direction.normalized * speed * Time.deltaTime * timeFlow.getTimeFlow();
    }

    private void MoveProjectileToRyker() {
        while (Vector3.Distance(positionInNextFrame, transform.parent.position) > rykersDistanceFromSB) {
            positionInNextFrame -= (direction.normalized / 10);
        }
        transform.position = positionInNextFrame;
    }

    private void GuidedMovement() {
        // Update it every frame, because Ryker may change position
        SetDirection();
        // Move towards the target
        if(activated && target != null){
            transform.position = Vector3.MoveTowards(   transform.position, 
                                                        targetLocation, 
                                                        speed * Time.deltaTime * timeFlow.getTimeFlow());
        }
    }

    private void SetDirection() {
        if (isCovering){
            if (isTypeTrippleOrQuintuple){
                direction = transform.parent.parent.localPosition;
            }else {
                direction = transform.parent.localPosition;
            }
        } else {
            // if direction had already changed
            if (hasBeenShifted && !guided) return;
            // For the navigation
            targetLocation = target.transform.position;
            // Rotate in the right direction
            direction = targetLocation - transform.position;
        }
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        direction = Quaternion.Euler(0, 0, rotationOfTypeMultiple) * direction;
    }

    // Detection has to happen here, because this script is always attached to gameobject with collider
    private void OnTriggerEnter2D(Collider2D other) {
        // Check for typeWideBeam that gets destroyed when it's centre hits the wall
        Collider2D wallDetectCollider = null;
        if (transform.childCount == 2) {
            Transform wallDetector = transform.GetChild(1);
            wallDetectCollider = wallDetector.GetComponent<Collider2D>();
            if (wallDetectCollider == null || 
                !wallDetector.gameObject.CompareTag("WallDetectorCollider") ||
                !other.IsTouching(wallDetectCollider)) {
                return;
            }
        }

        // Destroys itself when hits the wall and tells parent to fire another one; only when playing for angel; raycast for normal timeFlow
        if(other.gameObject.tag == "Wall" && !guided && timeFlow.getTimeFlow() != timeFlow.getNormalTime()) {
            // Also check if wall has been hit by collider that is to kill player or by smaller one for wall detection
            if (transform.parent.GetComponent<TypeTripple>() != null) {
                transform.parent.GetComponent<TypeTripple>().SetDestroyed(gameObject);
            }else if(transform.parent.GetComponent<TypeQuintuple>() != null) {
                transform.parent.GetComponent<TypeQuintuple>().SetDestroyed(gameObject);
            }else {
                if (isCovering && transform.parent.parent.GetComponent<CoveringSoulBreaker>() != null) {
                    // remove projectile from list of current projectiles
                    
                } else if(isCovering && transform.parent.parent.GetComponent<CoveringSoulBreakerCustomizableCanon>() != null) {
                    transform.parent.GetComponent<CoveringSoulBreakerCustomizableCanon>().ClearCurrentProjectile();
                } else {
                    transform.parent.GetComponent<FollowingSoulBreaker>().UpdateRotationAndFireAgain();
                    transform.parent.GetComponent<FollowingSoulBreaker>().ClearCurrentProjectile();
                }
                Destroy(gameObject);
            }
        }
    }

    // Guided visual indication, red dot, keeps blinking
    public IEnumerator KeepFading()
    {
        SpriteRenderer renderer = guidedVisual.GetComponent<SpriteRenderer>();
        while (true)
        {
            // Fade in
            for (float t = 0.0f; t < (blinkTime / 6f); t += Time.deltaTime) {
                float alpha = Mathf.Lerp(0f, 1f, t / (blinkTime / 6f));
                Color oldColor = renderer.color;
                renderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
                yield return null;
            }

            // Fade out
            for (float t = 0.0f; t < (blinkTime / 6f); t += Time.deltaTime) {
                float alpha = Mathf.Lerp(1f, 0f, t / (blinkTime / 6f));
                Color oldColor = renderer.color;
                renderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
                yield return null;
            }
        }
    }

    public void Dissolve(float speed) {
        disolveSpeed = speed;
        isDisolving = true;
    }
}
