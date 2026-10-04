using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerSB : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject origin;
    [SerializeField] private float borderOffset = 20f;
    private Vector3 targetPosition;
    private Vector3 originPosition;
    private Image image;
    private bool isPlayingForAngel = false;
    
    // Coloring
    [SerializeField] private GameObject projectile;
    [SerializeField] private Vector3 projectileStartPosition;
    [SerializeField] private GameObject projectileTarget;

    //
    // Setters
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void SetOrigin(GameObject origin)
    {
        this.origin = origin;
    }

    public void SetIsPlayingForAngel(bool isPlayingForAngel)
    {
        this.isPlayingForAngel = isPlayingForAngel;
    }

    public void SetProjectile(GameObject projectile)
    {
        this.projectile = projectile;
    }

    public void SetProjectileStartPosition(Vector3 projectileStartPosition)
    {
        this.projectileStartPosition = projectileStartPosition;
    }

    public void SetProjectileTarget(GameObject projectileTarget)
    {
        this.projectileTarget = projectileTarget;
    }

    //
    // Start is called before the first frame update
    void Start()
    {
        //
        image = GetComponent<Image>();
        //
        targetPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (projectile != null && projectileStartPosition != null && projectileTarget != null)
        {
            // Coloring
            float closenessOfProjectile =  Vector3.Distance(projectileStartPosition, projectile.transform.position) / 
                                            Vector3.Distance(projectileStartPosition, projectileTarget.transform.position);
            image.color = Color.Lerp(Color.white, Color.red, closenessOfProjectile);
        }

        if (isPlayingForAngel) {
            // Rotates the arrow to point towards the target
            originPosition = origin.transform.position;
            Vector3 direction = (targetPosition - originPosition).normalized;
            float angle = GetAngleFromVectorFloat(direction);
            transform.localEulerAngles = new Vector3(0, 0, angle);

            // Check if target is off screen
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
            bool isOffScreen =  targetPositionScreenPoint.x <= borderOffset ||
                                targetPositionScreenPoint.x >= Screen.width - borderOffset||
                                targetPositionScreenPoint.y <= borderOffset ||
                                targetPositionScreenPoint.y >= Screen.height - borderOffset;
            
            // Adjust positioning if target is off screen
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if(isOffScreen)
            {
                // Show pointer on screen and figure it's position
                image.enabled = true;
                if (cappedTargetScreenPosition.x <= borderOffset) cappedTargetScreenPosition.x = borderOffset;
                if (cappedTargetScreenPosition.x >= Screen.width - borderOffset) cappedTargetScreenPosition.x = Screen.width - borderOffset;
                if (cappedTargetScreenPosition.y <= borderOffset) cappedTargetScreenPosition.y = borderOffset;
                if (cappedTargetScreenPosition.y >= Screen.height - borderOffset) cappedTargetScreenPosition.y = Screen.height - borderOffset;
            
                // Set the position of the arrow
                transform.position = cappedTargetScreenPosition;
            }else {
                image.enabled = false;
            }
        }
    }

    public static float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
