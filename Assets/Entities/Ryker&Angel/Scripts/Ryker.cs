using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ryker : MonoBehaviour
{

    [Header("Dash Direction Arrow")]
    [SerializeField] private GameObject dashDirection;
    
    // Dash processing
    private static bool leftSpawn = false;
    private static bool isInDash = false;
    private Angel currentAngel;
    private bool isAimingDash = false;
    public bool IsAimingDash() { return isAimingDash; }

    // Angel for instantiation
    [SerializeField] private Angel angelPrefab;
    private bool isDialogueAfterWin = false;
    public void SetIsDialogueAfterWin(bool value) {isDialogueAfterWin = value;}

    // Components that change based on who is player in control
    private CircleCollider2D circleCollider2D;

    // For freezing position so ryker won't make any movement after initdash
    private Vector3 positionBeforeDash;
    // private bool keepPositionAfterInitDash = false;
    // public void StopKeepingPositionAfterDash() {
    //     keepPositionAfterInitDash = false;
    // }

    // Death handling
    private bool isDead = false;

    // Win handling
    private GameObject elevator;

    // TimeFlow for slowing time
    private TimeFlow timeFlow;

    // Input
    private PlayerControls playerInput;

    // CameraFocusPoint for focusing camera between Angel and mouse cursor when palyer is controlling Angel
    private CameraFocusPoint cameraFocusPoint;

    // Pointer
    [Header("Pointer")]
    private GameObject rykerPointer;
    [SerializeField] private GameObject pointerCautionArea;

    // BoostPlatform check
    private bool isInBoost = false;
    public bool getIsInBoost() {return isInBoost;}
    public void setIsInBoost(bool value) {isInBoost = value;}
    public void HideDashDirection() {  dashDirection.SetActive(false);  }
    public void HideDashDirectionByInput() {
        HideDashDirection();
        isAimingDash = false;
    }
    public void ShowDashDirection() {
        if (leftSpawn && !isInDash && !isInBoost) {
            dashDirection.SetActive(true);
            isAimingDash = true;
        }
    }


    //
    // Instantiating component variable references and taking care of dash direction arrow
    void Awake()
    {
        // Input
        playerInput = FindObjectOfType<PlayerControls>();
        // Ryker's components
        circleCollider2D = GetComponent<CircleCollider2D>();
        // Director tools
        timeFlow = FindObjectOfType<TimeFlow>();
        cameraFocusPoint = FindObjectOfType<CameraFocusPoint>();
        // Pointer
        rykerPointer = FindObjectOfType<Pointers>().GetRykerPointer();
        // Elevator
        elevator = FindObjectOfType<Elevator>().gameObject;
        elevator.SetActive(false);
        

        // Hide dash direction arrow at the start, because player needs to leave Spawn to show it
        HideDashDirection();

        leftSpawn = false;
        isInDash = false;
    }

    private void Start() {
    }

    private void Update() {
        // if (keepPositionAfterInitDash)
        // {
        //     transform.position = positionBeforeDash;
        // }
    }

    //
    // Input: Instantiating Angel, that makes initial dash
    public void InitDash()
    {
        if (leftSpawn && !isInDash && !isInBoost)
        {   
            // Set that Ryker is in dash
            isInDash = true;
            
            positionBeforeDash = transform.position;

            // Slow world time flow
            timeFlow.setTimeFlow(timeFlow.getAngelTime());

            // Make Ryker's collider a trigger so Angel can dash through (during shifting of Ryker, it becomes non-trigger again)
            circleCollider2D.isTrigger = true;

            // Deactivate everyhting on Ryker, so Angel can dance
            HideDashDirection();

            // Modifying position for Angel instantiation, because of all possible reasons to stop InitDash
            Vector3 angelInstantiatePosition = transform.position;
            // If player makes InitDash while Ryker is moving, Angel will be instantiated on a position
            // Ryker was 1 frame ago, because of collision with the wall
            Vector2 vel = GetComponent<Movement>().GetVelocity();
            if (vel != Vector2.zero) {
                angelInstantiatePosition -= new Vector3(vel.x * Time.deltaTime, vel.y * Time.deltaTime, 0);
                // Preventing Ryker from going into wall if initdash is made while moving close to collider
                GetComponent<Movement>().StopMovement();
            }
            // Every wall has a second collider that is slightly bigger than the main one,
            // those colliders are non-triggers when the player's playing for Ryker,
            // but at the moment he makes InitDash, they switch to Triggers,
            // what causes Ryker instantiating Angel a little off the wall.
            // This prevents Angel from stopping the dash because of collision with the wall
            GetComponent<AbilityShiftable>().SetWallBoundCollidersTrigger();
            // Instantiate angel, he then handles InitDash on his own
            currentAngel = Instantiate(angelPrefab, angelInstantiatePosition, Quaternion.identity);
            currentAngel.GetComponent<Angel>().SetRyker(this.gameObject);
            currentAngel.GetComponent<Angel>().SetIsDialogueAfterWin(isDialogueAfterWin);
            playerInput.SetAngel(currentAngel);
            // Reset movement variable, so after win, player can move Ryker again and Ryker won't move himself

            // Give instructions to parental gameobjects
            PlayerMadeInitDash();
            
            // Make camera focus between Angel and mouse cursor and narrow it's orthographic size
            cameraFocusPoint.SetAngel(currentAngel.gameObject);
            cameraFocusPoint.SetPlayerControllingAngel(true);
            cameraFocusPoint.StartZoomingToAngel();

            // Show caution area for pointer; uncomment later in case of implementing pointers
            // pointerCautionArea.SetActive(true);
        }
    }

    // Gives instruction to all parental components, so they adjust children for Angel's gameplay
    private void PlayerMadeInitDash(){
        FindObjectOfType<SoulBreakers>().AllSoulBreakersFire();
        FindObjectOfType<DashRenewers>().EnableDashRenewers();
        FindObjectOfType<BoostPlatforms>().DeactivateBoostPlatforms();
        FindObjectOfType<FlashingGates>().DisableIsTrigger();
        FindObjectOfType<Pointers>().SetNavigationForAngel();
        rykerPointer.GetComponent<PointerRyker>().SetOrigin(currentAngel.gameObject);

        // keepPositionAfterInitDash = true;
    }

    //
    // OLD: Change controls again to Ryker and delete instance of Angel
    public void ReturnToRyker(){
        // Take back players input control
        circleCollider2D.isTrigger = false;
        isInDash = false;
        // keepPositionAfterInitDash = false;

        // Reset world time flow
        timeFlow.setTimeFlow(timeFlow.getNormalTime());

        // Make camera focos strictly on Ryker
        cameraFocusPoint.SetPlayerControllingAngel(false);
        cameraFocusPoint.SetAngel(null);

        //
        FindObjectOfType<Pointers>().DestroyAllSBPointers();
    }

    //
    //
    public void CautionPointer() {
        rykerPointer.GetComponent<Image>().color = Color.red;
    }

    //
    //
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "SpawnPlatform")
        {
            leftSpawn = true;
        }
    }

    //
    // Handling death logic here
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Projectile" || other.gameObject.tag == "FlashingGate")
        {
            PlayerDied();
        }
    }

    //
    // Triggered by pressing "R" key
    public void Reload()
    {
        if (isDead)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }


    private void LetOthersKnowGameEnded()
    {
        FindObjectOfType<CameraFocusPoint>().StopFollowingAngel();
        FindObjectOfType<Pointers>().DestroyAllSBPointers();
        cameraFocusPoint.SetPlayerControllingAngel(false);
        Destroy(rykerPointer);
    }

    public void PlayerWon()
    {
        if(currentAngel != null)
        {
            Destroy(currentAngel.gameObject);
            playerInput.DestroyAngelReference();
            currentAngel = null;
        }

        ReturnToRyker();
        FindObjectOfType<CanvasWinDeath>().ShowWinTexts();

        elevator.SetActive(true);

        LetOthersKnowGameEnded();

        playerInput.SetWin();
    }

    public void PlayerDied()
    {
        // stop movement
        GetComponent<Movement>().StopMovement();
        // stop time
        timeFlow.setTimeFlow(0);
        LetOthersKnowGameEnded();
        isDead = true;
        FindObjectOfType<CanvasWinDeath>().ShowDeathTexts();
        // Death when player is playing as Ryker
        if(currentAngel == null)
        {
            HideDashDirection();
        // Death when player is playing as Angel
        }else{
            Destroy(currentAngel.gameObject);
            playerInput.DestroyAngelReference();
        }
        playerInput.SetDead();
        
    }

    public void AngelDied()
    {
        currentAngel = null;
        timeFlow.setTimeFlow(timeFlow.getNormalTime());
    }
}