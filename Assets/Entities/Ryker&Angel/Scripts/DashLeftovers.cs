using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashLeftovers : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Update()
    {
          if(animator.GetCurrentAnimatorStateInfo(0).IsName("dashLeftoversPostExplosion")) {
            Destroy(transform.parent.gameObject);
          }
    }
}
