using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowingSoulBreaker : MonoBehaviour
{
    // Phases of SB (chronological order)
    // 1. Nothing 
    // 2. Following & countdown = Activate()
    // 3. instantiating projectile = StartFiring()
    // 4. firing projectile = end fo StartFiring()
    // 5. projectile destroyed => rotate towards Ryker new position at rotateSpeed
    // 6. repeat from step 3
    private bool isFollowingRyker = false; // looking at target, counting (fading) until fire, all before InitDash
    private bool isRotatingTowardsTarget = false; // updating rotation after projectile was destroyed, before second or any other fire

    private GameObject target;

    // Fading before firing
    [Header("Fading")]
    private Coroutine coroutineFade;
    [SerializeField] private GameObject warningLight;
    [SerializeField] private float warningLightDuration = 6f;
    [SerializeField] private float additionalTimeToReact = 0.2f;

    // References
    private GameObject currentProjectile;
    private GameObject pointer;
    private TimeFlow timeFlow;
    private BoxCollider2D coll;

    public void ClearCurrentProjectile() {
        currentProjectile = null;
    }

    // Rotation towards new Ryker position
    [Header("Rotation after first fire")]
    [SerializeField] private float rotationSpeed = 1f;

    // Raycasting
    [SerializeField] private GameObject raycastOrigin;
    [SerializeField] private LayerMask playerLayerMask;
    private bool keepTrackOfProjectileDistance = false;
    private float rykersDistanceFromSB = 0f;

    // Disolving when destroyed
    [Header("Dissolving")]
    [SerializeField] private float dissolveSpeed = 2f;
    [SerializeField] private float dissolveDurationFrames = 10f;
    private float dissolveDiscountFactor;
    Material material;
    private bool isDissolving = false;
    [SerializeField] Shader dissolveShader;

    private bool dissolvementFrozen = false;


    //
    // Setters
    public void SetTarget(GameObject target) {
        this.target = target;
    }

    public void SetTriggerTrue() {
        coll.isTrigger = true;
    }

    public void SetTriggerFalse() {
        coll.isTrigger = false;
    }
    
    public void FreezeDissolvement() {
        dissolvementFrozen = true;
    }
    public void UnfreezeDissolvement() {
        dissolvementFrozen = false;
    }

    public void SetPointer(GameObject p) {
        this.pointer = p;
    }

    //
    // Start is called before the first frame update
    void Start()
    {
        timeFlow = GameObject.Find("TimeFlow").GetComponent<TimeFlow>();
        material = GetComponent<SpriteRenderer>().material;
        coll = GetComponent<BoxCollider2D>();
    }

    //
    // 
    void Update()
    {
        if (isDissolving) {
            // Disolve
            if (!dissolvementFrozen) {
                material.SetFloat("_Fade", material.GetFloat("_Fade") - dissolveDiscountFactor);
            }
            // Die after disolving
            if (material.GetFloat("_Fade") <= 0.01f) {
                Die();
            }
            return;
        }

        if(isFollowingRyker){
            Vector2 direction = target.transform.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }

        if(isRotatingTowardsTarget && currentProjectile == null){
            Vector2 direction = target.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, direction), rotationSpeed * Time.deltaTime);
            // If rotated enough, stop rotating
            if(Vector2.Angle(transform.up, direction) < .25f){
                isRotatingTowardsTarget = false;
                StartFiring();
            }
        }

        // Check whether projectile reached enough behind Ryker to potentially hit Ryker
        if (keepTrackOfProjectileDistance) {
            Vector3[] positionsOnNextFrame = currentProjectile.GetComponent<Projectile>().GetPositionsOnNextFrame();
            if (positionsOnNextFrame == null) {
                return;
            // TypeBasic or TypeWideBeam
            }else if (positionsOnNextFrame.Length == 1) {
                if(Vector3.Distance(positionsOnNextFrame[0], raycastOrigin.transform.position) > rykersDistanceFromSB) {
                    RaycastedProjectileHitRyker();
                }
            // TypeTripple or TypeQuintuple
            }else if (positionsOnNextFrame.Length == 3 || positionsOnNextFrame.Length == 5) {
                for (int i = 0; i < positionsOnNextFrame.Length; i++) { // for cycle, cause need a reference to bullet that hits
                    if(Vector3.Distance(positionsOnNextFrame[i], raycastOrigin.transform.position) > rykersDistanceFromSB) {
                        RaycastedProjectileHitRyker();
                    }
                }
            }
        }
    }

    private void RaycastedProjectileHitRyker() {
        // Stop this enourmous part of code from executing
        keepTrackOfProjectileDistance = false;
        // Let AbilityGuidedUnguided.cs know that player died, so it won't update projectile's position anymore
        currentProjectile.GetComponent<Projectile>().SetPlayerDiedAndRykersDistance(true, rykersDistanceFromSB);
        // Let Ryker.cs know that he died -> handle timeflow, colliders, Angel destruction, died text etc.
        FindObjectOfType<Ryker>().PlayerDied();
    }

    //
    // Runs when player leaves elevator, looking at target, countdown, starts firing
    public void Activate()
    {
        // Start looking at target
        isFollowingRyker = true;
        // Warning light starts fading = countdown until initiation of firing
        coroutineFade = StartCoroutine(Fade(StartFiring));
    }

    // SB blinks 3 times (that's why we divide by 6) + waits for additionalTimeToReact and 
    // then triggers callback method, in this case initiating firing projectile
    public IEnumerator Fade(Action callback = null)
    {
        SpriteRenderer warningLightRenderer = warningLight.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            // Fade in
            for (float t = 0.0f; t < (warningLightDuration / 6f); t += Time.deltaTime) {
                float alpha = Mathf.Lerp(0f, 1f, t / (warningLightDuration / 6f));
                Color oldColor = warningLightRenderer.color;
                warningLightRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
                yield return null;
            }

            // Fade out
            for (float t = 0.0f; t < (warningLightDuration / 6f); t += Time.deltaTime) {
                float alpha = Mathf.Lerp(1f, 0f, t / (warningLightDuration / 6f));
                Color oldColor = warningLightRenderer.color;
                warningLightRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
                yield return null;
            }
        }
        // let's give player additional time to react,
        yield return new WaitForSeconds(additionalTimeToReact);

        callback?.Invoke();
    }

    //
    // Instantiates projectile and then tells him to do what it's programmed to do
    public void StartFiring()
    {
        if (timeFlow.getTimeFlow() == timeFlow.getNormalTime()) {
            // RayCast for checking if SB will hit Ryker in regular timeFlow
            RaycastHit2D hitInfo = Physics2D.Raycast(   raycastOrigin.transform.position, 
                                                        (raycastOrigin.transform.position - transform.position).normalized, 
                                                        Mathf.Infinity,
                                                        playerLayerMask);
            // If hit Ryker, keep track of projectile distance and continue checking in Update()
            if(hitInfo.transform.name == "Ryker"){
                keepTrackOfProjectileDistance = true;
                // rykersDistanceFromSB = Vector2.Distance(hitInfo.transform.position, raycastOrigin.transform.position);
                rykersDistanceFromSB = Vector3.Distance(target.transform.position, raycastOrigin.transform.position);
            }
        }

        // Do not rotate, so projectiles will be aligned
        isFollowingRyker = false;
        // Instantiate projectile
        currentProjectile = GetComponent<ProjectileFactory>().CraftProjectile();
        currentProjectile.transform.SetParent(transform);
        // Give projectile reference to target and let it do its thing
        currentProjectile.GetComponent<Projectile>().SetTarget(target);
        // Give projectile and target references to pointer
        currentProjectile.GetComponent<Projectile>().Activate();
    }

    public void ManualFire() {
        if (coroutineFade != null) {
            StopCoroutine(coroutineFade);
        }
        StartFiring();
    }

    //
    // Give a command to rotate towards target's new position
    public void UpdateRotationAndFireAgain()
    {
        isRotatingTowardsTarget = true;
    
    }

    // Also exit, because of situation when Angel's touching SB at the start of dash -> not detected when using enter
    private void OnTriggerExit2D(Collider2D other) {
        CheckIfAngelDashedThrough(other);
        
        if (other.gameObject.GetComponent<AbilityGuidedUnguided>() == null) return;

        if (other.gameObject.GetComponent<Projectile>() != null) {
            other.gameObject.GetComponent<Projectile>().SetLeftSoulBreakersCollider(true);
        }else if (other.transform.parent.gameObject.GetComponent<Projectile>() != null) {
            other.transform.parent.gameObject.GetComponent<Projectile>().SetLeftSoulBreakersCollider(true);
        }   
    }

    // For cases when Angel's dash ends with Angel still within SB
    private void OnTriggerEnter2D(Collider2D other) {
        CheckIfAngelDashedThrough(other);
        CheckIfHitByHarmfulProjectile(other);
    }

    private void CheckIfAngelDashedThrough(Collider2D other) {
        if(other.gameObject.tag == "Angel" && other.gameObject.GetComponent<Angel>().IsInDash()) {
            dissolveDurationFrames = other.GetComponent<Angel>().GetRemainingDashDurationFramesCount();
            HandleDestroyment(other);
        }
    }

    private void CheckIfHitByHarmfulProjectile(Collider2D other) {
        bool projectileLeftCollider = CheckWhetherProjectileLeftCollider(other);
        if(other.gameObject.GetComponent<AbilityGuidedUnguided>() == null && !projectileLeftCollider) return;

        if (other.gameObject.GetComponent<Projectile>() != null) {
            if (other.gameObject.GetComponent<Projectile>().IsHarmful()) {
                HandleDestroyment(other);
            }
        }else if(other.transform.parent.gameObject.GetComponent<Projectile>() != null) {
            if (other.transform.parent.gameObject.GetComponent<Projectile>().IsHarmful()) {
                HandleDestroyment(other);
            }
        }
    }

    private bool CheckWhetherProjectileLeftCollider(Collider2D other) {
        if(other.gameObject.GetComponent<AbilityGuidedUnguided>() != null) {
            if (other.gameObject.GetComponent<Projectile>() != null) {
                return other.gameObject.GetComponent<Projectile>().GetLeftSoulBreakersCollider();
            }else if(other.transform.parent.gameObject.GetComponent<Projectile>() != null) {
                return other.transform.parent.gameObject.GetComponent<Projectile>().GetLeftSoulBreakersCollider();
            }
        }
        return false;
    }
    private bool CheckWhetherProjectileHarmful(Collider2D other) {
        if(other.gameObject.GetComponent<AbilityGuidedUnguided>() != null) {
            if (other.gameObject.GetComponent<Projectile>() != null) {
                return other.gameObject.GetComponent<Projectile>().IsHarmful();
            }else if(other.transform.parent.gameObject.GetComponent<Projectile>() != null) {
                return other.transform.parent.gameObject.GetComponent<Projectile>().IsHarmful();
            }
        }
        return false;
    }

    private void HandleDestroyment(Collider2D destroyer) {
        // Remove from parent list, but destroyed later for visual feedback
        transform.parent.GetComponent<SoulBreakers>().RemoveSoulBreaker(gameObject);
        // Disolve itself, then destroy itself
        dissolveDiscountFactor = material.GetFloat("_Fade") / (dissolveDurationFrames * ( 3f/4 ));
        warningLight.SetActive(false);
        isDissolving = true;
        // Call disolve of projectile at the same speed and turn off its collider
        if (currentProjectile != null) { // may be set to null by subscripts right before being desrtoyed for time untill new is fired
            currentProjectile.GetComponent<Projectile>().DisableAllColliders();
            currentProjectile.GetComponent<Projectile>().DissolveAllProjectiles(dissolveShader, dissolveSpeed);
        }
        // Prevent SB while dissolving from getting hit again or stop Angel's dash
        Physics2D.IgnoreCollision(coll, destroyer, true);
    }

    private void Die() {
        SoulBreakers sbs = transform.parent.GetComponent<SoulBreakers>();
        if (sbs.GetLastSBReference() != null) {
            sbs.LaunchPostWinPhase();
        }
        // Handling destruction of SB
        Destroy(pointer);
        Destroy(gameObject);
    }
}
