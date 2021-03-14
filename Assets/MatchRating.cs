using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchRating : MonoBehaviour {
    Image imageUI;
    Animator animator;
    bool activated;
    public int requiredPointsToActivateThisRate;
	void Start()
    {
        imageUI = GetComponent<Image>();
        animator = GetComponent<Animator>();
        imageUI.CrossFadeAlpha(0f, 0f, true);
    }

    void Update()
    {
        if (!activated)
        {
            if(Points.i.currentPoints >= requiredPointsToActivateThisRate)
            {
                activated = true;
                imageUI.CrossFadeAlpha(1f, 0.25f, true);
                animator.SetTrigger("glance");
            }
        }
    }
	
}
