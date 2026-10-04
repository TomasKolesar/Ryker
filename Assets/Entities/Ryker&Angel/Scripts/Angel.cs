using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class Angel : MonoBehaviour
{
    // Components
    private Rigidbody2D rigidBody; // dash velocity
    private Movement movement;
    private TrailRenderer trailRenderer;
    private GameObject ryker; // call function when player dies/wins
    public void SetRyker(GameObject gameObject) { 
        if (!gameObject.GetComponent<Ryker>()) return;
        this.ryker = gameObject;
    }
    private Material material; // dissolvement

    //
    //  DASH
    private SoulBreakers soulBreakersScript;
    // Dash variables and phases
    private Vector2 dashStartPosition;
    private Vector2 dashDirection = new Vector2(0,0);
    private bool isInitDashing = false;
    private bool isAttacking = false;
    public bool IsInDash() { return isAttacking || isInitDashing; } // To check whether SB can be destroyed
    // For enabling SBs to be destroyed when Angel is dashing
    private void SetIsAttacking(bool val) {
        this.isAttacking = val;
    }

    private void SetIsInitDashing(bool val) {
        if (val) soulBreakersScript.AllSoulBreakersSetTriggerTrue();
        this.isInitDashing = val;
    }

    [Header("Dash physics")]
    [SerializeField] float dashSpeed = 100;
    private float accelerationRateX;
    private float accelerationRateY;
    [SerializeField] float dashFrozenFrames = 4f;
    private float dashFrozenFramesCount = 0f;
    public float GetRemainingDashDurationFramesCount() { return dashDurationFramesCount; }
    [SerializeField] float dashDurationFrames = 11f;
    private float dashDurationFramesCount = 0f;
    [SerializeField] float winSlowMoCoefficient = 8f;

    [Header("Animation handling")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject dashLeftoversPrefab;
    private bool instantiatedDashLeftovers = false;

    [Header("Camera shake during dash")]
    [SerializeField] private float shakeIntensity = 3f;
    [SerializeField] private float shakeTime = .5f;

    [Header("Dash indicators")]
    [SerializeField] private GameObject dashIndicators;
    private AngelDashIndicators dashIndicatorsScript;
    private int storedDashes = 0;
    private bool canAimDash = true;
    private bool isAimingDash = false;
    public bool IsAimingDash() { return isAimingDash; }
    private bool canDash = true;

    [Header("Countdown before another dash")]
    [SerializeField] float firstAttackWait = .2f;
    [SerializeField] float nextAttackWait = .45f;
    private float countdownDenominator = 1f;

    [Header("Slow-mo limit countdown")]
    [SerializeField] float slowMoLimit = 100f;
    private float currentSlowMoLimit;
    [SerializeField] float slowMoLimitDischargeRate = 20f;
    [SerializeField] float slowMoLimitRechargeRate = 5f;
    [SerializeField] float slowMoLimitRestoreRate = 20f;

    //
    //  SHIFT
    [Header("Shift indicator")]
    [SerializeField] private GameObject shiftRykerChargeIndicator;
    [SerializeField] private GameObject shiftProjectilesChargeIndicator;
    [SerializeField] private GameObject shiftIndicator;
    private AngelShiftIndicator shiftIndicatorScript;
    private bool isAimingShift = false;
    public bool IsAimingShift() { return isAimingShift; }

    [Header("Shift physics")]
    [SerializeField] float maxShiftDistance = 7f;
    [SerializeField] float shiftSpeed = 1200f;

    [Header("Shift charging")]
    [SerializeField] float shiftRykerChargeTime = 1.5f;
    private float shiftRykerChargeCounter;
    [SerializeField] float shiftProjectilesChargeTime = 1.5f;
    private float shiftProjectilesChargeCounter;

    // Camera boundaries variables
    private Camera mainCamera;
    private Vector3 screenBounds;
    private Vector3 previousScreenBounds;
    private Vector3 rykerPosition;
    private Transform rykerTransform;
    // Input
    private PlayerControls playerInput;

    // Win handling
    private bool playerWon = false;
    private bool adjustedAfterWin = false;
    public void SBDestroyed(float SBCount) { // caled everytime angel destroys SB, set true only when there's 1 left
        if(SBCount == 0) { 
            playerWon = true;
        }
    }
    private bool isDialogueAfterWin = false;
    public void SetIsDialogueAfterWin(bool val) { isDialogueAfterWin = val; }
    // Freezing dash
    private bool dashFrozen = false;
    private Vector2 previousTargetVelocity;
    // Dissolvement after win
    private bool isDissolving = false;
    private float dissolveDiscountFactor = 0f;

    // TimeFlow for slowing time
    private TimeFlow timeFlow;



    //
    // Initialize components and causes InitialDash to be triggered when object is created
    void Start()
    {
        InitializeComponents();

        // Enable all components like trailrenderer for initial dash that will be triggered when object is created
        SetupDashing();
        SetIsInitDashing(true);

        //
        shiftRykerChargeCounter = shiftRykerChargeTime;
        shiftProjectilesChargeCounter = shiftProjectilesChargeTime;
    }

    // Initialize components of this gameobject
    private void InitializeComponents()
    {
        // components of this gameobject
        rigidBody = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        movement = FindObjectOfType<Movement>();
        // dash and shift indicators
        dashIndicatorsScript = dashIndicators.GetComponent<AngelDashIndicators>();
        shiftIndicatorScript = shiftIndicator.GetComponent<AngelShiftIndicator>();
        // components of other gameobjects
        timeFlow = FindObjectOfType<TimeFlow>();
        mainCamera = Camera.main;
        // for enabling to dash through SB
        soulBreakersScript = FindObjectOfType<SoulBreakers>();
        // variables for limiting Angel's movement within screen
        rykerTransform = FindObjectOfType<Ryker>().transform;
        rykerPosition = rykerTransform.position;
        previousScreenBounds = new Vector3(0,0,0);
        // for subtracting time of every frame from slowmo limit
        currentSlowMoLimit = slowMoLimit;
        // for dissolvement after win
        material = GetComponent<SpriteRenderer>().material;
        //
        playerInput = FindObjectOfType<PlayerControls>();
    }

    //
    // Takes care of dashing after player releases RightMouseButton
    // Also fixes camera errors (Angel leaving screen, Camera movign after initial dash) and takes care of initial and attack dashes
    void Update()
    {
        // makes slowmo unavailable untill it fully charges, triggered when player reaches 0 slowmolimit
        if (currentSlowMoLimit <= 0f)
        {
            EndSlowmo();
            canAimDash = false;
            dashIndicatorsScript.StartRestoringSlowMoLimit();
        }else if (currentSlowMoLimit >= slowMoLimit)
        {
            canAimDash = true;
            dashIndicatorsScript.StopRestoringSlowMoLimit();
        }

        // Update rotation towards mouse cursor for animations
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

    }

    void FixedUpdate()
    {
        HandleSlowMoCountdown();
        HandleShiftRecharge();
        HandleDashing();
        HandleDissolvementAfterWin();
    }

    // INPUT
    // Either make a dash or shift, based on which button is being pressed (RightMouseButton or Shift)
    // if no button is being pressed, Angel makes dash without aiming
    public void MouseLeft()
    {
        // DASH
        if (canDash && !isAimingShift)
        {
            SetupDashing();
            SetIsAttacking(true);
        } else if (storedDashes > 0 && !isInitDashing && !isAttacking && !isAimingShift) 
        { 
            storedDashes--;
            dashIndicatorsScript.HideLastStoredDashIndicator();
            SetupDashing();
            SetIsAttacking(true);
        }
        // SHIFT Ryker
        if (isAimingShift && shiftRykerChargeCounter >= shiftRykerChargeTime)
        {
            ShiftRyker();
        }
    }

    // If player is not aiming shift, then it starts slowmo, otherwise it shifts only projectiles
    public void MouseRight() {
        // DASH Aim
        if (canAimDash && !isAimingShift)
        {
            isAimingDash = true;
            dashIndicatorsScript.DrawOutDashDirection();
            StartSlowmo();
        }

        // SHIFT projectiles
        if (isAimingShift && shiftProjectilesChargeCounter >= shiftProjectilesChargeTime) {
            ShiftProjectiles();
        }
    }

    //
    //  DASHING
    //
    // Processing time wait between dashes, setting up dash starting position, bool variables or waiting for next dash
    void SetupDashing()
    {
        // Hide base dash indicator
        dashIndicatorsScript.IndicateBaseDashReadyNot();
        // Make Angel able to destroy them during dashing
        soulBreakersScript.AllSoulBreakersSetTriggerTrue();
        // Set up necessary variables for dashing
        // These two will be reverted in EndDashing()
        trailRenderer.emitting = true;
        movement.enabled = false;
        // Reverted after waitfornextdash countdown
        canDash = false;
        // Necessary for calculations
        dashStartPosition = transform.position;
        dashDirection = new Vector2(0,0);
        // Setup frame counters
        dashFrozenFramesCount = dashFrozenFrames;
        dashDurationFramesCount = dashDurationFrames;
        // So they change color, frequency or colliders. Whatever the fuck they want
        FindObjectOfType<FlashingGates>().SetIsAngelInDash(true);
    }

    // Processes dashes that are enabled based on their conditions
    private void HandleDashing()
    {
        ProcessDash(isInitDashing, firstAttackWait);
        ProcessDash(isAttacking, nextAttackWait);
    }

    // Taking care of physics of InitialDash and AttackDash, called every frame
    void ProcessDash(bool dashCondition, float waitTimeAfterDash)
    {
        if (!dashCondition) { return; }
        if (dashFrozen) { return; }
        //
        // direction
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 myPos = transform.position;
        if (dashDirection == new Vector2(0,0)) 
        { 
            dashDirection = targetPosition - myPos;
        }
        dashDirection.Normalize();
        //
        // handle freezing
        if (dashFrozenFramesCount > 0) {
            dashFrozenFramesCount -= 1;
            if (timeFlow.getTimeFlow() != 0) timeFlow.freezeTime();
            return;
        } else if (dashFrozenFramesCount == 0) {
            dashFrozenFramesCount -= 1;
            timeFlow.setTimeFlow(timeFlow.getAngelTime());
            CinemachineShake.Instance.ShakeCamera(shakeIntensity, shakeTime);
            animator.SetTrigger("dashAnimationTrigger");
        }

        if (!instantiatedDashLeftovers && !isInitDashing) {
            Instantiate(dashLeftoversPrefab, transform.position, Quaternion.identity);
            instantiatedDashLeftovers = true;
        }

        // Make another dash with half length behind destroyed body of last SB at modified lower speed
        if (playerWon && !adjustedAfterWin) {
            dashDurationFramesCount = dashDurationFrames * winSlowMoCoefficient;
            dashSpeed /= winSlowMoCoefficient;
            adjustedAfterWin = true;
        }

        // Play dialogue aftter win (if there's one) and start disolvement after 1/3 of dash length behind last SB
        if (playerWon && adjustedAfterWin && dashDurationFramesCount == Mathf.Round( dashDurationFrames * winSlowMoCoefficient * (2f/3f))) {
            if (isDialogueAfterWin) {
                // Freeze dash
                FreezeDashAndLastSBDissolvement();
                // Freeze dissolvment of last SB
                FindObjectOfType<DialogueTrigger>().PlayDialogueAfterWin();
                // Do not repeat
                isDialogueAfterWin = false;
            }else if (!isDissolving){
                StartWinDissolvement();
            }
            dashIndicators.SetActive(false);

            dashDurationFramesCount -= 1;
            return;
        }

        //
        // handle dashing     
        Vector2 targetVelocity = dashDirection * dashSpeed;
        if (dashDurationFramesCount > 0) {
            dashDurationFramesCount -= 1;
            rigidBody.velocity = targetVelocity;
            return;
        } else if (dashDurationFramesCount == 0) {
            if (isInitDashing) SetIsInitDashing(false);
            else SetIsAttacking(false);

            EndDashing(waitTimeAfterDash);
        }
    }

    private void StartWinDissolvement() {
        // Hide dash arrow and disable trail renderer
        dashIndicatorsScript.HideDashDirection();
        trailRenderer.enabled = false;
        // Start dissolving Angel for the rest of the last dash
        dissolveDiscountFactor = material.GetFloat("_Fade") / dashDurationFramesCount;
        isDissolving = true;
    }

    public void FreezeDashAndLastSBDissolvement() {
        dashFrozen = true;
        previousTargetVelocity = rigidBody.velocity;
        rigidBody.velocity = Vector2.zero;
        if (soulBreakersScript.GetLastSBReference() != null) {
            soulBreakersScript.GetLastSBReference().GetComponent<FollowingSoulBreaker>().FreezeDissolvement();
        }
    }

    public void UnfreezeDashAndLastSBDissolvement() {
        dashFrozen = false;
        rigidBody.velocity = previousTargetVelocity;
        if (soulBreakersScript.GetLastSBReference() != null) {
            soulBreakersScript.GetLastSBReference().GetComponent<FollowingSoulBreaker>().UnfreezeDissolvement();
        }
        StartWinDissolvement();
    }

    // Enaables Angel's movement and disables trail after dashing and starts countdown before next dash
    void EndDashing(float seconds)
    {
        rigidBody.velocity = Vector2.zero;
        // Prevent countdown from happening when player decides to continue dashing with stored dashes
        StopCoroutine("WaitForNextDash");
        // Start countdown before next dash
        trailRenderer.emitting = false;
        movement.enabled = true;
        // Make Angel unable to destroy SB when not dashing
        soulBreakersScript.AllSoulBreakersSetTriggerFalse();
        // Make player wait before next dash
        StartCoroutine(WaitForNextDash(seconds));
        // To be able to show dashLeftovers for next dash
        instantiatedDashLeftovers = false;
        // Make Angel able to go through FlashingGates if he is in slow-mo
        FindObjectOfType<FlashingGates>().SetIsAngelInDash(false);
        // Let SoulBreakers.cs to check whether Angel destroyed last SB
        FindObjectOfType<SoulBreakers>().LaunchPostWinPhase();
        // If angel is still being registered as in slow mo by PlayerControls.cs, then adjust timeflow
        if (playerInput.IsAngelInSlowmo()) {
            timeFlow.setTimeFlow(timeFlow.getSlowmoTime());
        }

        // Note: Probably not necessary anymore after implementing new bound system with two colliders for each wall...
        // // If player makes InitDash when Ryker is close to the wall and the dashDirection is towards the wall, then the timeflow will be blocked at 0 # BUGFIX
        // if (timeFlow.getTimeFlow() == 0) timeFlow.setTimeFlow(timeFlow.getAngelTime());
    }

    IEnumerator WaitForNextDash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
          
        canDash = true;
        dashIndicatorsScript.IndicateBaseDashReady();
    }

    // Dissolvement after win
    private void HandleDissolvementAfterWin()
    {
        if (isDissolving && dissolveDiscountFactor != 0) {
            material.SetFloat("_Fade", material.GetFloat("_Fade") - dissolveDiscountFactor);
            if (material.GetFloat("_Fade") <= 0) {
                isDissolving = false;
                dissolveDiscountFactor = 0;
                // Ryker.cs takes care of destroying this gameobject
            }
        }
    }

    //
    //  SHIFT
    //
    // Recharges shift charges and shows them if player is showing shiftAreaIndicator
    private void HandleShiftRecharge() {
        // Ryker
        if (shiftRykerChargeCounter < shiftRykerChargeTime) {
            shiftRykerChargeCounter += Time.deltaTime;
        }else if (shiftIndicator.activeSelf) {
            shiftRykerChargeIndicator.SetActive(true);
        }
        // Projectiles
        if (shiftProjectilesChargeCounter < shiftProjectilesChargeTime) {
            shiftProjectilesChargeCounter += Time.deltaTime;
        } else if (shiftIndicator.activeSelf) {
            shiftProjectilesChargeIndicator.SetActive(true);
        }
    }

    private void ShowShiftIndicators() {
        shiftIndicator.SetActive(true);
        if (shiftRykerChargeCounter >= shiftRykerChargeTime) {
            shiftRykerChargeIndicator.SetActive(true);
        }
        if (shiftProjectilesChargeCounter >= shiftProjectilesChargeTime) {
            shiftProjectilesChargeIndicator.SetActive(true);
        }
    }

    public void HideShiftIndicators() {
        shiftIndicator.SetActive(false);
        shiftRykerChargeIndicator.SetActive(false);
        shiftProjectilesChargeIndicator.SetActive(false);
    }
    // Just a call with additional functionalities called by PlayerControls.cs
    public void HideShiftIndicatorsByInput() {
        HideShiftIndicators();
        isAimingShift = false;
        dashIndicatorsScript.DrawInDashDirection();
        dashIndicatorsScript.ShowAllStoredDashIndicators();
    }

    private void ShiftRyker() {
        GameObject rykerReference = shiftIndicatorScript.GetRykerWithinColliderReference();
            // Return if Ryker is not within shiftIndicator
            if(rykerReference == null) {return;}
            //apply force on Ryker in direction of shiftindicator
            Vector3 shiftDirection = -(transform.position - rykerReference.transform.position).normalized;
            rykerReference.GetComponent<AbilityShiftable>().ShiftYourslef( shiftDirection, 
                                                                transform.position, 
                                                                shiftIndicator.GetComponent<Collider2D>().bounds.extents.y * 2, 
                                                                shiftSpeed, 
                                                                maxShiftDistance);

            shiftRykerChargeCounter = 0f;
            shiftRykerChargeIndicator.SetActive(false);
    }

    private void ShiftProjectiles() {
        // Get all gameObjects in shift indicator collider
            List<GameObject> projectilesWithinCollider = shiftIndicatorScript.GetProjectilesWithinCollider();
            // at least some optimalization
            if (projectilesWithinCollider.Count == 0) { return; }
            // check if ryker is inside and if so, then tell him to shift
            // also check if projectile with AbilityShiftable and if so, then tell it to shift itself
            foreach (GameObject obj in projectilesWithinCollider)
            {
                if (obj.GetComponent<AbilityShiftable>())
                {
                    //apply force on projectile in direction of shiftindicator
                    Vector3 shiftDirection = -(transform.position - obj.transform.position).normalized;
                    obj.GetComponent<AbilityShiftable>().ShiftYourslef( shiftDirection, 
                                                                        transform.position, 
                                                                        shiftIndicator.GetComponent<Collider2D>().bounds.extents.y * 2, 
                                                                        shiftSpeed, 
                                                                        maxShiftDistance);
                }
            }

            shiftProjectilesChargeCounter = 0f;
            shiftProjectilesChargeIndicator.SetActive(false);
    }

    //
    //  SLOWMO
    //
    // Decrease or restore slowmo limit based on canDash boolean and also visually represent it
    private void HandleSlowMoCountdown()
    {
        // Do not decrease or restore slowmolimit when Angel is in dash
        if (isInitDashing || isAttacking) { return; }

        // Adjust fillrate of slowmo harm material shader graph
        float harmRate = (slowMoLimit - currentSlowMoLimit) / slowMoLimit;
        dashIndicatorsScript.SetSlowMoHarmValue(harmRate);

        // Decrease or slowely restore slowmo limit based on canDash
        if(isAimingDash)
        {
            currentSlowMoLimit -= slowMoLimitDischargeRate;
        }else if (currentSlowMoLimit < slowMoLimit && !isAimingDash && canAimDash)
        {
            currentSlowMoLimit += slowMoLimitRechargeRate;
        }else if (!canAimDash)
        {
            currentSlowMoLimit += slowMoLimitRestoreRate;
        }
    }

    // Slows time and shows dashdirection
    private void StartSlowmo()
    {
        timeFlow.setTimeFlow(timeFlow.getSlowmoTime());
        playerInput.SetAngelInSlowmo(true);
        SlowDownCountdown();
    }

    // Ends slowmo (sets timeflow to normal) and hides dashdirection. Function called from Update when player releases RightMouseButton
    private void EndSlowmo()
    {
        if (!isAimingShift) { dashIndicatorsScript.DrawInDashDirection(); }
        isAimingDash = false;
        timeFlow.setTimeFlow(timeFlow.getAngelTime());
        ResumeNormalCountdown();
    }
    // Just a call with additional functionalities called by PlayerControls.cs
    public void EndSlowmoByInput() {
        dashIndicatorsScript.DrawInDashDirection();
        EndSlowmo();
    }

    // Call this function in response to an event to slow down the countdown
    public void SlowDownCountdown()
    {
        countdownDenominator = timeFlow.getSlowmoTime();
    }

    // Call this function to resume normal countdown speed
    public void ResumeNormalCountdown()
    {
        countdownDenominator = timeFlow.getNormalTime();
    }


    public void AimShift()
    {
        if(!isAimingDash){
            isAimingShift = true;
            ShowShiftIndicators();
            dashIndicatorsScript.HideDashDirection();
            dashIndicatorsScript.HideAllStoredDashIndicators();
        }
    }

    //
    // STORING DASHES
    //
    // For storing dashes via DashRenewer object
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "DashRenewer" && storedDashes < 5) {
            storedDashes++;
            dashIndicatorsScript.ShowNewStoredDashIndicator();
            Destroy(other.gameObject);
            return;
        } else if(other.gameObject.tag == "Projectile" && CheckProjectileHarmful(other.gameObject) && !isAttacking) {
            ryker.GetComponent<Ryker>().PlayerDied();
            Destroy(gameObject);
        }
    }

    private bool CheckProjectileHarmful(GameObject projectile) {
        if (projectile.transform.parent.GetComponent<Projectile>() != null) {
            if (projectile.transform.parent.GetComponent<Projectile>().IsHarmful()) {
                return true;
            }
        }
        if (projectile.GetComponent<Projectile>() != null) {
            if (projectile.GetComponent<Projectile>().IsHarmful()) {
                return true;
            }
        }
        return false;
    }

    //
    // If Angel hits environmental objects it ends the dash before it reaches maximum distance, 
    // if the dash is not over 2/3 of it's duration (frames)
    void OnCollisionEnter2D(Collision2D other)
    {
        dashDirection = new Vector2(0,0);
        SetIsInitDashing(false);
        SetIsAttacking(false);
        
        if (dashDurationFramesCount > Mathf.Ceil(dashDurationFrames * (1f/3f))) {
            canDash = true;
            EndDashing(firstAttackWait);
        }else {
            EndDashing(nextAttackWait);
        }
    }
}