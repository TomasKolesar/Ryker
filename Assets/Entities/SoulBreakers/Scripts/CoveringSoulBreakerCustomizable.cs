using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveringSoulBreakerCustomizable : MonoBehaviour
{
    [SerializeField] private float timeBetweenFiring = 1f;
    private float timeSinceLastFire = 0f;
    private bool activated = false;

    [SerializeField] private List<GameObject> canons;

    // private GameObject pointer;
    private TimeFlow timeFlow;

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
    }

    private void Update() {
        float deltaOfTime = Time.deltaTime;
        if (timeFlow.getTimeFlow() == timeFlow.getSlowmoTime()) {
            deltaOfTime *= (timeFlow.getSlowmoTime() / timeFlow.getAngelTime());
        }
        timeSinceLastFire += deltaOfTime;
        
        if (timeSinceLastFire >= timeBetweenFiring && activated) {
            timeSinceLastFire = 0f;
            // Iterate through all canons and fire
            foreach(GameObject canon in canons){
                canon.GetComponent<CoveringSoulBreakerCustomizableCanon>().Fire();
            }
        }
    }

    public void StartFiring() {
        activated = true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        CheckIfAngelDashedThrough(other);
        CheckIfHitByHarmfulProjectile(other);
    }

    private void CheckIfAngelDashedThrough(Collider2D other) {
        if(other.gameObject.tag == "Angel" && other.gameObject.GetComponent<Angel>().IsInDash()){
            // Remove from parental list
            transform.parent.GetComponent<SoulBreakers>().RemoveSoulBreaker(gameObject);
            // Handling destruction of SB
            // Destroy(pointer);
            Destroy(gameObject);
        }
    }

    private void CheckIfHitByHarmfulProjectile(Collider2D other) {

    }
}
