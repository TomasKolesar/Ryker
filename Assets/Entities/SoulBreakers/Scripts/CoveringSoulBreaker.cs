using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoveringSoulBreaker : MonoBehaviour
{
    // Firing
    [SerializeField] private float timeBetweenFiring = 1f;
    private float timeSinceLastFire = 0f;
    private bool activated = false;

    // Projectiles reference
    private List<GameObject> currentProjectiles = new List<GameObject>();

    // Canons reference
    [SerializeField] private List<GameObject> canons;

    // Components
    private TimeFlow timeFlow;
    private Collider2D coll;

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
    public void SetTriggerTrue() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void SetTriggerFalse() {
        GetComponent<BoxCollider2D>().isTrigger = false;
    }

    //
    // Start is called before the first frame update
    void Start()
    {
        timeFlow = GameObject.Find("TimeFlow").GetComponent<TimeFlow>();
        timeSinceLastFire = timeBetweenFiring; // To fire at the start
        coll = GetComponent<Collider2D>();
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update() {
        // Dissolvement
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

        // Firing periodically
        float deltaOfTime = Time.deltaTime;
        if (timeFlow.getTimeFlow() == timeFlow.getSlowmoTime()) {
            deltaOfTime *= (timeFlow.getSlowmoTime() / timeFlow.getAngelTime());
        }
        timeSinceLastFire += deltaOfTime;
        
        if (timeSinceLastFire >= timeBetweenFiring && activated) {
            timeSinceLastFire = 0f;
            // Iterate through all canons and fire
            foreach(GameObject canon in canons){
                // Fire
                currentProjectiles.Add(GetComponent<ProjectileFactory>().CraftProjectile());
                currentProjectiles.Last().transform.SetParent(canon.transform);
                currentProjectiles.Last().transform.position = canon.transform.position; // I do now know why do I have to set it manually, otherwise spawning at (0,0)
                currentProjectiles.Last().GetComponent<Projectile>().SetIsCoveringTrue();
                currentProjectiles.Last().GetComponent<Projectile>().Activate();
            }
        }
    }

    public void StartFiring() {
        activated = true;
    }

    // Preventing SB from detecting that it should destroy itself when instantiating projectiles withint its own collider
    private void OnTriggerExit2D(Collider2D other) {
        // CheckIfAngelDashedThrough(other);

        if (other.gameObject.GetComponent<AbilityGuidedUnguided>() == null) return;

        if(other.gameObject.GetComponent<Projectile>() != null) {
            other.gameObject.GetComponent<Projectile>().SetLeftSoulBreakersCollider(true);
        }else if(other.transform.parent.gameObject.GetComponent<Projectile>() != null) {
            other.transform.parent.gameObject.GetComponent<Projectile>().SetLeftSoulBreakersCollider(true);
        }
    }

    // Handling collisions with Angel and harmful projectiles
    private void OnTriggerEnter2D(Collider2D other) 
    {
        CheckIfAngelDashedThrough(other);
        CheckIfHitByHarmfulProjectile(other);
    }

    private void CheckIfAngelDashedThrough(Collider2D other) {
        if(other.gameObject.tag == "Angel" && other.gameObject.GetComponent<Angel>().IsInDash()){
            HandleDestroyment(other);
        }
    }

    private void CheckIfHitByHarmfulProjectile(Collider2D other) {
        if (other.gameObject.GetComponent<AbilityGuidedUnguided>() == null) return;

        bool leftCollider = CheckWhetherProjectileLeftCollider(other);
        bool harmful = CheckWhetherProjectileHarmful(other);
        if(other.gameObject.GetComponent<Projectile>() != null && leftCollider && harmful) {
                Debug.Log("Dying by hit from harmful projectile");
        }else if(other.transform.parent.gameObject.GetComponent<Projectile>() != null && leftCollider && harmful) {
                Debug.Log("Dying by hit from harmful projectile");
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
        isDissolving = true;
        // Prevent projectiles from hitting anything and dissolve them
        if (currentProjectiles.Count > 0) {
            foreach(GameObject projectile in currentProjectiles) {
                projectile.GetComponent<Projectile>().DisableAllColliders();
                projectile.GetComponent<Projectile>().DissolveAllProjectiles(dissolveShader, dissolveSpeed);
            }
        }
        currentProjectiles.Clear();
        // Prevent SB while dissolving from getting hit again or stop Angel's dash
        Physics2D.IgnoreCollision(coll, destroyer, true);
    }

    private void Die() {
        SoulBreakers sbs = transform.parent.GetComponent<SoulBreakers>();
        if (sbs.GetLastSBReference() != null) {
            sbs.LaunchPostWinPhase();
        }
        Destroy(gameObject);
    }
}
